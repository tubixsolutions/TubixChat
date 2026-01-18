namespace TubixChat.BizLogicLayer.DTOs;

public class MessageDto
{
    public long Id { get; set; }
    public int SenderUserId { get; set; }
    public string SenderUserName { get; set; } = string.Empty;
    public string SenderFullName { get; set; } = string.Empty;
    public int RecieverUserId { get; set; }
    public string RecieverUserName { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsRead { get; set; }
    public bool IsDelivered { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsMine { get; set; } // Xabar o'zimnikimi?
}

public class SendMessageDto
{
    public int SenderUserId { get; set; }
    public int RecieverUserId { get; set; }
    public string MessageText { get; set; } = string.Empty;
}

public class ConversationDto
{
    public UserDto User { get; set; } = null!;
    public MessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}
