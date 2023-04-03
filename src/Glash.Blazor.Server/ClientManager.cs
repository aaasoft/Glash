using Glash.Core;
using Glash.Server;
using Quick.EntityFrameworkCore.Plus;
using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Blazor.Server
{
    public class ClientManager : IClientManager
    {
        public bool Login(LoginInfo loginInfo)
        {
            var model = ConfigDbContext.CacheContext
                .Find(new Model.ClientInfo(loginInfo.Name));
            if (model == null)
                return false;
            var answer = CryptoUtils.GetAnswer(loginInfo.Question, model.Password);
            return answer == loginInfo.Answer;
        }

        public string[] GetClientRelateAgents(string clientName)
        {
            return ConfigDbContext.CacheContext
                .Query<Glash.Blazor.Server.Model.ClientAgentRelation>()
                .Where(t => t.ClientName == clientName)
                .Select(t => t.AgentName)
                .ToArray();
        }

        public bool IsClientRelateAgent(string clientName, string agnetName)
        {
            var model = ConfigDbContext.CacheContext.Find(new Glash.Blazor.Server.Model.ClientAgentRelation()
            {
                ClientName = clientName,
                AgentName = agnetName
            });
            return model != null;
        }

    }
}
