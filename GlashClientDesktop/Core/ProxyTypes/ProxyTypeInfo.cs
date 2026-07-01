namespace GlashClientDesktop.Core.ProxyTypes
{
    public class ProxyTypeInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        private Func<string, IProxyType> factory;

        public IProxyType CreateInstance(string config = null) => factory.Invoke(config);

        internal ProxyTypeInfo(string id, string name, Func<string, IProxyType> factory)
        {
            Id = id;
            Name = name;
            this.factory = factory;
        }
    }
}
