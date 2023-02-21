// 在此系统中，除了Mqtt协议外，只提供客户端连接设备
// 如果一个设备具有多个控制器需要整合，那么请分开进行配置，并做好标记,最终查询时多个控制器考虑信息整合
// 以下为asp.net 6.0的写法，如果用5.0，请看Program.five.cs文件
using Autofac;
using Autofac.Extensions.DependencyInjection;
using PZIOT;
using PZIOT.Common;
using PZIOT.Common.LogHelper;
using PZIOT.Common.Seed;
using PZIOT.Extensions;
using PZIOT.Extensions.Apollo;
using PZIOT.Extensions.Middlewares;
using PZIOT.Filter;
using PZIOT.Hubs;
using PZIOT.IServices;
using PZIOT.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using PZIOT.Extensions.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

// 1、配置host与容器
builder.Host
.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacModuleRegister());
    builder.RegisterModule<AutofacPropertityModuleReg>();
})
.ConfigureLogging((hostingContext, builder) =>
{
    builder.AddFilter("System", LogLevel.Error);
    builder.AddFilter("Microsoft", LogLevel.Error);
    builder.SetMinimumLevel(LogLevel.Error);
    builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
})
.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.Sources.Clear();
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
    config.AddConfigurationApollo("appsettings.apollo.json");
});


// 2、配置服务
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
builder.Services.AddSingleton(new LogLock(builder.Environment.ContentRootPath));
builder.Services.AddUiFilesZipSetup(builder.Environment);

Permissions.IsUseIds4 = AppSettings.app(new string[] { "Startup", "IdentityServer4", "Enabled" }).ObjToBool();
RoutePrefix.Name = AppSettings.app(new string[] { "AppSettings", "SvcName" }).ObjToString();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
//加载工具或中间件
builder.Services.AddMemoryCacheSetup();
builder.Services.AddRedisCacheSetup();
builder.Services.AddSqlsugarSetup();
builder.Services.AddDbSetup();
builder.Services.AddAutoMapperSetup();
builder.Services.AddCorsSetup();
builder.Services.AddMiniProfilerSetup();
builder.Services.AddSwaggerSetup();
builder.Services.AddJobSetup();
builder.Services.AddHttpContextSetup();
builder.Services.AddAppTableConfigSetup(builder.Environment);
builder.Services.AddHttpApi();
builder.Services.AddRedisInitMqSetup();
builder.Services.AddRabbitMQSetup();
builder.Services.AddKafkaSetup(builder.Configuration);
builder.Services.AddEventBusSetup();
builder.Services.AddNacosSetup(builder.Configuration);

builder.Services.AddAuthorizationSetup();
builder.Services.AddMqttSetup();
builder.Services.AddIotSetup();
builder.Services.AddHostedService<PZIOTDataGatherServices>();
builder.Services.AddHostedService<PZIOTEquipmentSimpleStatusAnalysisServices>();
builder.Services.AddHostedService<PZIOTEquipmentNotSameDatasIntervalAnalysisSerivces>();
if (Permissions.IsUseIds4)
{
    builder.Services.AddAuthentication_Ids4Setup();
}
else
{
    builder.Services.AddAuthentication_JWTSetup();
}

builder.Services.AddIpPolicyRateLimitSetup(builder.Configuration);
builder.Services.AddSignalR().AddNewtonsoftJsonProtocol();
builder.Services.AddScoped<UseServiceDIAttribute>();
builder.Services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
        .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpPollySetup();
builder.Services.AddControllers(o =>
{
    o.Filters.Add(typeof(GlobalExceptionsFilter));
    //o.Conventions.Insert(0, new GlobalRouteAuthorizeConvention());
    o.Conventions.Insert(0, new GlobalRoutePrefixFilter(new RouteAttribute(RoutePrefix.Name)));
    //o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;//可解决接口不能为空
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
})
//.AddFluentValidation(config =>
//{
//    //程序集方式添加验证
//    config.RegisterValidatorsFromAssemblyContaining(typeof(UserRegisterVoValidator));
//    //是否与MvcValidation共存
//    config.DisableDataAnnotationsValidation = true;
//})
;

builder.Services.AddEndpointsApiExplorer();

builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


// 3、配置中间件
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    //app.UseHsts();

}

app.UseIpLimitMiddle();
app.UseRequestResponseLogMiddle();
app.UseRecordAccessLogsMiddle();
app.UseSignalRSendMiddle();
app.UseIpLogMiddle();
app.UseAllServicesMiddle(builder.Services);

app.UseSession();
app.UseSwaggerAuthorized();
app.UseSwaggerMiddle(() => Assembly.GetExecutingAssembly().GetManifestResourceStream("PZIOT.Api.index.html"));

app.UseCors(AppSettings.app(new string[] { "Startup", "Cors", "PolicyName" }));
DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(defaultFilesOptions);
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseStatusCodePages();
app.UseRouting();

if (builder.Configuration.GetValue<bool>("AppSettings:UseLoadTest"))
{
    app.UseMiddleware<ByPassAuthMiddleware>();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseMiniProfilerMiddleware();
//app.UseExceptionHandlerMidd();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapHub<ChatHub>("/api2/chatHub");
});


var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var myContext = scope.ServiceProvider.GetRequiredService<MyContext>();
var tasksQzServices = scope.ServiceProvider.GetRequiredService<ITasksQzServices>();
var schedulerCenter = scope.ServiceProvider.GetRequiredService<ISchedulerCenter>();
var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
app.UseSeedDataMiddle(myContext, builder.Environment.WebRootPath);
app.UseQuartzJobMiddleware(tasksQzServices, schedulerCenter);
app.UseConsulMiddle(builder.Configuration, lifetime);
app.ConfigureEventBus();
// 4、运行
app.Run();
