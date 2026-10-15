using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.SubCommentCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.SubCommentHandlers;

public class CreateSubCommentCommandHandler(IRepository<SubComment> _repository,
                                            IRepository<Comment> _commentRepository,
                                            IUnitOfWork _unitOfWork,
                                            IMapper _mapper)
    : IRequestHandler<CreateSubCommentCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(CreateSubCommentCommand request, CancellationToken cancellationToken)
    {
        // Cevap verilmek istenen yorum gerçekten var mý?
        // (Soft delete sayesinde silinmiþ bir yoruma da cevap verilemez.)
        var comment = await _commentRepository.GetByIdAsync(request.CommentId!.Value);

        if (comment is null)
            return BaseResult<object>.Fail("Cevap verilmek istenen yorum bulunamadý.");

        var subComment = _mapper.Map<SubComment>(request);

        await _repository.CreateAsync(subComment);
        var result = await _unitOfWork.SaveChangesAsync();

        return result
            ? BaseResult<object>.Success()
            : BaseResult<object>.Fail("Cevap eklenirken bir sorun oluþtu.");
    }
}