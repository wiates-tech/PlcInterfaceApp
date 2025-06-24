using EasyModbus;
using S7.Net;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PlcInterfaceApp.Services
{
    public class PlcService : IPlcService
    {
        // Fields for Modbus and S7 connections
        private ModbusClient modbusClient;
        private Plc s7Plc;

        // Property to check if either Modbus or S7 PLC is connected
        public bool IsConnected
        {
            get
            {
                return (modbusClient != null && modbusClient.Connected) ||
                       (s7Plc != null && s7Plc.IsConnected);
            }
        }

        /* ConnectAsync method to establish connection to PLCs */
        public async Task<bool> ConnectAsync(string type, string ip, int port)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ip))
                    throw new ArgumentException("IP address cannot be null or empty.");

                if (port <= 0 || port > 65535)
                    throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1 and 65535.");

                if (type == "Modbus")
                {
                    try
                    {
                        modbusClient = new ModbusClient(ip, port)
                        {
                            ConnectionTimeout = 3000
                        };
                        modbusClient.Connect();
                        return modbusClient.Connected;
                    }
                    catch (SocketException sockEx)
                    {
                        Console.WriteLine($"Modbus connection failed: {sockEx.Message}");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Modbus unexpected error: {ex.Message}");
                        return false;
                    }
                }
                else if (type == "S7")
                {
                    s7Plc = new Plc(CpuType.S71200, ip, 0, 1);

                    return await Task.Run(() =>
                    {
                        try
                        {
                            s7Plc.Open();
                            return s7Plc.IsConnected;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"S7 general Task error: {ex.Message}");
                            return false;
                        }
                    });
                }

                else
                {
                    Console.WriteLine("Unsupported PLC type specified.");
                    return false;
                }
            }
            catch (Exception topEx)
            {
                Console.WriteLine($"General connection failure: {topEx.Message}");
                return false;
            }
        }

        /* ReadValueAsync method to read values from PLCs */
        public async Task<string> ReadValueAsync(string type, string address, string dataType)
        {
            return await Task.Run(() =>
            {
                try
                {
                    int addr = int.Parse(address);

                    if (type == "Modbus")
                    {
                        switch (dataType)
                        {
                            case "Int":
                                return modbusClient.ReadHoldingRegisters(addr, 1)[0].ToString();

                            case "Bool":
                                return modbusClient.ReadCoils(addr, 1)[0].ToString();

                            case "Real":
                                var floatWords = modbusClient.ReadHoldingRegisters(addr, 2);
                                byte[] floatBytes = new byte[4];
                                Buffer.BlockCopy(BitConverter.GetBytes(floatWords[1]), 0, floatBytes, 0, 2);
                                Buffer.BlockCopy(BitConverter.GetBytes(floatWords[0]), 0, floatBytes, 2, 2);
                                return BitConverter.ToSingle(floatBytes, 0).ToString("F2");

                            case "Char":
                                var word = modbusClient.ReadHoldingRegisters(addr, 1)[0];
                                char high = (char)(word >> 8);
                                char low = (char)(word & 0xFF);
                                return $"{high}{low}".Trim('\0');

                            case "String":
                                int numRegisters = 10;
                                var regs = modbusClient.ReadHoldingRegisters(addr, numRegisters);
                                byte[] strBytes = new byte[numRegisters * 2];
                                for (int i = 0; i < numRegisters; i++)
                                {
                                    strBytes[2 * i] = (byte)(regs[i] >> 8);
                                    strBytes[2 * i + 1] = (byte)(regs[i] & 0xFF);
                                }
                                return Encoding.ASCII.GetString(strBytes).TrimEnd('\0');
                        }
                    }
                    else if (type == "S7")
                    {
                        switch (dataType)
                        {
                            case "Int":
                                return s7Plc.Read($"DB1.DBW{addr}").ToString();

                            case "Real":
                                return s7Plc.Read(DataType.DataBlock, 1, addr, VarType.Real, 1).ToString();

                            case "Bool":
                                return s7Plc.Read($"DB1.DBX2.{addr}").ToString();

                            case "Char":
                                byte b = (byte)s7Plc.Read(DataType.DataBlock, 1, addr, VarType.Byte, 1);
                                return Convert.ToChar(b).ToString();

                            case "String":
                                return ((string)s7Plc.Read(DataType.DataBlock, 1, addr, VarType.String, 10)).Trim('\0');
                        }
                    }

                    return "Unsupported";
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            });
        }

        /* WriteValueAsync method to write values to PLCs */
        public async Task<bool> WriteValueAsync(string type, string address, string dataType, string value)
        {
            return await Task.Run(() =>
            {
                try
                {
                    int addr = int.Parse(address);

                    if (type == "Modbus")
                    {
                        switch (dataType)
                        {
                            case "Int":
                                modbusClient.WriteSingleRegister(addr, int.Parse(value));
                                break;

                            case "Bool":
                                modbusClient.WriteSingleCoil(addr, bool.Parse(value));
                                break;

                            case "Real":
                                float realVal = float.Parse(value);
                                byte[] floatBytes = BitConverter.GetBytes(realVal);
                                ushort[] floatUShorts = new ushort[2];
                                floatUShorts[0] = BitConverter.ToUInt16(floatBytes, 2);
                                floatUShorts[1] = BitConverter.ToUInt16(floatBytes, 0);
                                modbusClient.WriteMultipleRegisters(addr, floatUShorts.Select(x => (int)x).ToArray());
                                break;

                            case "Char":
                                char c = value.Length > 0 ? value[0] : '\0';
                                modbusClient.WriteSingleRegister(addr, (ushort)c);
                                break;

                            case "String":
                                int regCount = (int)Math.Ceiling(value.Length / 2.0);
                                ushort[] strUShorts = new ushort[regCount];
                                for (int i = 0; i < value.Length; i += 2)
                                {
                                    byte b1 = (byte)value[i];
                                    byte b2 = (i + 1 < value.Length) ? (byte)value[i + 1] : (byte)0;
                                    strUShorts[i / 2] = (ushort)((b1 << 8) | b2);
                                }
                                modbusClient.WriteMultipleRegisters(addr, strUShorts.Select(x => (int)x).ToArray());
                                break;
                        }
                    }
                    else if (type == "S7")
                    {
                        switch (dataType)
                        {
                            case "Int":
                                s7Plc.Write($"DB1.DBW{addr}", short.Parse(value));
                                break;

                            case "Real":
                                s7Plc.Write(DataType.DataBlock, 1, addr, float.Parse(value));
                                break;

                            case "Bool":
                                s7Plc.Write($"DB1.DBX2.{addr}", bool.Parse(value));
                                break;

                            case "Char":
                                s7Plc.Write(DataType.DataBlock, 1, addr, (byte)(value.Length > 0 ? value[0] : '\0'));
                                break;

                            case "String":
                                string strValue = value ?? "";
                                int maxLen = 10;
                                byte[] buffer = new byte[2 + maxLen];

                                buffer[0] = (byte)maxLen;
                                buffer[1] = (byte)Math.Min(strValue.Length, maxLen);

                                Encoding.ASCII.GetBytes(strValue.PadRight(maxLen, '\0'), 0, Math.Min(strValue.Length, maxLen), buffer, 2);

                                s7Plc.WriteBytes(DataType.DataBlock, 1, addr, buffer);
                                break;
                        }
                    }

                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }
            });
        }
    }
}