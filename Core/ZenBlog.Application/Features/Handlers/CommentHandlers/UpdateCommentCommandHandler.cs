using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CommentCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers
{
    public class UpdateCommentCommandHandler(IRepository<Comment> _repository,
                                             IUnitOfWork _unitOfWork,
                                             IMapper _mapper) : IRequestHandler<UpdateCommentCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {

            var comment = await _repository.GetByIdAsync(request.Id!.Value);

            if (comment is null)
                return BaseResult<object>.Fail("Bu Id'ye sahip blog yazýsý bulunamdý.");

            comment.Body = request.Body!;

            _repository.Update(comment);

            var result = await _unitOfWork.SaveChangesAsync();


            return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Yorum güncellenirken bir sorun oluþtu.");





        }
    }
}
