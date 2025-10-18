using SignalrMvc.Data;
using SignalrMvc.Models;
using SignalrMvc.Repositories;
using SignalrMvc.Repositories.Interfaces;

namespace SignalrMvc.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<ChatMessage>? _chatMessages;
        private IGenericRepository<User>? _users;
        private IGenericRepository<PrivateMessage>? _privateMessages;
        private IGenericRepository<UserConnection>? _userConnections;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<ChatMessage> ChatMessages
        {
            get
            {
                _chatMessages ??= new GenericRepository<ChatMessage>(_context);
                return _chatMessages;
            }
        }

        public IGenericRepository<User> Users
        {
            get
            {
                _users ??= new GenericRepository<User>(_context);
                return _users;
            }
        }

        public IGenericRepository<PrivateMessage> PrivateMessages
        {
            get
            {
                _privateMessages ??= new GenericRepository<PrivateMessage>(_context);
                return _privateMessages;
            }
        }

        public IGenericRepository<UserConnection> UserConnections
        {
            get
            {
                _userConnections ??= new GenericRepository<UserConnection>(_context);
                return _userConnections;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

