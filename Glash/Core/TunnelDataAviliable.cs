using Quick.Protocol;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace G
{
    [JsonSerializable(typeof(D))]
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    internal partial class GDSerializerContext : JsonSerializerContext { }

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