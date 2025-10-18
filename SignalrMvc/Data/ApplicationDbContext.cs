using Microsoft.EntityFrameworkCore;
using SignalrMvc.Models;

namespace SignalrMvc.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ChatMessage
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SenderName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.SentAt).IsRequired();
                entity.Property(e => e.ConnectionId).HasMaxLength(50);

                // Create index for better query performance
                entity.HasIndex(e => e.SentAt).HasDatabaseName("IX_ChatMessages_SentAt");
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.ProfileImage).HasMaxLength(255);
                entity.Property(e => e.IsOnline).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.IsOnline);
                entity.HasIndex(e => e.LastSeen);
            });

            // Configure PrivateMessage
            modelBuilder.Entity<PrivateMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.IsRead).HasDefaultValue(false);
                entity.Property(e => e.SentAt).HasDefaultValueSql("GETDATE()");

                // Relationships
                entity.HasOne(e => e.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(e => e.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => new { e.SenderId, e.ReceiverId, e.SentAt });
                entity.HasIndex(e => new { e.ReceiverId, e.IsRead });
                entity.HasIndex(e => e.SentAt);
            });

            // Configure UserConnection
            modelBuilder.Entity<UserConnection>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ConnectionId).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.ConnectionId).IsUnique();
                entity.Property(e => e.ConnectedAt).HasDefaultValueSql("GETDATE()");

                // Relationship
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Connections)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
            });

            // Seed data - 5 demo users
            SeedUsers(modelBuilder);
        }

        private void SeedUsers(ModelBuilder modelBuilder)
        {
            // Password for all users: "Password123" (hashed)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "alice",
                    PasswordHash = passwordHash,
                    FullName = "Alice Johnson",
                    Email = "alice@chat.com",
                    ProfileImage = "https://ui-avatars.com/api/?name=Alice+Johnson&background=667eea&color=fff",
                    IsOnline = false,
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    Username = "bob",
                    PasswordHash = passwordHash,
                    FullName = "Bob Smith",
                    Email = "bob@chat.com",
                    ProfileImage = "https://ui-avatars.com/api/?name=Bob+Smith&background=764ba2&color=fff",
                    IsOnline = false,
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Id = 3,
                    Username = "charlie",
                    PasswordHash = passwordHash,
                    FullName = "Charlie Brown",
                    Email = "charlie@chat.com",
                    ProfileImage = "https://ui-avatars.com/api/?name=Charlie+Brown&background=f093fb&color=fff",
                    IsOnline = false,
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Id = 4,
                    Username = "diana",
                    PasswordHash = passwordHash,
                    FullName = "Diana Prince",
                    Email = "diana@chat.com",
                    ProfileImage = "https://ui-avatars.com/api/?name=Diana+Prince&background=4facfe&color=fff",
                    IsOnline = false,
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Id = 5,
                    Username = "eve",
                    PasswordHash = passwordHash,
                    FullName = "Eve Davis",
                    Email = "eve@chat.com",
                    ProfileImage = "https://ui-avatars.com/api/?name=Eve+Davis&background=43e97b&color=fff",
                    IsOnline = false,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}

