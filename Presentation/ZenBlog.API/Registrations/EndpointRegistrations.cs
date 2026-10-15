using ZenBlog.API.Endpoints.Blog;
using ZenBlog.API.Endpoints.Category;
using ZenBlog.API.Endpoints.Comment;
using ZenBlog.API.Endpoints.User;

namespace ZenBlog.API.Registrations
{
    public static class EndpointRegistrations
    {

        public static void RegisterEndpoints(this IEndpointRouteBuilder app)
        {
            app.RegisterCategoryEndpoints();
            app.RegisterUserEndpoints();
            app.RegisterBlogEndpoints();
            app.RegisterCommentEndpoint();
        }


    }
}
