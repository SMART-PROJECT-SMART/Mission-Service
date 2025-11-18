using System.Security.AccessControl;
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

        public static IServiceCollection AddAppConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAlgorithmConfig(configuration);
            services.AddTelemetryWeightsConfig(configuration);
            services.AddFitnessWeightsConfig(configuration);
            return services;
        }
        private static IServiceCollection AddAlgorithmConfig(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.Configure<AssignmentAlgorithmConfiguration>(
                configuration.GetSection(MissionServiceConstants.Configuration.ALGORITHM_CONFIG_SECTION));
        }

        private static IServiceCollection AddTelemetryWeightsConfig(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.Configure<TelemetryWeightsConfiguration>(
                configuration.GetSection(MissionServiceConstants.Configuration.TELEMETRY_WEIGHTS_CONFIG_SECTION));
        }
        private static IServiceCollection AddFitnessWeightsConfig(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.Configure<FitnessWeightsConfiguration>(
                configuration.GetSection(MissionServiceConstants.Configuration.FITNESS_WEIGHTS_CONFIG_SECTION));
        }
    }
}
