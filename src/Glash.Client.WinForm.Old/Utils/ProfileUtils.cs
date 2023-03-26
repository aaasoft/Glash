using Glash.Client.WinForm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.WinForm.Utils
{
    public static class ProfileUtils
    {
        public const string PROFILE_FILE_EXTENSION = ".gcp";

        public static string GetProfileFolder()
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                nameof(Glash),
                nameof(Client),
                "Profiles");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }

        public static string GetProfileFullPathFromProfileName(string profileName)
        {
            return Path.Combine(GetProfileFolder(), profileName);
        }

        public static ProfileModel GetDefaultProfile()
        {
            return new ProfileModel();
        }

        public static string GetDefaultProfileContent()
        {
            return GetDefaultProfile().ToString();
        }

        public static string[] GetProfileNames()
        {
            var di = new DirectoryInfo(GetProfileFolder());
            var files = di.GetFiles($"*{PROFILE_FILE_EXTENSION}");
            return files.Select(t => t.Name).ToArray();
        }
    }
}
