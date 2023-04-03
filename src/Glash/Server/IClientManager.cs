using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Server
{
    public interface IClientManager
    {
        bool Login(LoginInfo loginInfo);
        string[] GetClientRelateAgents(string clientName);
        public bool IsClientRelateAgent(string clientName, string agnetName);
    }
}
