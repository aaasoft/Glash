using Microsoft.AspNetCore.Components.Web;
using Quick.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.Razor
{
    public partial class Login
    {
        [TextResource]
        public enum Texts
        {
            Title,
            ChooseProfile,
            ServerUrl,
            User,
            Password,
            Login
        }

        private string Message;
        private string ServerUrl;
        private string User;
        private string Password;

        private void OnPost()
        {
            
        }
    }
}
