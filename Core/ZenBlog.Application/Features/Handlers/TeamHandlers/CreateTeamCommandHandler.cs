using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using ZenBlog.Application.Base;
using ZenBlog.Application.Constants;
using ZenBlog.Application.Contracts.Infrastructure;
using ZenBlog.Application.Contracts.Persistence;
using ZenBlog.Application.Features.Commands.TeamCommands;
using ZenBlog.Domain.Entities;

namespace ZenBlog.Application.Features.Handlers.TeamHandlers;

public class CreateTeamCommandHandler(IRepository<Team> _repository,
                                      IUnitOfWork _unitOfWork,
                                      IMapper _mapper,
                                      IFileService _fileService)
    : IRequestHandler<CreateTeamCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = _mapper.Map<Team>(request);

        team.Image = await UploadAsync(request.Image!, cancellationToken);

        await _repository.CreateAsync(team);
        var result = await _unitOfWork.SaveChangesAsync();

        if (!result)
        {
            await _fileService.DeleteAsync(team.Image, cancellationToken);   // kaydedilemediyse yükleneni geri al
            return BaseResult<object>.Fail("Ekip üyesi eklenirken bir sorun oluþtu.");
        }

        return BaseResult<object>.Success(team.Id);
    }

    private async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        return await _fileService.UploadAsync(stream, file.FileName, ImageFolders.Teams, cancellationToken);
    }
}