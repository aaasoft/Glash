using Quick.EntityFrameworkCore.Plus;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Server.BlazorApp.Model
{
    [Table($"{nameof(Glash)}_{nameof(Server)}_{nameof(ClientAgentRelation)}")]
    public class ClientAgentRelation : IHasDependcyRelation
    {
        public string ClientId { get; set; }
        public string AgentId { get; set; }

        public override int GetHashCode()
        {
            return this.GetHashCode(
                t => t.ClientId,
                t => t.AgentId);
        }

        public ModelDependcyInfo[] GetDependcyRelation()
        {
            return new ModelDependcyInfo[]
            {
                new ModelDependcyInfo<ClientAgentRelation, ClientInfo>
                (
                    source => source.ClientId == null ? null : new ClientInfo(source.ClientId),
                    target => t => t.ClientId == target.Id
                ),
                new ModelDependcyInfo<ClientAgentRelation, AgentInfo>
                (
                    source => source.AgentId == null ? null : new AgentInfo(source.AgentId),
                    target => t => t.AgentId == target.Id
                )
            };
        }
    }
}
