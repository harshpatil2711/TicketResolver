using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TicketResolver.Startup))]
namespace TicketResolver
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
