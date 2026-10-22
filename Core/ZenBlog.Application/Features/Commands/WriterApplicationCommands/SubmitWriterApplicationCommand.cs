using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.WriterApplicationCommands;

// Kullanýcý: yazarlýk baþvurusu gönder. Kim olduðu token'dan bulunuyor.
public record SubmitWriterApplicationCommand(List<Guid> CategoryIds,
                                             string? Motivation,
                                             string? Experience,
                                             string? SampleLinks,
                                             bool AcceptedRules) : IRequest<BaseResult<object>>;