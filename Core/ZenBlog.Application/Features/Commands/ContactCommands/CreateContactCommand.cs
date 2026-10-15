using MediatR;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.ContactCommands;

public record CreateContactCommand(
                                        string? MapLocationUrl,
                                        string? Address,
                                        string? PhoneNumber,
                                        string? Email):IRequest<BaseResult<object>> ;
