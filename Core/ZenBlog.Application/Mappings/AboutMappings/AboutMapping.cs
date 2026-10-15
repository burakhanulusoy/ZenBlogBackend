using AutoMapper;
using ZenBlog.Application.Features.Commands.AboutCommands;
using ZenBlog.Application.Features.Results.AboutResults;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Mappings.AboutMappings;

// AutoMapper'a "hangi tipten hangi tipe nasýl kopyalama yapýlacaðýný" anlatan talimat listesi.
// AutoMapper alanlarý ÝSÝMLERÝNE bakarak eþler: kaynakta ve hedefte ayný isimli alan varsa
// deðeri kendisi kopyalar. Ayný isimli ama farklý iþleme ihtiyaç duyan alanlar için
// ForMember ile özel talimat veririz.
public class AboutMapping : Profile
{
    public AboutMapping()
    {
        // ---------------------------------------------------------------
        // OKUMA: Veritabaný (About) ? Kullanýcýya dönen cevap (Result)
        // ---------------------------------------------------------------
        // Kullanýldýðý yer: GetAboutsQueryHandler, GetAboutByIdQueryHandler
        // Bütün alanlarýn adý ve tipi ayný (resimler iki tarafta da string/Azure adresi),
        // bu yüzden ek ayar gerekmiyor; AutoMapper hepsini kendisi eþliyor.
        CreateMap<About, GetAboutsQueryResult>();
        CreateMap<About, GetAboutByIdQueryResult>();

        // ---------------------------------------------------------------
        // OLUÞTURMA: Kullanýcýnýn isteði (CreateAboutCommand) ? YENÝ About
        // ---------------------------------------------------------------
        // Kullanýldýðý yer: CreateAboutCommandHandler ? _mapper.Map<About>(request)
        //
        // Sorun: Command'daki Image1 kullanýcýnýn bilgisayarýndan gelen DOSYA (IFormFile),
        //        entity'deki Image1 ise o dosyanýn Azure'daki ADRESÝ (string).
        //        Ýsimler ayný olduðu için AutoMapper eþlemeye çalýþýr ama dosyayý adrese çeviremez.
        //
        // Çözüm: Ignore ? "Bu alanlara sen dokunma."
        //        Metin alanlarýný AutoMapper doldurur, resim alanlarýný handler doldurur:
        //        handler dosyayý Azure'a yükler ve dönen adresi about.Image1'e yazar.
        CreateMap<CreateAboutCommand, About>()
            .ForMember(dest => dest.Image1, opt => opt.Ignore())
            .ForMember(dest => dest.Image2, opt => opt.Ignore())
            .ForMember(dest => dest.Image3, opt => opt.Ignore());

        // ---------------------------------------------------------------
        // GÜNCELLEME: Kullanýcýnýn isteði (UpdateAboutCommand) ? MEVCUT About'un üzerine
        // ---------------------------------------------------------------
        // Kullanýldýðý yer: UpdateAboutCommandHandler ? _mapper.Map(request, about)
        // Burada yeni nesne oluþmaz; veritabanýndan çekilen about'un alanlarý deðiþtirilir.
        //
        // Id Ignore:
        //   Kaydýn kimliði güncellemede asla deðiþmemeli. Zaten ayný deðer gelecek,
        //   ama "bu eþleme Id'ye dokunmaz" diye açýkça garanti altýna alýyoruz (savunma amaçlý).
        //
        // Resimler Ignore (EN ÖNEMLÝ KISIM):
        //   Kullanýcý güncellemede resim seçmezse request.Image1 = null gelir.
        //   Ignore olmasaydý AutoMapper bu null'u about.Image1'e yazar, eski resim adresi kaybolurdu
        //   (veritabanýnda bu alan boþ olamadýðý için kayýt da hata verirdi).
        //   Ignore sayesinde eski adresler korunur; handler sadece YENÝ resim seçilen alanlarý deðiþtirir.
        CreateMap<UpdateAboutCommand, About>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Image1, opt => opt.Ignore())
            .ForMember(dest => dest.Image2, opt => opt.Ignore())
            .ForMember(dest => dest.Image3, opt => opt.Ignore());

        // KISA KURAL:
        // Birebir kopyalanabilecek alanlarý AutoMapper'a býrak.
        // Dönüþüm ya da özel mantýk gerektiren alanlarý Ignore edip handler'da kendin doldur.
    }
}