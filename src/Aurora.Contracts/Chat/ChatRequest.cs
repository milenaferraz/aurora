namespace Aurora.Contracts.Chat;

public record ChatRequest(string Message, string? ConversationId = null);
