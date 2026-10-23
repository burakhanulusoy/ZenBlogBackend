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

public class UpdateTeamCommandHandler(IRepository<Team> _repository,
                                      IUnitOfWork _unitOfWork,
                                      IMapper _mapper,
                                      IFileService _fileService)
    : IRequestHandler<UpdateTeamCommand, BaseResult<object>>
{
    public async Task<BaseResult<object>> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _repository.GetByIdAsync(request.Id!.Value);
        if (team is null)
            return BaseResult<object>.Fail("Bu Id'ye sahip ekip üyesi bulunamadý.");

        // Metin alanlarýný kopyala (Id ve Image eþlemede Ignore edildi)
        _mapper.Map(request, team);

        string? oldUrl = null;   // kayýt BAÞARILI olursa silinecek eski fotoðraf
        string? newUrl = null;   // kayýt BAÞARISIZ olursa silinecek yeni fotoðraf

        if (request.Image is not null)
        {
            oldUrl = team.Image;
            team.Image = await UploadAsync(request.Image, cancellationToken);
            newUrl = team.Image;
        }

        _repository.Update(team);
        var result = await _unitOfWork.SaveChangesAsync();

        if (!result)
        {
            if (newUrl is not null)
                await _fileService.DeleteAsync(newUrl, cancellationToken);
            return BaseResult<object>.Fail("Ekip üyesi güncellenirken bir sorun oluþtu.");
        }

        if (oldUrl is not null)
            await _fileService.DeleteAsync(oldUrl, cancellationToken);

        return BaseResult<object>.Success();
    }

    private async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        return await _fileService.UploadAsync(stream, file.FileName, ImageFolders.Teams, cancellationToken);
    }
}