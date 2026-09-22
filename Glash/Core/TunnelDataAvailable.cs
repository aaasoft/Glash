using Quick.Protocol;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

//命令空间G是Glash的缩写，这里使用缩写是为了最大程序减小代理数据类名(G.D)的长度以节省网络流量，提升传输效率
namespace G
{
    [JsonSerializable(typeof(D))]
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    internal partial class GDSerializerContext : JsonSerializerContext { }

    /// <summary>
    /// 类名D是Data的缩写，这里使用缩写是为了最大程序减小代理数据类名(G.D)的长度以节省网络流量，提升传输效率
    /// </summary>
    public class D : AbstractQpSerializer<D>
    {
        protected override JsonTypeInfo<D> GetTypeInfo() => GDSerializerContext.Default.D;

        public int TunnelId { get; set; }
        /// <summary>
        /// Base64编码的字节数组
        /// </summary>
        public string Data { get; set; }
    }
}