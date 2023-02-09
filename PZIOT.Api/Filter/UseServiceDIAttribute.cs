using PZIOT.IServices;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PZIOT.Filter
{
    public class UseServiceDIAttribute : ActionFilterAttribute
    {

        protected readonly ILogger<UseServiceDIAttribute> _logger;
        private readonly IEquipmentServices _equipmentServices;
        private readonly string _name;

        public UseServiceDIAttribute(ILogger<UseServiceDIAttribute> logger, IEquipmentServices equipmentServices,string Name="")
        {
            _logger = logger;
            _equipmentServices = equipmentServices;
            _name = Name;
        }


        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //var dd =await _blogArticleServices.Query();
            base.OnActionExecuted(context);
            DeleteSubscriptionFiles();
        }

        private void DeleteSubscriptionFiles()
        {
           
        }
    }
}
