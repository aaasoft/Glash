using Microsoft.EntityFrameworkCore;
using Quick.Localize;

namespace Glash.Blazor.Agent
{
    public class Global
    {
        public static Global Instance { get; } = new Global();
        public TextManager TextManager { get; private set; }

        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model.Profile>();
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
