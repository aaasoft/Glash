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
            [Text("en-US", "Glash Client")]
            [Text("zh-CN", "Glash客户端")]
            Title,
            [Text("en-US", "Choose profile...")]
            [Text("zh-CN", "请选择配置...")]
            ChooseProfile,
            [Text("en-US", "Server url")]
            [Text("zh-CN", "服务端地址")]
            ServerUrl,
            [Text("en-US", "User")]
            [Text("zh-CN", "用户")]
            User,
            [Text("en-US", "Password")]
            [Text("zh-CN", "密码")]
            Password,
            [Text("en-US", "Login")]
            [Text("zh-CN", "登录")]
            Login
        }

        private string Title;
        private string Message;
        private string ServerUrl;
        private string User;
        private string Password;

        public Login()
        {            
            Title = Global.TextManager.GetText(Texts.Title);
        }

        private void OnPost()
        {
            
        }
    }
}
