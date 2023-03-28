using Quick.EntityFrameworkCore.Plus;
using Quick.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.Razor.Model
{
    public class Config : BaseModel
    {
        [TextResource]
        public enum Texts
        {
            Name,
            ServerUrl,
            User,
            Password
        }

        public string Value { get; set; }

        public static string GetConfig(string id)
        {
            return ConfigDbContext.CacheContext.Find(new Config() { Id = id })?.Value;
        }

        public static void SetConfig(string id, string value)
        {
            var model = ConfigDbContext.CacheContext.Find(new Config() { Id = id });
            if (model == null)
            {
                ConfigDbContext.CacheContext.Add(new Config()
                {
                    Id = id,
                    Value = value
                });
            }
            else
            {
                model.Value = value;
                ConfigDbContext.CacheContext.Update(model);
            }
        }
    }
}
