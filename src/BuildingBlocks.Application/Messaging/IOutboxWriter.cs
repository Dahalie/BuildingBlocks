namespace BuildingBlocks.Application.Messaging;

public interface IOutboxWriter
{
    void Stage(IIntegrationEvent integrationEvent);
}
