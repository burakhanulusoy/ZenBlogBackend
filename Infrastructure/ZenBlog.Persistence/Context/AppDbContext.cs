using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Persistence.Context
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // YENÝ: UserBlock ? AppUser'a ÜÇ ayrý iliþki var (bloke edilen, bloke eden admin, kaldýran admin).
            // EF hangisinin hangisi olduðunu kendisi çözemez; burada tek tek söylüyoruz.
            // Restrict: kullanýcý satýrý silinmeye kalkýlýrsa bloke geçmiþi yüzünden engellensin
            // (zaten kullanýcýlarý silmiyoruz; geçmiþ hiç kaybolmasýn diye güvence).
            modelBuilder.Entity<UserBlock>(entity =>
            {
                entity.HasOne(b => b.User)
                      .WithMany(u => u.Blocks)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.BlockedByAdmin)
                      .WithMany()
                      .HasForeignKey(b => b.BlockedByAdminId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.UnblockedByAdmin)
                      .WithMany()
                      .HasForeignKey(b => b.UnblockedByAdminId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(b => b.Reason).HasMaxLength(1000).IsRequired();
                entity.Property(b => b.EvidenceUrl).HasMaxLength(500).IsRequired();
                entity.Property(b => b.UnblockReason).HasMaxLength(1000);


                // YENÝ: RoleChangeLog ? AppUser'a iki iliþki (rolü deðiþen, deðiþtiren admin)
                modelBuilder.Entity<RoleChangeLog>(entity =>
                {
                    entity.HasOne(r => r.User)
                          .WithMany()
                          .HasForeignKey(r => r.UserId)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(r => r.ChangedByAdmin)
                          .WithMany()
                          .HasForeignKey(r => r.ChangedByAdminId)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.Property(r => r.RoleName).HasMaxLength(256).IsRequired();
                });

                // YENÝ: WriterApplication ? AppUser'a iki iliþki (baþvuran, karar veren admin)
                modelBuilder.Entity<WriterApplication>(entity =>
                {
                    entity.HasOne(a => a.User)
                          .WithMany()
                          .HasForeignKey(a => a.UserId)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(a => a.DecidedByAdmin)
                          .WithMany()
                          .HasForeignKey(a => a.DecidedByAdminId)
                          .OnDelete(DeleteBehavior.Restrict);

                    entity.Property(a => a.Topics).HasMaxLength(500).IsRequired();
                    entity.Property(a => a.Motivation).HasMaxLength(2000).IsRequired();
                    entity.Property(a => a.Experience).HasMaxLength(1000);
                    entity.Property(a => a.SampleLinks).HasMaxLength(1000);
                    entity.Property(a => a.RejectReason).HasMaxLength(1000);
                });



            });

            foreach (var entitiyType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entitiyType.ClrType))
                {
                    //wrapper yapýyoruz, yani sorgulara otomatik olarak silinmiþ olanlarý getirme filtresi ekliyoruz
                    //select * from table where IsDeleted = false olanlar gelsin sadece diyeceðiz
                    modelBuilder.Entity(entitiyType.ClrType)
                                .HasQueryFilter(ConvertToDeleteFilter(entitiyType.ClrType));
                }
            }
        }

        private static LambdaExpression ConvertToDeleteFilter(Type type)
        {
            var parameter = Expression.Parameter(type, "e");
            var property = Expression.Property(Expression.Convert(parameter, typeof(BaseEntity)), "IsDeleted");
            var notDeleted = Expression.Not(property);
            return Expression.Lambda(notDeleted, parameter);
            //ýsdeleted false olanalr gelcek soft delete için
        }

        public DbSet<About> Abouts { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<SocialMedia> SocialMedias { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamSocialMedia> TeamSocialMedias { get; set; }
        public DbSet<SubComment> SubComments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }
        public DbSet<RoleChangeLog> RoleChangeLogs { get; set; }   

        public DbSet<WriterApplication> WriterApplications { get; set; }   
    }
}