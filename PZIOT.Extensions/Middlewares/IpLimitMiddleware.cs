using System;
using AspNetCoreRateLimit;
using PZIOT.Common;
using log4net;
using Microsoft.AspNetCore.Builder;

namespace PZIOT.Extensions.Middlewares
{
    /// <summary>
    /// ip 限流
    /// </summary>
    public static class IpLimitMiddleware
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IpLimitMiddleware));
        public static void UseIpLimitMiddle(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            try
            {
                if (AppSettings.app("Middleware", "IpRateLimit", "Enabled").ObjToBool())
                {
                    app.UseIpRateLimiting();
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error occured limiting ip rate.\n{e.Message}");
                throw;
            }
        }
    }
}
