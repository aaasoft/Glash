using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Quick.Blazor.Bootstrap;
using Quick.Localize;

namespace GlashClient.Pages
{
    public partial class Index : ComponentBase_WithGettextSupport
    {
        private static string TextTitle => Locale<Index>.GetString("Glash Client");
        private static string TextLoginPasswordManage => Locale<Index>.GetString("Login Password Manage");
        private static string TextProfileManage => Locale<Index>.GetString("Profile Manage");
        private static string TextPleaseInputPassword => Locale<Index>.GetString("Please input password");
        private static string TextLogin => Locale<Index>.GetString("Login");
        
        public bool IsLogin { get; private set; } = false;
        public string Message { get; private set; }
        private string CorrectPassword => Core.LoginPasswordManager.Instance.LoginPassword;

        [BindProperty]
        public string Password { get; set; }

        [Parameter]
        public RenderFragment Body { get; set; }

        private void Show<T>()
        {
            Body = Quick.Blazor.Bootstrap.Utils.BlazorUtils.GetRenderFragment<T>();
        }


#if DEBUG
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Password = CorrectPassword;
        }
#endif

        public void OnPost()
        {
            if (!IsLogin && CorrectPassword != Password)
            {
                Message = Locale<Index>.GetString("Password is wrong");
                return;
            }
            IsLogin = true;
            StateHasChanged();
        }

        private void onPasswordKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
                OnPost();
        }
    }
}
