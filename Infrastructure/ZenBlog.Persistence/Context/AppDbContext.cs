using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZenBlog.Domain.Entities;
using ZenBlog.Domain.Entities.Common;

namespace ZenBlog.Persistence.Context
{
    public class AppDbContext(DbContextOptions options):DbContext(options)
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


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





        public DbSet<About>  Abouts { get; set; }
        public DbSet<Blog>  Blogs { get; set; }
        public DbSet<Category>  Categories { get; set; }
        public DbSet<Contact>  Contacts { get; set; }
        public DbSet<ContactMessage>  ContactMessages { get; set; }
        public DbSet<SocialMedia>  SocialMedias { get; set; }
        public DbSet<Team>  Teams { get; set; }
        public DbSet<TeamSocialMedia>  TeamSocialMedias { get; set; }



    }
}
