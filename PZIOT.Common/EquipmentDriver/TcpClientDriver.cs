using Grpc.Core;
using PZIOT.Model.PZIOTModels;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Common.EquipmentDriver
{
    public class TcpClientDriver : IEquipmentDriver<TcpClientConnectionModel>
    {
        private TcpClientConnectionModel tcpClientConnectionModel;
        private AsyncTcpSession client;
        private string currentData=string.Empty;
        private long currentCount = 0;
        private long preCount = 0;
        public async Task<bool> CreatConnect(TcpClientConnectionModel t)
        {
           bool result = await Task.Run(() =>
            {
                tcpClientConnectionModel = t;
                //建立Tcp链接
                client = new AsyncTcpSession();
                client.Connect(new IPEndPoint(IPAddress.Parse(tcpClientConnectionModel.Serverip), tcpClientConnectionModel.Port));
                // 连接断开事件
                client.Closed += client_Closed;
                // 收到服务器数据事件
                client.DataReceived += client_DataReceived;
                // 连接到服务器事件
                client.Connected += client_Connected;
                // 发生错误的处理
                client.Error += client_Error;
                return true;
            });
            
            return result;
        }
        void client_Error(object sender, ErrorEventArgs e)
        {

        }

        void client_Connected(object sender, EventArgs e)
        {
            ConsoleHelper.WriteSuccessLine($"{tcpClientConnectionModel.Serverip}连接成功!");
        }
        void client_DataReceived(object sender, DataEventArgs e)
        {
            //接收服务端的回复，不然就是等待
            currentData = Encoding.ASCII.GetString(e.Data);
            currentCount++ ;
        }

        void client_Closed(object sender, EventArgs e)
        {
            client.Close();
        }
        public Task<bool> DisConnect()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetConnectionState()
        {
            throw new NotImplementedException();
        }

        public  Task<List<EquipmentReadResponseProtocol>> ReadMultipleParasFromEquipment(List<string> readparas)
        {
            throw new NotImplementedException();
        }

        public async Task<EquipmentReadResponseProtocol> ReadSingleParaFromEquipment(string para)
        {
            EquipmentReadResponseProtocol result = await Task.Run(async () => {
                client.Send(Encoding.Default.GetBytes(para));
                bool flag = true;
                while (flag) {
                    if (preCount!=currentCount) {
                        Console.WriteLine("轮询等待返回中");
                        flag = false;
                        preCount = currentCount;
                    }
                    await Task.Delay(1000);
                } ;
                return new EquipmentReadResponseProtocol() { 
                     RequestPara =para,
                     ResponseValue= currentData
                };
            });
            return result;
        }
    }
}
