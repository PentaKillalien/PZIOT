﻿using PZIOT.IServices;
using Quartz;
using System.Threading.Tasks;

/// <summary>
/// 这里要注意下，命名空间和程序集是一样的，不然反射不到
/// </summary>
namespace PZIOT.Tasks
{
    public class Job_Blogs_Quartz : JobBase, IJob
    {
        private readonly IEquipmentServices _equipmentServices;

        public Job_Blogs_Quartz(IEquipmentServices equipmentServices, ITasksQzServices tasksQzServices)
        {
            _equipmentServices = equipmentServices;
            _tasksQzServices = tasksQzServices;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var executeLog = await ExecuteJob(context, async () => await Run(context));
        }
        public async Task Run(IJobExecutionContext context)
        {
            var list = await _equipmentServices.Query();
            // 也可以通过数据库配置，获取传递过来的参数
            JobDataMap data = context.JobDetail.JobDataMap;
            //int jobId = data.GetInt("JobParam");
        }
    }
}
