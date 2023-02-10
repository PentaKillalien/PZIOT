﻿using Grpc.Core;
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
            ConsoleHelper.WriteWarningLine($"{tcpClientConnectionModel.Serverip}发生错误!");
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
        /// <summary>
        /// 被关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_Closed(object sender, EventArgs e)
        {
            client.Close();
        }
        public async Task<bool> DisConnect()
        {
            bool result = await Task.Run(() => {
                client.Close();
                return true;
            });

            return result;
        }

        public async Task<bool> GetConnectionState()
        {
            bool result =await Task.Run(() => {
                return client.IsConnected;
            });

            return result;
        }

        public async Task<List<EquipmentReadResponseProtocol>> RequestMultipleParasFromEquipment(List<string> readparas)
        {
            List<EquipmentReadResponseProtocol> listinfos = new List<EquipmentReadResponseProtocol>();
            foreach (string parasItem in readparas) {
                EquipmentReadResponseProtocol temp = await RequestSingleParaFromEquipment(parasItem);
                listinfos.Add(temp);
            }
            return listinfos;
        }

        public async Task<EquipmentReadResponseProtocol> RequestSingleParaFromEquipment(string para)
        {
            EquipmentReadResponseProtocol result = await Task.Run(async () => {
                client.Send(Encoding.Default.GetBytes(para));
                bool flag = false;
                for (int i = 0; i < tcpClientConnectionModel.TimeOut/10; i++)
                {
                    await Task.Delay(10);
                    Console.WriteLine("轮询等待返回中");
                    if (preCount != currentCount)
                    {
                        preCount = currentCount;
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    return new EquipmentReadResponseProtocol()
                    {
                        RequestPara = para,
                        ResponseValue = currentData
                    };
                }
                else {
                    return new EquipmentReadResponseProtocol()
                    {
                        RequestPara = para,
                        ResponseValue = "响应超时"
                    };
                }
                
            });
            return result;
        }
    }
}