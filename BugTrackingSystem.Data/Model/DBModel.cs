using System.Data.Entity;

namespace BugTrackingSystem.Data.Model
{
    public partial class DBModel : DbContext
    {
        public DBModel()
            : base("name=DBModel")
        {
        }

        public virtual DbSet<BugAttachment> BugAttachments { get; set; }
        public virtual DbSet<BugPriority> BugPriorities { get; set; }
        public virtual DbSet<Bug> Bugs { get; set; }
        public virtual DbSet<BugStatus> BugStatuses { get; set; }
        public virtual DbSet<Filter> Filters { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bug>()
                .HasMany(e => e.BugAttachments)
                .WithRequired(e => e.Bug)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Filter>()
                .Property(e => e.Search)
                .IsUnicode(false);

            modelBuilder.Entity<Project>()
                .Property(e => e.Prefix)
                .IsFixedLength();

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Bugs)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Projects)
                .Map(m => m.ToTable("ProjectUsers").MapLeftKey("ProjectID").MapRightKey("UserID"));

            modelBuilder.Entity<User>()
                .HasMany(e => e.Bugs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.AssignedUserID);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Filters)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);
        }
    }
}
