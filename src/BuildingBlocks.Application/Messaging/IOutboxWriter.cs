using BuildingBlocks.Contracts.Messaging;

namespace BuildingBlocks.Application.Messaging;

public interface IOutboxWriter<TDbContext>
{
    void Stage(IIntegrationEvent integrationEvent);
}
