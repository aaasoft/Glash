using Quick.EntityFrameworkCore.Plus;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Server.BlazorApp.Model
{
    [Table($"{nameof(Glash)}_{nameof(Server)}_{nameof(AgentInfo)}")]
    public class AgentInfo : BaseModel
    {
        public enum Texts
        {
            ModelName,
            Name,
            Password
        }

        public AgentInfo() { }
        public AgentInfo(string id) { Id = id; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
