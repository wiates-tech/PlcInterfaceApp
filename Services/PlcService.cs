using EasyModbus;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcInterfaceApp.Services
{
    public class PlcService : IPlcService
    {
        private ModbusClient modbusClient;
        private Plc s7Plc;

        public async Task<bool> ConnectAsync(string type, string ip, int port)
        {
            if (type == "Modbus")
            {
                modbusClient = new ModbusClient(ip, port);
                modbusClient.ConnectionTimeout = 2000;
                modbusClient.Connect();
                return modbusClient.Connected;
            }
            else if (type == "S7")
            {
                s7Plc = new Plc(CpuType.S71500, ip, 0, 1);
                return await Task.Run(() =>
                {
                    s7Plc.Open();
                    return s7Plc.IsConnected;
                });
            }
            return false;
        }

        public async Task<string> ReadValueAsync(string type, string address, string dataType)
        {
            return await Task.Run(() =>
            {
                if (type == "Modbus")
                {
                    if (dataType == "Int")
                        return modbusClient.ReadHoldingRegisters(int.Parse(address), 1)[0].ToString();
                    else if (dataType == "Bool")
                        return modbusClient.ReadCoils(int.Parse(address), 1)[0].ToString();
                }
                else if (type == "S7")
                {
                    if (dataType == "Int")
                        return ((short)s7Plc.Read($"DB1.DBW{address}")).ToString();
                    else if (dataType == "Bool")
                        return ((bool)s7Plc.Read($"DB1.DBX{address}")).ToString();
                }
                return "Unsupported";
            });
        }

        public async Task<bool> WriteValueAsync(string type, string address, string dataType, string value)
        {
            return await Task.Run(() =>
            {
                if (type == "Modbus")
                {
                    if (dataType == "Int")
                        modbusClient.WriteSingleRegister(int.Parse(address), int.Parse(value));
                    else if (dataType == "Bool")
                        modbusClient.WriteSingleCoil(int.Parse(address), bool.Parse(value));
                }
                else if (type == "S7")
                {
                    if (dataType == "Int")
                        s7Plc.Write($"DB1.DBW{address}", short.Parse(value));
                    else if (dataType == "Bool")
                        s7Plc.Write($"DB1.DBX{address}", bool.Parse(value));
                }
                return true;
            });
        }
    }
}
