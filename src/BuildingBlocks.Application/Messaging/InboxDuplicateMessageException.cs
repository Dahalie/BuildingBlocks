namespace BuildingBlocks.Application.Messaging;

public sealed class InboxDuplicateMessageException(Guid messageId, string handlerType)
    : Exception($"Duplicate inbox message {messageId} for handler {handlerType}")
{
    public Guid MessageId { get; } = messageId;
    public string HandlerType { get; } = handlerType;
}
