using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Quick.Localize;

namespace Glash.Blazor.Server
{
    public class Global
    {
        public static Global Instance { get; } = new Global();
        public TextManager TextManager { get; private set; }
        public string ConnectionPassword
        {
            get
            {
                var password = Model.Config.GetConfig(nameof(ConnectionPassword));
                if (string.IsNullOrEmpty(password))
                {
                    password = Guid.NewGuid().ToString("N");
                    Model.Config.SetConfig(nameof(ConnectionPassword), password);
                }
                return password;
            }
            set
            {
                Model.Config.SetConfig(nameof(ConnectionPassword), value);
                GlashServerMiddlewareExtensions.ServerOptions.Password = value;
            }
        }

        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model.Config>();
            modelBuilder.Entity<Model.AgentInfo>();
            modelBuilder.Entity<Model.ClientInfo>();
            modelBuilder.Entity<Model.ClientAgentRelation>()
                .HasKey(t => new { t.ClientName, t.AgentName });
        }

        public void Init()
        {
            Init(Thread.CurrentThread.CurrentCulture.IetfLanguageTag);
        }

        public void Init(string language)
        {
            TextManager = TextManager.GetInstance(language);
        }
    }
}
