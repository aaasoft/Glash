using Microsoft.EntityFrameworkCore;
using Quick.EntityFrameworkCore.Plus;
using Quick.EntityFrameworkCore.Plus.SQLite;
using Quick.Localize;
using System.Diagnostics;

namespace Glash.Server.BlazorApp
{
    public class Global
    {
        public static Global Instance { get; } = new Global();
        public TextManager TextManager { get; private set; }

        public void Init()
        {
            var dbFile = SQLiteDbContextConfigHandler.CONFIG_DB_FILE;
            Init(Thread.CurrentThread.CurrentCulture.IetfLanguageTag, dbFile, null);
        }

        public void Init(string language, string dbFile, Action<ModelBuilder> modelBuilderHandler)
        {
            TextManager = TextManager.GetInstance(language);
            ConfigDbContext.Init(new SQLiteDbContextConfigHandler(dbFile), modelBuilder =>
            {
                modelBuilder.Entity<Model.Config>();
                modelBuilder.Entity<Model.AgentInfo>();
                modelBuilder.Entity<Model.ClientInfo>();
                modelBuilder.Entity<Model.ClientAgentRelation>()
                    .HasKey(t => new { t.ClientId, t.AgentId });
                modelBuilderHandler?.Invoke(modelBuilder);
            });
            using (var dbContext = new ConfigDbContext())
                dbContext.EnsureDatabaseCreatedAndUpdated(t => Debug.Print(t));
            ConfigDbContext.CacheContext.LoadCache();
        }
    }
}
