using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenBlog.Application.Base;

namespace ZenBlog.Application.Features.Commands.CommentCommands;

public record CreateCommentCommand(string? Body,
                                     Guid? AppUserId,
                                     Guid? BlogId
                                  ):IRequest<BaseResult<object>>;
