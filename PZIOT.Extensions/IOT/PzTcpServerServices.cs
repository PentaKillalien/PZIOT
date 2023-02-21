using log4net;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PZIOT.Common;
using PZIOT.Tasks.Trigger;
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
    public class PzTcpServerServices : IHostedService, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IotService));
        private IHost host;
        public void Dispose()
        {
            host.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //List
            var triggerData = new PzTcpTriggerData();
            triggerData.ValueChanged += async (sender, e) =>
            {
                Console.WriteLine($"数据项编号{e.EquipmentIp}数据发生变化,oldValue>{e.OldValue}=>newvalue>{e.NewValue}");
                //post
            };
            
            ConsoleHelper.WriteSuccessLine("PzTcpServer服务已启动...");
            host = SuperSocketHostBuilder.Create<TextPackageInfo, DoublePoundSignPipelineFilter>().UsePackageHandler((s, p) =>
            {
                try
                {
                    ConsoleHelper.WriteInfoLine($"pzTcp Recive:{p.Text}");
                    JObject jo = (JObject)JsonConvert.DeserializeObject(p.Text); //转换为Json对象
                    int input1 = 0;
                    var input1Result = int.TryParse(jo["UlKeyPushCount"][0].ToString(), out input1);
                    int input2 = 0;
                    var input2Result = int.TryParse(jo["UlKeyPushCount"][1].ToString(), out input2);
                    int input3 = 0;
                    var input3Result = int.TryParse(jo["UlKeyPushCount"][2].ToString(), out input3);
                    int input4 = 0;
                    var input4Result = int.TryParse(jo["UlKeyPushCount"][3].ToString(), out input4);
                    string statu = jo["ProductionState"].ToString();
                    //+1判断就要上传
                    
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteErrorLine($"pzTcp Recive failed {ex.Message}");
                }
                return new ValueTask();
            }).UseSession<CptAppSession>().ConfigureSuperSocket(options =>//配置服务器如服务器名和监听端口等基本信息
                    {
                        options.Name = "PziotTcpServer";
                        options.ReceiveBufferSize = 2048;
                        options.Listeners = new List<ListenOptions>(){
                        new ListenOptions{
                         Ip="Any",
                         Port = 1988
                        }
                        };
                    }).Build();

            try
            {
                host.RunAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            host.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 双##号结尾协议
        /// </summary>
        private class DoublePoundSignPipelineFilter : TerminatorPipelineFilter<TextPackageInfo>
        {
            protected Encoding Encoding { get; private set; }

            public DoublePoundSignPipelineFilter()
                : this(Encoding.ASCII)
            {

            }

            public DoublePoundSignPipelineFilter(Encoding encoding)
                : base(new[] { (byte)'#', (byte)'#' })
            {
                Encoding = encoding;
            }

            protected override TextPackageInfo DecodePackage(ref ReadOnlySequence<byte> buffer)
            {
                return new TextPackageInfo { Text = buffer.GetString(Encoding) };
            }
        }

        private class CptAppSession : AppSession
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
