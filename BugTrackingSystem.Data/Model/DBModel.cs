namespace BugTrackingSystem.Data.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBModel : DbContext
    {
        public DBModel()
            : base("name=DBModel")
        {
        }

        public virtual DbSet<Bug> Bugs { get; set; }
        public virtual DbSet<BugAttachment> BugAttachments { get; set; }
        public virtual DbSet<BugPriority> BugPriorities { get; set; }
        public virtual DbSet<BugStatu> BugStatus { get; set; }
        public virtual DbSet<Filter> Filters { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bug>()
                .HasMany(e => e.BugAttachments)
                .WithRequired(e => e.Bug)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BugPriority>()
                .HasMany(e => e.Bugs)
                .WithRequired(e => e.BugPriority)
                .HasForeignKey(e => e.PriorityID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BugStatu>()
                .HasMany(e => e.Bugs)
                .WithRequired(e => e.BugStatu)
                .HasForeignKey(e => e.StatusID)
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
                .Map(m => m.ToTable("ProjectUser").MapLeftKey("ProjectID").MapRightKey("UserID"));

            modelBuilder.Entity<User>()
                .HasMany(e => e.Bugs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.AssignedUserID);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Filters)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRole>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.UserRole)
                .WillCascadeOnDelete(false);
        }
    }
}
