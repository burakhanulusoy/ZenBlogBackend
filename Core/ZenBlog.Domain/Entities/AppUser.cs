using Microsoft.AspNetCore.Identity;
using ZenBlog.Domain.Enums;   // YENÝ

namespace ZenBlog.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? LinkedlnUrl { get; set; }
        public string? FacebookUrl { get; set; }

        // YENÝ: HESAP DURUMU
        // Kullanýcý satýrý HÝÇ silinmiyor, sadece durumu deðiþiyor.
        // Böylece e-posta ve kullanýcý adý veritabanýnda kaldýðý için Identity
        // "bu e-posta / kullanýcý adý kullanýmda" diyerek tekrar kayýt olunmasýný kendiliðinden engelliyor.
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;

        // YENÝ: "Hesabýmý kapat" denildiði an. 30 gün bu tarihten itibaren sayýlýr.
        public DateTime? DeletionRequestedAt { get; set; }

        // YENÝ: "Son 3 gün" hatýrlatma maili gönderildi mi? (ayný maili iki kez atmamak için)
        public DateTime? DeletionReminderSentAt { get; set; }


        public virtual IList<Comment> Comments { get; set; }
        public virtual IList<SubComment> SubComments { get; set; }
        public virtual IList<Blog> Blogs { get; set; }

        // YENÝ: bu kullanýcýya uygulanan blokelerin geçmiþi
        public virtual IList<UserBlock> Blocks { get; set; }
    }
}