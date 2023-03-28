using Quick.EntityFrameworkCore.Plus;
using Quick.EntityFrameworkCore.Plus.SQLite;
using Quick.Localize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Glash.Client
{
    public class Global
    {
        public static Global Instance { get; } = new Global();

        private const string LANGUAGE_RESOURCE_PREFIX = "Glash.Client.Razor.Language.";
        private string _Language;
        public string Language
        {
            get { return _Language; }
            set
            {
                _Language = value;
                TextManager = TextManager.GetInstance(value);
                Razor.Model.Config.SetConfig(nameof(Language), value);
                LanguageChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler LanguageChanged;
        public TextManager TextManager { get; private set; }
        public CultureInfo[] GetLanuages()
        {
            var list = new HashSet<string>();
            foreach (var t in typeof(Global).Assembly.GetManifestResourceNames())
            {
                if (!t.StartsWith(LANGUAGE_RESOURCE_PREFIX))
                    continue;
                var name = t.Substring(LANGUAGE_RESOURCE_PREFIX.Length);
                var index = name.IndexOf('.');
                if (index <= 0)
                    continue;
                name = name.Substring(0, index).Replace("_", "-");
                if (!list.Contains(name))
                    list.Add(name);
            }
            return list.Select(t => new CultureInfo(t)).ToArray();
        }

        public void Init()
        {
            ConfigDbContext.Init(new SQLiteDbContextConfigHandler(SQLiteDbContextConfigHandler.CONFIG_DB_FILE), modelBuilder =>
            {
                modelBuilder.Entity<Razor.Model.Config>();
                modelBuilder.Entity<Razor.Model.Profile>();
            });
            using (var dbContext = new ConfigDbContext())
                dbContext.EnsureDatabaseCreatedAndUpdated(t => Debug.Print(t));
            ConfigDbContext.CacheContext.LoadCache();

            _Language = Razor.Model.Config.GetConfig(nameof(Language));
            if (_Language == null)
                _Language = Thread.CurrentThread.CurrentCulture.IetfLanguageTag;
            TextManager = TextManager.GetInstance(Language);
        }
    }
}
