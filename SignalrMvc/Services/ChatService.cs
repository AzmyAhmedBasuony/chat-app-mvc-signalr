using Microsoft.EntityFrameworkCore;
using SignalrMvc.DTOs;
using SignalrMvc.Models;
using SignalrMvc.UnitOfWork;

namespace SignalrMvc.Services
{
    public interface IChatService
    {
        Task<List<UserDto>> GetAllUsersAsync(int currentUserId);
        Task<ChatHistoryDto> GetChatHistoryAsync(int currentUserId, int otherUserId);
        Task<MessageDto> SendMessageAsync(int senderId, SendMessageDto dto);
        Task<bool> MarkMessagesAsReadAsync(int currentUserId, int senderId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<Dictionary<int, int>> GetUnreadCountsPerUserAsync(int currentUserId);
    }

    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(int currentUserId)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.Id != currentUserId);
            
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                ProfileImage = u.ProfileImage,
                IsOnline = u.IsOnline,
                LastSeen = u.LastSeen
            }).OrderByDescending(u => u.IsOnline).ThenBy(u => u.Username).ToList();
        }

        public async Task<ChatHistoryDto> GetChatHistoryAsync(int currentUserId, int otherUserId)
        {
            // Get other user info
            var otherUser = await _unitOfWork.Users.GetByIdAsync(otherUserId);
            if (otherUser == null)
                throw new InvalidOperationException("User not found");

            // Get all messages between the two users
            var messages = await _unitOfWork.PrivateMessages.FindAsync(m =>
                (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == currentUserId));

            var orderedMessages = messages.OrderBy(m => m.SentAt).ToList();

            var messageDtos = new List<MessageDto>();
            foreach (var msg in orderedMessages)
            {
                var sender = msg.SenderId == currentUserId ? 
                    await _unitOfWork.Users.GetByIdAsync(currentUserId) : otherUser;
                var receiver = msg.ReceiverId == currentUserId ? 
                    await _unitOfWork.Users.GetByIdAsync(currentUserId) : otherUser;

                messageDtos.Add(new MessageDto
                {
                    Id = msg.Id,
                    SenderId = msg.SenderId,
                    SenderUsername = sender?.Username ?? "",
                    SenderFullName = sender?.FullName,
                    SenderProfileImage = sender?.ProfileImage,
                    ReceiverId = msg.ReceiverId,
                    ReceiverUsername = receiver?.Username ?? "",
                    ReceiverFullName = receiver?.FullName,
                    ReceiverProfileImage = receiver?.ProfileImage,
                    Message = msg.Message,
                    IsRead = msg.IsRead,
                    SentAt = msg.SentAt,
                    ReadAt = msg.ReadAt
                });
            }

            var unreadCount = orderedMessages.Count(m => m.ReceiverId == currentUserId && !m.IsRead);

            return new ChatHistoryDto
            {
                OtherUser = new UserDto
                {
                    Id = otherUser.Id,
                    Username = otherUser.Username,
                    FullName = otherUser.FullName,
                    ProfileImage = otherUser.ProfileImage,
                    IsOnline = otherUser.IsOnline,
                    LastSeen = otherUser.LastSeen
                },
                Messages = messageDtos,
                UnreadCount = unreadCount
            };
        }

        public async Task<MessageDto> SendMessageAsync(int senderId, SendMessageDto dto)
        {
            var sender = await _unitOfWork.Users.GetByIdAsync(senderId);
            var receiver = await _unitOfWork.Users.GetByIdAsync(dto.ReceiverId);

            if (sender == null || receiver == null)
                throw new InvalidOperationException("User not found");

            var message = new PrivateMessage
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Message = dto.Message,
                SentAt = DateTime.Now,
                IsRead = false
            };

            await _unitOfWork.PrivateMessages.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return new MessageDto
            {
                Id = message.Id,
                SenderId = sender.Id,
                SenderUsername = sender.Username,
                SenderFullName = sender.FullName,
                SenderProfileImage = sender.ProfileImage,
                ReceiverId = receiver.Id,
                ReceiverUsername = receiver.Username,
                ReceiverFullName = receiver.FullName,
                ReceiverProfileImage = receiver.ProfileImage,
                Message = message.Message,
                IsRead = message.IsRead,
                SentAt = message.SentAt,
                ReadAt = message.ReadAt
            };
        }

        public async Task<bool> MarkMessagesAsReadAsync(int currentUserId, int senderId)
        {
            var unreadMessages = await _unitOfWork.PrivateMessages.FindAsync(m =>
                m.SenderId == senderId && m.ReceiverId == currentUserId && !m.IsRead);

            if (!unreadMessages.Any())
                return false;

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.Now;
                _unitOfWork.PrivateMessages.Update(message);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _unitOfWork.PrivateMessages.CountAsync(m =>
                m.ReceiverId == userId && !m.IsRead);
        }

        public async Task<Dictionary<int, int>> GetUnreadCountsPerUserAsync(int currentUserId)
        {
            var unreadMessages = await _unitOfWork.PrivateMessages.FindAsync(m =>
                m.ReceiverId == currentUserId && !m.IsRead);

            return unreadMessages
                .GroupBy(m => m.SenderId)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
