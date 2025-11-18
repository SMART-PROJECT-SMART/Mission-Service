using Mission_Service.Common.Constants;
using Mission_Service.Config;

namespace Mission_Service.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            return services;
        }
        public static IServiceCollection AddAlgorithmConfig(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<AssignmentAlgorithmConfiguration>(
                configuration.GetSection(MissionServiceConstants.Configuration.ALGORITHM_CONFIG_SECTION));
        }
    }
}
