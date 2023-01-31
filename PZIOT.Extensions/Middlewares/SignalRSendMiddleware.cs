﻿using System.Threading.Tasks;
using PZIOT.Common;
using PZIOT.Common.LogHelper;
using PZIOT.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace PZIOT.Extensions.Middlewares
{
    /// <summary>
    /// 中间件
    /// SignalR发送数据
    /// </summary>
    public class SignalRSendMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;
        private readonly IHubContext<ChatHub> _hubContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="hubContext"></param>
        public SignalRSendMiddleware(RequestDelegate next, IHubContext<ChatHub> hubContext)
        {
            _next = next;
            _hubContext = hubContext;
        }



        public async Task InvokeAsync(HttpContext context)
        {
            if (AppSettings.app("Middleware", "SignalR", "Enabled").ObjToBool())
            {
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate", LogLock.GetLogData()); 
            }
            await _next(context);
        }

    }
}

