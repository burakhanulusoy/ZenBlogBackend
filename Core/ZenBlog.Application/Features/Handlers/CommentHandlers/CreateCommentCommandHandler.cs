using AutoMapper;
using MediatR;
using ZenBlog.Application.Base;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.CommentCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.CommentHandlers
{
    public class CreateCommentCommandHandler(IRepository<Comment> _repository,
                                             IUnitOfWork _unitOfWork,
                                             IMapper _mapper,
                                             IRepository<Blog> _blogRepository) : IRequestHandler<CreateCommentCommand, BaseResult<object>>
    {
        public async Task<BaseResult<object>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {


            var blog = await _blogRepository.GetByIdAsync(request.BlogId!.Value);

            if(blog is null)
            {
                return BaseResult<object>.Fail("Blog bulunamadý");
            }

            var comment = _mapper.Map<Comment>(request);

            await _repository.CreateAsync(comment);

            var result = await _unitOfWork.SaveChangesAsync();

            return result ? BaseResult<object>.Success() : BaseResult<object>.Fail("Yorum eklenirken bir sorun oluþtu.");




        }
    }
}
