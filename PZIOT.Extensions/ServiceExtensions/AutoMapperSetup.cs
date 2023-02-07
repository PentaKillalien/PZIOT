using AutoMapper;
using PZIOT.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace PZIOT.Extensions
{
    /// <summary>
    /// Automapper 启动服务
    /// </summary>
    public static class AutoMapperSetup
    {
        //自动映射model 和viewmodel的转换，事先配置好每一个转换的属性
        public static void AddAutoMapperSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.RegisterMappings();
        }
    }
}
