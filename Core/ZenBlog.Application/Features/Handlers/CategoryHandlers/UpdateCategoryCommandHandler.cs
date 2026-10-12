using AutoMapper;
using FluentValidation;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CategoryCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CategoryHandlers
{
    public class UpdateCategoryCommandHandler(IRepository<Category> _repository,
                                              IMapper _mapper,
                                              IUnitOfWork _unitOfWork) : IRequestHandler<UpdateCategoryCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {



            var category = await _repository.GetByIdAsync(request.Id);

            if (category is null)
            {
                return BaseResult<object>.Fail("Bu Id'ye sahip kategori bulunamadý.");
            }

            // ---------------------------------------------------------------
            // AUTOMAPPER'DA ÝKÝ FARKLI MAP KULLANIMI
            // ---------------------------------------------------------------
            //
            // 1) _mapper.Map<Category>(request)
            //    ? YENÝ bir Category nesnesi oluþturur (arka planda "new Category()").
            //    ? CREATE iþlemlerinde kullanýlýr, çünkü veritabanýnda henüz kayýt yok.
            //    ? UPDATE'te kullanýrsan HATA ALIRSIN: EF zaten ayný Id'li bir nesneyi
            //      takip ediyor, ikinci bir nesne gelince þu hatayý fýrlatýr:
            //      "The instance of entity type 'Category' cannot be tracked because
            //       another instance with the same key value is already being tracked."
            //    ? Ayrýca request'te olmayan alanlar (CreatedDate vb.) varsayýlan
            //      deðere döner ve veri kaybý olur.
            //
            // 2) _mapper.Map(request, category)
            //    ? Yeni nesne OLUÞTURMAZ, request'teki deðerleri MEVCUT nesnenin
            //      üzerine yazar.
            //    ? UPDATE iþlemlerinde kullanýlýr: önce veritabanýndan çek, sonra
            //      üzerine yaz.
            //    ? EF'in elinde tek nesne olduðu için çakýþma olmaz, request'te
            //      olmayan alanlar da korunur.
            //
            // KURAL: Create ? Map<Category>(request)
            //        Update ? Map(request, category)
            // ---------------------------------------------------------------
            _mapper.Map(request, category);

            // category zaten EF tarafýndan takip edildiði için Update çaðrýsý
            // aslýnda zorunlu deðil, ama niyeti açýkça göstermek için kalabilir.
            _repository.Update(category);

            var result = await _unitOfWork.SaveChangesAsync();

            return result
                ? BaseResult<object>.Success()
                : BaseResult<object>.Fail("Güncelleme yapýlýrken bir sorun oluþtu.");





        }
    }
}
