using Quick.LiteDB.Plus;
using Quick.Localize;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Blazor.Client.Model
{
    [Table($"{nameof(Glash)}_{nameof(Client)}_{nameof(Connection)}")]
    public class Connection : BaseModel
    {
        public Connection() { }
        public Connection(string id)
        {
            Id = id;
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ServerUrl { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return Locale<Connection>.GetString("Connection") + $"[{Name}]";
        }
    }
}
