using Quick.Protocol;

namespace Glash.Core.Server
{
    public class GlashClientContext : IDisposable
    {
        public string Name { get; private set; }
        public QpChannel Channel { get; private set; }
        public DateTime CreateTime { get; private set; }

        public GlashClientContext(string name, QpChannel channel)
        {
            Name = name;
            Channel = channel;
            CreateTime = DateTime.Now;
        }

        public void Dispose()
        {
            Channel.Disconnect();
        }
    }
}
