namespace Aurora.Contracts.Chat;

public record ChatStreamEvent(
    string EventType,
    string? Content = null,
    string? Tool = null,
    string? ConversationId = null);
