using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using SignalrMvc.DTOs;
using SignalrMvc.Models;
using SignalrMvc.Services;
using SignalrMvc.UnitOfWork;

namespace SignalrMvc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatService _chatService;
        private static readonly Dictionary<int, List<string>> _userConnections = new();
        private static int _anonymousUserCount = 0;
        private static readonly object _lock = new object();

        public ChatHub(IUnitOfWork unitOfWork, IChatService chatService)
        {
            _unitOfWork = unitOfWork;
            _chatService = chatService;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return null;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = GetCurrentUserId();

                if (userId.HasValue)
                {
                    // Authenticated user
                    var connectionId = Context.ConnectionId;

                    lock (_lock)
                    {
                        if (!_userConnections.ContainsKey(userId.Value))
                        {
                            _userConnections[userId.Value] = new List<string>();
                        }
                        _userConnections[userId.Value].Add(connectionId);
                    }

                    // Save connection to database
                    await _unitOfWork.UserConnections.AddAsync(new UserConnection
                    {
                        UserId = userId.Value,
                        ConnectionId = connectionId,
                        ConnectedAt = DateTime.Now
                    });

                    // Update user online status
                    var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
                    if (user != null)
                    {
                        user.IsOnline = true;
                        user.LastSeen = DateTime.Now;
                        _unitOfWork.Users.Update(user);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    // Notify all clients about user status change
                    await Clients.All.SendAsync("UserStatusChanged", userId.Value, true);
                    
                    // Send authenticated user count
                    var onlineCount = _userConnections.Count;
                    await Clients.All.SendAsync("UpdateUserCount", onlineCount);
                }
                else
                {
                    // Anonymous user (for group chat)
                    lock (_lock)
                    {
                        _anonymousUserCount++;
                    }
                    await Clients.All.SendAsync("UpdateUserCount", _anonymousUserCount);
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnConnectedAsync: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = GetCurrentUserId();
                var connectionId = Context.ConnectionId;

                if (userId.HasValue)
                {
                    // Authenticated user
                    lock (_lock)
                    {
                        if (_userConnections.ContainsKey(userId.Value))
                        {
                            _userConnections[userId.Value].Remove(connectionId);
                            
                            if (_userConnections[userId.Value].Count == 0)
                            {
                                _userConnections.Remove(userId.Value);
                            }
                        }
                    }

                    // Remove connection from database
                    var connection = await _unitOfWork.UserConnections.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
                    if (connection != null)
                    {
                        _unitOfWork.UserConnections.Remove(connection);
                    }

                    // Update user online status if they have no more connections
                    if (!_userConnections.ContainsKey(userId.Value))
                    {
                        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
                        if (user != null)
                        {
                            user.IsOnline = false;
                            user.LastSeen = DateTime.Now;
                            _unitOfWork.Users.Update(user);
                        }

                        await Clients.All.SendAsync("UserStatusChanged", userId.Value, false);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    var onlineCount = _userConnections.Count;
                    await Clients.All.SendAsync("UpdateUserCount", onlineCount);
                }
                else
                {
                    // Anonymous user
                    lock (_lock)
                    {
                        _anonymousUserCount--;
                        if (_anonymousUserCount < 0) _anonymousUserCount = 0;
                    }
                    await Clients.All.SendAsync("UpdateUserCount", _anonymousUserCount);
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnDisconnectedAsync: {ex.Message}");
            }
        }

        // Private messaging methods (require authentication)
        [Authorize]
        public async Task SendPrivateMessage(int receiverId, string message)
        {
            try
            {
                var senderId = GetCurrentUserId();
                if (!senderId.HasValue)
                {
                    await Clients.Caller.SendAsync("Error", "Authentication required");
                    return;
                }

                var messageDto = await _chatService.SendMessageAsync(senderId.Value, new SendMessageDto
                {
                    ReceiverId = receiverId,
                    Message = message
                });

                if (_userConnections.ContainsKey(receiverId))
                {
                    var receiverConnections = _userConnections[receiverId];
                    await Clients.Clients(receiverConnections).SendAsync("ReceivePrivateMessage", messageDto);
                }

                await Clients.Caller.SendAsync("MessageSent", messageDto);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Failed to send message: {ex.Message}");
            }
        }

        [Authorize]
        public async Task MarkMessagesAsRead(int senderId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue) return;

                await _chatService.MarkMessagesAsReadAsync(currentUserId.Value, senderId);

                if (_userConnections.ContainsKey(senderId))
                {
                    var senderConnections = _userConnections[senderId];
                    await Clients.Clients(senderConnections).SendAsync("MessagesRead", currentUserId.Value);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Failed to mark messages as read: {ex.Message}");
            }
        }

        [Authorize]
        public async Task SendTypingIndicator(int receiverId, bool isTyping)
        {
            try
            {
                var senderId = GetCurrentUserId();
                if (!senderId.HasValue) return;

                if (_userConnections.ContainsKey(receiverId))
                {
                    var receiverConnections = _userConnections[receiverId];
                    await Clients.Clients(receiverConnections).SendAsync("UserTyping", senderId.Value, isTyping);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendTypingIndicator: {ex.Message}");
            }
        }

        // Group chat methods (allow anonymous)
        [AllowAnonymous]
        public async Task SendMessage(string name, string message)
        {
            try
            {
                // Save message to database
                var chatMessage = new ChatMessage
                {
                    SenderName = name,
                    Message = message,
                    SentAt = DateTime.Now,
                    ConnectionId = Context.ConnectionId
                };

                await _unitOfWork.ChatMessages.AddAsync(chatMessage);
                await _unitOfWork.SaveChangesAsync();

                // Send message to all clients
                await Clients.All.SendAsync("NewMessage", name, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving message: {ex.Message}");
                throw;
            }
        }

        [AllowAnonymous]
        public async Task<IEnumerable<ChatMessage>> GetChatHistory(int count = 50)
        {
            try
            {
                var messages = await _unitOfWork.ChatMessages.GetAllAsync();
                return messages.OrderByDescending(m => m.SentAt).Take(count).OrderBy(m => m.SentAt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving chat history: {ex.Message}");
                return Enumerable.Empty<ChatMessage>();
            }
        }
    }
}


