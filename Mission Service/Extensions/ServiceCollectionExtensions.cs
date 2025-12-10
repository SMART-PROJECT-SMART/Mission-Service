using System.Security.AccessControl;
using Mission_Service.Common.Constants;
using Mission_Service.Config;
using Mission_Service.Models;
using Mission_Service.Services.Assignment_Request_Queue;
using Mission_Service.Services.Assignment_Suggestion_Worker;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Crossover;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Execution;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness_Calculator;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness.Fitness_Calculator;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Main_Algorithm;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Mutation;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Population.Population_Initilizer;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Repair;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Repair.Pipeline;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Repair.Strategies;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Reproduction;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Selection;
using Mission_Service.Services.Genetic_Assignment_Algorithm.Selection.Elite;

namespace Mission_Service.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddHttpClient(MissionServiceConstants.HttpClients.CALLBACK_HTTP_CLIENT);
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
            services.AddHostedService<AssignmentSuggestionWorker>();
            return services;
        }
    }
}
