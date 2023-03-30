using Quick.EntityFrameworkCore.Plus;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Server.BlazorApp.Model
{
    [Table($"{nameof(Glash)}_{nameof(Server)}_{nameof(ClientInfo)}")]
    public class ClientInfo : BaseModel
    {
        public enum Texts
        {
            ModelName,
            Name,
            Password
        }

        public ClientInfo() { }
        public ClientInfo(string id) { Id = id; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
