using Quick.Localize;

namespace GlashClientDesktop.ViewModels;

public class EditConnectionDialogViewModel : ViewModelBase
{
    public string Text_ConnectionName { get; } = Locale<EditConnectionDialogViewModel>.GetString("Connection Name");
    public string Text_ServerUrl { get; } = Locale<EditConnectionDialogViewModel>.GetString("Server Url");
    public string Text_User { get; } = Locale<EditConnectionDialogViewModel>.GetString("User");
    public string Text_Password { get; } = Locale<EditConnectionDialogViewModel>.GetString("Password");
    public string Text_PreferHighSpeedMode { get; } = Locale<EditConnectionDialogViewModel>.GetString("Prefer HighSpeed Mode");

    public Model.Connection Model { get; set; }
}
