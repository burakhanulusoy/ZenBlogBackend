using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenBlog.Application.Features.Results.UserResults;

namespace ZenBlog.Application.Features.Results.CommentResults;

public record CommentSubCommentResult(Guid Id,
                                      string Body,
                                      DateTime CreatedAt,
                                      GetUsersQueryResult AppUser);
