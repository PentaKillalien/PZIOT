using PZIOT.Common;
using PZIOT.EventBus.EventHandling;
using PZIOT.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PZIOT.EventBus
{
    public class EquipmentQueryIntegrationEventHandler : IIntegrationEventHandler<EquipmentQueryIntegrationEvent>
    {
        private readonly IEquipmentServices _equipmentServices;
        private readonly ILogger<EquipmentQueryIntegrationEventHandler> _logger;

        public EquipmentQueryIntegrationEventHandler(
            IEquipmentServices blogArticleServices,
            ILogger<EquipmentQueryIntegrationEventHandler> logger)
        {
            _equipmentServices = blogArticleServices;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(EquipmentQueryIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, "PZIOT", @event);

            ConsoleHelper.WriteSuccessLine($"----- Handling integration event: {@event.Id} at PZIOT - ({@event})");

            await _equipmentServices.QueryById(@event.BlogId.ToString());
        }

    }
}
