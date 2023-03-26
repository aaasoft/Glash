using Glash.Client.WinForm.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.WinForm.Model
{
    public class ProfileInfo
    {
        public string Name { get; set; }
        public ProfileModel Model { get; set; }

        public void Save()
        {
            var fileName = ProfileUtils.GetProfileFullPathFromProfileName(Name);
            var fileContent = JsonConvert.SerializeObject(Model, Formatting.Indented);
            File.WriteAllText(fileName, fileContent);
        }

        public static ProfileInfo Load(string profileName)
        {
            var profile = new ProfileInfo();
            profile.Name = profileName;
            var fileName = ProfileUtils.GetProfileFullPathFromProfileName(profileName);
            var fileContent = File.ReadAllText(fileName);
            profile.Model = JsonConvert.DeserializeObject<ProfileModel>(fileContent);
            return profile;
        }
    }
}
