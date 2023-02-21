using log4net;
using Microsoft.Extensions.Hosting;
using PZIOT.Common;
using SuperSocket;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using SuperSocket.Server;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PZIOT.Extensions.IOT
{
    /// <summary>
    /// 客制化项目，服务端
    /// </summary>
    public class RhBg102TcpServerServices : IHostedService, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IotService));
        private IHost host;
        public void Dispose()
        {
            this.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConsoleHelper.WriteSuccessLine("Rhbg102服务启动");
            host = SuperSocketHostBuilder.Create<TextPackageInfo, Cpt300LinePipelineFilter>().UsePackageHandler((s, p) =>
            {
                try
                {

                }
                catch (Exception ex)
                {
                    //p.test去掉尾 然后读出需要的信息，记录到数据库
                }
                return new ValueTask();
            }).UseSession<CptAppSession>().ConfigureSuperSocket(options =>//配置服务器如服务器名和监听端口等基本信息
                    {
                        options.Name = "Tcp";
                        options.ReceiveBufferSize = 2048;
                        options.Listeners = new List<ListenOptions>(){
                        new ListenOptions{
                         Ip="1988",
                         Port = 1988
                        }
                        };
                    }).Build();

            try
            {
                host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public class Cpt300LinePipelineFilter : TerminatorPipelineFilter<TextPackageInfo>
        {
            protected Encoding Encoding { get; private set; }

            public Cpt300LinePipelineFilter()
                : this(Encoding.ASCII)
            {

            }

            public Cpt300LinePipelineFilter(Encoding encoding)
                : base(new[] { (byte)'#', (byte)'#' })
            {
                Encoding = encoding;
            }

            protected override TextPackageInfo DecodePackage(ref ReadOnlySequence<byte> buffer)
            {
                return new TextPackageInfo { Text = buffer.GetString(Encoding) };
            }
        }

        public class CptAppSession : AppSession
        {

            protected override ValueTask OnSessionConnectedAsync()
            {

                //找到这个IP关联的驱动名
                var ip = this.RemoteEndPoint.ToString().Split(':')[0];
                return base.OnSessionConnectedAsync();
            }

            protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
            {
                try
                {
                    var ip = this.RemoteEndPoint.ToString().Split(':')[0];
                    return base.OnSessionClosedAsync(e);

                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return base.OnSessionClosedAsync(e);
                }

            }
        }
    }
}
