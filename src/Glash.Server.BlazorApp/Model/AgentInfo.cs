using Quick.EntityFrameworkCore.Plus;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Server.BlazorApp.Model
{
    [Table($"{nameof(Glash)}_{nameof(Server)}_{nameof(AgentInfo)}")]
    public class AgentInfo
    {
        public enum Texts
        {
            ModelName,
            Name,
            Password
        }

        public AgentInfo() { }
        public AgentInfo(string name) { Name = name; }

        [Key]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }

        public override int GetHashCode()
        {
            return this.GetHashCode(t => t.Name);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, t => t.Name);
        }
    }
}
