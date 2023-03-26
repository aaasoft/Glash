using Glash.Core.Client;
using Newtonsoft.Json;

namespace Glash.Client.WinForm.Model
{
    public class ProfileModel
    {
        public List<ServerInfo> ServerList { get; set; } = new List<ServerInfo>();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
