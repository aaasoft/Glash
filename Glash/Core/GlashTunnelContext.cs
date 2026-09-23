using System.Buffers;
using System.Buffers.Text;
using System.Text;
using System.Threading;
using Quick.Protocol;

namespace Glash.Core
{
    public class GlashTunnelContext : IDisposable
    {
        private CancellationTokenSource cts;
        private byte[] readBuffer = new byte[4 * 1024];
        private byte[] writeBuffer = new byte[8 * 1024];
        private QpChannel channel;
        private int tunnelId;
        private byte tunnelPackageType;
        private Stream stream;
        private Action<Exception> errorHandler;
        private int readLoopRunning; // 0=未运行 1=读泵运行中，防止 Start() 重入产生并发读泵

        // 流量统计：上行=从本地流读出发往远端（upload），下行=从远端收到写入本地流（download）
        private long _uploadBytes;
        private long _downloadBytes;
        public long UploadBytes => Interlocked.Read(ref _uploadBytes);
        public long DownloadBytes => Interlocked.Read(ref _downloadBytes);
        // 所属代理规则 Id（由调用方在创建时赋值；Agent 侧可不设置）
        public string RuleId { get; set; }

        public GlashTunnelContext(QpChannel channel, int tunnelId, byte tunnelPackageType, Stream stream, Action<Exception> errorHandler)
        {
            this.channel = channel;
            this.tunnelId = tunnelId;
            this.tunnelPackageType = tunnelPackageType;

            this.stream = stream;
            this.errorHandler = errorHandler;
            if (tunnelPackageType > 0)
                channel.RegisterPackageHandler(tunnelPackageType, tunnelPackageHandler);
        }

        private async ValueTask tunnelPackageHandler(QpChannel channel, byte packageType, ReadOnlySequence<byte> bodyBuffer)
        {
            try
            {
                // 下行：从远端收到的字节（写入本地流前先累计）
                Interlocked.Add(ref _downloadBytes, bodyBuffer.Length);
                var currentBuffer = bodyBuffer;
                while (currentBuffer.Length > 0)
                {
                    var ret = Math.Min(Convert.ToInt32(currentBuffer.Length), writeBuffer.Length);
                    currentBuffer.Slice(0, ret).CopyTo(writeBuffer);
                    currentBuffer = currentBuffer.Slice(ret);
                    stream?.Write(writeBuffer, 0, ret);
                }
                stream?.Flush();
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        private async Task beginRead(CancellationToken token)
        {
            try
            {
                var task = stream?.ReadAsync(readBuffer, 0, readBuffer.Length, token);
                if (task == null)
                    return;
                var ret = await task.ConfigureAwait(false);
                if (ret <= 0)
                    throw new IOException("Read count: " + ret);
                // 上行：本地流读出的字节即发往远端的数据量
                Interlocked.Add(ref _uploadBytes, ret);
                //如果支持通道包类型
                if (tunnelPackageType > 0)
                {
                    await channel.SendPackage(tunnelPackageType, async writer =>
                    {
                        readBuffer.AsSpan(0, ret).CopyTo(writer.GetSpan(ret));
                        writer.Advance(ret);
                        await writer.FlushAsync().ConfigureAwait(false);
                        return ret;
                    }).ConfigureAwait(false);
                }
                //否则使用传统模式
                else
                {
                    await channel.SendNoticePackage(new G.D()
                    {
                        TunnelId = tunnelId,
                        Data = Convert.ToBase64String(readBuffer, 0, ret)
                    }).ConfigureAwait(false);
                }
                _ = beginRead(token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally
            {
                Interlocked.Exchange(ref readLoopRunning, 0);
            }
        }

        public void PushData(string data)
        {
            var strBytesLength = Encoding.UTF8.GetByteCount(data);
            if (strBytesLength > writeBuffer.Length)
                writeBuffer = new byte[strBytesLength];
            strBytesLength = Encoding.UTF8.GetBytes(data, writeBuffer);
            var ret = Base64.DecodeFromUtf8InPlace(writeBuffer.AsSpan(0, strBytesLength), out var dataBytesLength);
            if (ret != OperationStatus.Done)
                throw new IOException($"Error when convert base64 string to byte array,reason: {ret}");
            try
            {
                stream?.Write(writeBuffer, 0, dataBytesLength);
                stream?.Flush();
                // 下行：Base64 模式写入本地流的字节
                Interlocked.Add(ref _downloadBytes, dataBytesLength);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public void OnError(Exception ex)
        {
            errorHandler?.Invoke(ex);
        }

        public void Start()
        {
            // 已有读泵在跑则忽略重复调用（如 StartTunnel 重发），避免两个循环共用
            // readBuffer/stream 与同一 channel 导致数据错乱
            if (Interlocked.CompareExchange(ref readLoopRunning, 1, 0) == 0)
            {
                cts?.Dispose();
                cts = new CancellationTokenSource();
                _ = beginRead(cts.Token);
            }
        }

        public void Dispose()
        {
            if (tunnelPackageType > 0)
                channel.UnregisterPackageHandler(tunnelPackageType);
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            try
            {
                stream?.Dispose();
                stream = null;
            }
            catch { }
        }
    }
}
