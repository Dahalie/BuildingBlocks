using BuildingBlocks.Contracts.Messaging;

namespace BuildingBlocks.Application.Messaging;

public interface IOutboxWriter
{
    void Stage(IIntegrationEvent integrationEvent);
}
