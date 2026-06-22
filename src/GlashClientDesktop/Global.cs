using Quick.LiteDB.Plus;

namespace GlashClientDesktop
{
    public class Global
    {
        public static Global Instance { get; } = new Global();

        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model.Config>();
            modelBuilder.Entity<Model.Connection>();
        }
    }
}
