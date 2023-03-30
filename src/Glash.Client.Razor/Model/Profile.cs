using Quick.EntityFrameworkCore.Plus;
using Quick.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.Razor.Model
{
    [Table($"{nameof(Glash)}_{nameof(Client)}_{nameof(Profile)}")]
    public class Profile : BaseModel
    {
        [TextResource]
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
