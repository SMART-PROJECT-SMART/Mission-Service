using System.Security.AccessControl;
using Mission_Service.Common.Constants;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Services.AssignmentRequestQueue;
using Mission_Service.Services.AssignmentResultManager;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection;
using Mission_Service.Services.AssignmentSuggestionWorker;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Reproduction;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite;
using Mission_Service.Services.UAVStatusService;

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

        public static IServiceCollection AddAppConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddAlgorithmConfig(configuration);
            services.AddTelemetryWeightsConfig(configuration);
            services.AddFitnessWeightsConfig(configuration);
            services.AddAssignmentQueueConfig(configuration);
            return services;
        }

        private static IServiceCollection AddAlgorithmConfig(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services.Configure<AssignmentAlgorithmConfiguration>(
                configuration.GetSection(
                    MissionServiceConstants.Configuration.ALGORITHM_CONFIG_SECTION
                )
            );
        }

        private static IServiceCollection AddTelemetryWeightsConfig(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services.Configure<TelemetryWeightsConfiguration>(
                configuration.GetSection(
                    MissionServiceConstants.Configuration.TELEMETRY_WEIGHTS_CONFIG_SECTION
                )
            );
        }

        private static IServiceCollection AddFitnessWeightsConfig(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services.Configure<FitnessWeightsConfiguration>(
                configuration.GetSection(
                    MissionServiceConstants.Configuration.FITNESS_WEIGHTS_CONFIG_SECTION
                )
            );
        }

        private static IServiceCollection AddAssignmentQueueConfig(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services.Configure<AssignmentRequestQueueConfiguration>(
                configuration.GetSection(
                    MissionServiceConstants.Configuration.ASSIGNMENT_QUEUE_CONFIG_SECTION
                )
            );
        }

        public static IServiceCollection AddAssignmentAlgorithmServices(
            this IServiceCollection services
        )
        {
            services.AddSingleton<IAssignmentAlgorithm, GeneticAssignmentAlgorithm>();
            services.AddSingleton<IFitnessCalculator, FitnessCalculator>();
            services.AddSingleton<IPopulationInitializer, PopulationInitializer>();
            services.AddSingleton<ISelectionStrategy, TournamentSelectionStrategy>();
            services.AddSingleton<ICrossoverStrategy, UniformCrossoverStrategy>();
            services.AddSingleton<IMutationStrategy, SwapMutationStrategy>();
            services.AddSingleton<IEliteSelector, EliteSelector>();
            services.AddSingleton<IOffspringGenerator, OffspringGenerator>();
            services.AddSingleton<IParallelExecutor, ParallelExecutor>();
            services.AddRepairStrategies();
            services.AddRepairPipeline();
            return services;
        }

        private static IServiceCollection AddRepairPipeline(this IServiceCollection services)
        {
            services.AddSingleton<IRepairPipeline, RepairPipline>();
            return services;
        }

        private static IServiceCollection AddRepairStrategies(this IServiceCollection services)
        {
            services.AddSingleton<IRepairStrategy, TypeMismatchRepairStrategy>();
            services.AddSingleton<IRepairStrategy, TimeWindowRepairStrategy>();
            services.AddSingleton<IRepairStrategy, OverlapRepairStrategy>();
            services.AddSingleton<IRepairStrategy, DuplicateMissionRepairStrategy>();
            return services;
        }

        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddSingleton<IAssignmentSuggestionQueue, AssignmentSuggestionQueue>();
            services.AddSingleton<IAssignmentResultManager, AssignmentResultManager>();
            services.AddHostedService<AssignmentSuggestionWorker>();
            return services;
        }

        public static IServiceCollection AddHttpClients(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddHttpClient(
                MissionServiceConstants.HttpClients.LTS_HTTP_CLIENT,
                client =>
                {
                    string? baseAddress = configuration.GetSection(
                        MissionServiceConstants.Configuration.LTS_CONFIG_SECTION
                    )[MissionServiceConstants.Configuration.BASE_ADDRESS_KEY];

                    if (!string.IsNullOrEmpty(baseAddress))
                    {
                        client.BaseAddress = new Uri(baseAddress);
                    }
                }
            );

            return services;
        }

        public static IServiceCollection AddUAVServices(this IServiceCollection services)
        {
            services.AddSingleton<IUAVStatusService, UAVStatusService>();
            return services;
        }
    }
}
