namespace PZIOT.EventBus.EventHandling
{
    public class EquipmentQueryIntegrationEvent : IntegrationEvent
    {
        public string BlogId { get; private set; }

        public EquipmentQueryIntegrationEvent(string blogid)
            => BlogId = blogid;
    }
}
