using ZenBlog.API.Endpoints.About;
using ZenBlog.API.Endpoints.Blog;
using ZenBlog.API.Endpoints.Category;
using ZenBlog.API.Endpoints.Comment;
using ZenBlog.API.Endpoints.Contact;
using ZenBlog.API.Endpoints.ContactMessage;
using ZenBlog.API.Endpoints.Home;
using ZenBlog.API.Endpoints.Role;
using ZenBlog.API.Endpoints.SocialMedia;
using ZenBlog.API.Endpoints.SubComment;
using ZenBlog.API.Endpoints.Team;
using ZenBlog.API.Endpoints.User;
using ZenBlog.API.Endpoints.WriterApplication;

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
            app.RegisterSubCommentEndpoints();
            app.RegisterContactEndpoints();
            app.RegisterContactMessageEndpoints();
            app.RegisterAboutEndpoints();
            app.RegisterRoleEndpoints();
            app.RegisterWriterApplicationEndpoints();
            app.RegisterSocialMediaEndpoint();
            app.RegisterTeamEndpoints();
            app.RegisterHomeEndpoints();   
        }


    }
}
