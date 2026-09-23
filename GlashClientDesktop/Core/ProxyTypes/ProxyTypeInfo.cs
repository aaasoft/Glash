namespace GlashClientDesktop.Core.ProxyTypes
{
    public class ProxyTypeInfo
    {
        public string Id { get; set; }
        public Func<string> GetName { get; set; }
        public string Name => GetName();
        private Func<string, IProxyType> factory;

        public IProxyType CreateInstance(string config = null) => factory.Invoke(config);

        internal ProxyTypeInfo(string id, Func<string> getName, Func<string, IProxyType> factory)
        {
            Id = id;
            GetName = getName;
            this.factory = factory;
        }
    }
}
