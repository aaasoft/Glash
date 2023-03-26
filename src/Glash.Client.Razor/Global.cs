using Quick.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client
{
    public class Global
    {
        public static TextManager TextManager { get; private set; } = TextManager.DefaultInstance;
        //public static TextManager TextManager { get; private set; } = TextManager.GetInstance("en-US");
    }
}
