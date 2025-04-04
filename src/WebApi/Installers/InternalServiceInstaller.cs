using Application;
using Infrastructure;

namespace WebApi.Installers
{
    public static class InternalServiceInstaller
    {
        public static IServiceCollection AddInternalServices(this IServiceCollection services,
                                                             IConfiguration          configuration)
        {
            services.AddApplication();
            services.AddInfrastructure(configuration);
            services.AddCors();


            return services;
        }
    }
}