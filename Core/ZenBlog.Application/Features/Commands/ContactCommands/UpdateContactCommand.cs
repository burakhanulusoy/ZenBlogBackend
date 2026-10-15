using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.ContactCommands;

public record UpdateContactCommand(Guid Id,
                                    string? MapLocationUrl,
                                    string? Address,
                                    string? PhoneNumber,
                                    string? Email):IRequest<BaseResult<object>>;
