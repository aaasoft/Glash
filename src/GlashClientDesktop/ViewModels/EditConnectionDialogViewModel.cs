using Quick.Localize;

namespace GlashClientDesktop.ViewModels;

public class EditConnectionDialogViewModel : ViewModelBase
{
    public string Text_ConnectionName { get; } = Locale.GetString("Connection Name");
    public string Text_ServerUrl { get; } = Locale.GetString("Server Url");
    public string Text_User { get; } = Locale.GetString("User");
    public string Text_Password { get; } = Locale.GetString("Password");

    public Model.Connection Model { get; set; }
}
