using Glash.Core;
using Glash.Server;
using Quick.EntityFrameworkCore.Plus;

namespace Glash.Blazor.Server
{
    public class AgentManager : IAgentManager
    {
        public bool Login(LoginInfo loginInfo)
        {
            var model = ConfigDbContext.CacheContext
                        .Find(new Model.AgentInfo(loginInfo.Name));
            if (model == null)
                return false;
            var answer = CryptoUtils.GetAnswer(loginInfo.Question, model.Password);
            return answer == loginInfo.Answer;
        }
    }
}
