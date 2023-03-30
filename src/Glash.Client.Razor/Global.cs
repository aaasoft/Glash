using Glash.Core.Client;
using Quick.Blazor.Bootstrap;
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
                Razor.Model.Config.SetConfig(nameof(Language), value);
                afterLanuageChanged();
                LanguageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler LanguageChanged;
        public TextManager TextManager { get; private set; }
        public GlashClient GlashClient { get; internal set; }

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

        public static string GetProfileFolder()
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                nameof(Glash),
                nameof(Client));
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }

        public void Init()
        {
            var dbFile = Path.Combine(GetProfileFolder(), SQLiteDbContextConfigHandler.CONFIG_DB_FILE);
            ConfigDbContext.Init(new SQLiteDbContextConfigHandler(dbFile), modelBuilder =>
            {
                modelBuilder.Entity<Razor.Model.Config>();
                modelBuilder.Entity<Razor.Model.Profile>();
                modelBuilder.Entity<Razor.Model.ProxyRule>();
            });
            using (var dbContext = new ConfigDbContext())
                dbContext.EnsureDatabaseCreatedAndUpdated(t => Debug.Print(t));
            ConfigDbContext.CacheContext.LoadCache();

            _Language = Razor.Model.Config.GetConfig(nameof(Language));
            if (_Language == null)
                _Language = Thread.CurrentThread.CurrentCulture.IetfLanguageTag;
            afterLanuageChanged();
        }

        private void afterLanuageChanged()
        {
            TextManager = TextManager.GetInstance(Language);
            ModalPrompt.TextOk = ModalAlert.TextOk = TextManager.GetText(Razor.ClientTexts.Ok);
            ModalPrompt.TextCancel = ModalAlert.TextCancel = TextManager.GetText(Razor.ClientTexts.Cancel);
        }
    }
}
