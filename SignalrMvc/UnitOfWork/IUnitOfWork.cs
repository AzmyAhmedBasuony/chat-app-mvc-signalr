using SignalrMvc.Models;
using SignalrMvc.Repositories.Interfaces;

namespace SignalrMvc.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<ChatMessage> ChatMessages { get; }
        IGenericRepository<User> Users { get; }
        IGenericRepository<PrivateMessage> PrivateMessages { get; }
        IGenericRepository<UserConnection> UserConnections { get; }
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}

