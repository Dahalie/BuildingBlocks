namespace BuildingBlocks.Application.Messaging;

public interface IInboxStore
{
    Task<bool> ExistsAsync(Guid messageId, string handlerType, CancellationToken ct = default);
    Task RecordAsync(Guid messageId, string messageType, string handlerType, CancellationToken ct = default);
    Task MarkProcessedAsync(Guid messageId, string handlerType, CancellationToken ct = default);
}
