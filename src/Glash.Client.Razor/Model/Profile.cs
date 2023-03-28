using Quick.EntityFrameworkCore.Plus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.Razor.Model
{
    public class Profile : BaseModel
    {
        public string Name { get; set; }
        public string ServerUrl { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
