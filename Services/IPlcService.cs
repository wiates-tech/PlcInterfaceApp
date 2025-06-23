using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcInterfaceApp.Services
{
    public interface IPlcService
    {
        Task<bool> ConnectAsync(string type, string ip, int port);
        Task<string> ReadValueAsync(string type, string address, string dataType);
        Task<bool> WriteValueAsync(string type, string address, string dataType, string value);
    }
}
