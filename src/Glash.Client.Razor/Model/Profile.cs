using Quick.EntityFrameworkCore.Plus;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glash.Client.Razor.Model
{
    [Table($"{nameof(Glash)}_{nameof(Client)}_{nameof(Profile)}")]
    public class Profile : BaseModel
    {
        public enum Texts
        {
            ModelName,
            Name,
            ServerUrl,
            Account,
            Password
        }

        public Profile() { }
        public Profile(string id)
        {
            Id = id;
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ServerUrl { get; set; }
        [Required]
        public string Account { get; set; }
        [Required]
        public string Password { get; set; }

        public override string ToString()
        {
            return $"{Global.Instance.TextManager.GetText(Texts.ModelName)}[{Name}]";
        }
    }
}
