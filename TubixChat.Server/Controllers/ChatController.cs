using Microsoft.AspNetCore.Mvc;
using TubixChat.BizLogicLayer.DTOs;
using TubixChat.BizLogicLayer.Services;

namespace TubixChat.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string searchText)
    {
        var users = await _chatService.SearchUsersAsync(searchText);
        return Ok(users);
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessageDto)
    {
        var message = await _chatService.SendMessageAsync(sendMessageDto);
        return Ok(message);
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetMessages([FromQuery] int currentUserId, [FromQuery] int otherUserId)
    {
        var messages = await _chatService.GetConversationMessagesAsync(currentUserId, otherUserId);
        return Ok(messages);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] int userId)
    {
        var conversations = await _chatService.GetUserConversationsAsync(userId);
        return Ok(conversations);
    }

    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkAsRead([FromQuery] int currentUserId, [FromQuery] int senderUserId)
    {
        await _chatService.MarkMessagesAsReadAsync(currentUserId, senderUserId);
        return Ok(new { success = true });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount([FromQuery] int userId, [FromQuery] int fromUserId)
    {
        var count = await _chatService.GetUnreadCountAsync(userId, fromUserId);
        return Ok(new { count });
    }
}
