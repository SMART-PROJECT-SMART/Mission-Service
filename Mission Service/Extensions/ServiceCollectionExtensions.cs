using Microsoft.Extensions.Options;
using Mission_Service.Common.Constants;
using Mission_Service.Config;
using Mission_Service.DataBase.MongoDB.Repositoreis;
using Mission_Service.DataBase.MongoDB.Repositoreis.Interfaces;
using Mission_Service.DataBase.MongoDB.Services;
using Mission_Service.Services.AssignmentRequestQueue;
using Mission_Service.Services.AssignmentRequestQueue.Interfaces;
using Mission_Service.Services.AssignmentResultManager;
using Mission_Service.Services.AssignmentResultManager.Interfaces;
using Mission_Service.Services.AssignmentSuggestionWorker;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Execution.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm;
using Mission_Service.Services.GeneticAssignmentAlgorithm.MainAlgorithm.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Mutation.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Population.PopulationInitilizer.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Pipeline.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Repair.Strategies.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Reproduction;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Reproduction.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite.Interfaces;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Interfaces;
using Mission_Service.Services.MissionExecutor;
using Mission_Service.Services.MissionExecutor.Interfaces;
using Mission_Service.Services.UAVFetcher;
using Mission_Service.Services.UAVFetcher.Interfaces;
using Mission_Service.Services.UAVStatusService;
using Mission_Service.Services.UAVStatusService.Interfaces;
using MongoDB.Driver;

namespace Mission_Service.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter()
                    );
                });
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
            services.AddMongoDbConfiguration(configuration);
            return services;
        }

        private static IServiceCollection AddMongoDbConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<MongoDBConfiguration>(
                configuration.GetSection(
                    MissionServiceConstants.Configuration.MONGODB_CONFIG_SECTION
                )
            );
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                MongoDBConfiguration mongoDbConfiguration = serviceProvider
                    .GetRequiredService<IOptions<MongoDBConfiguration>>()
                    .Value;
                MongoClientSettings mongoClientSettings = MongoClientSettings.FromConnectionString(
                    mongoDbConfiguration.ConnectionString
                );
                mongoClientSettings.MaxConnectionPoolSize =
                    mongoDbConfiguration.MaxConnectionPoolSize;
                mongoClientSettings.MinConnectionPoolSize =
                    mongoDbConfiguration.MinConnectionPoolSize;
                mongoClientSettings.MaxConnectionIdleTime =
                    mongoDbConfiguration.MaxConnectionIdleTime;
                mongoClientSettings.ServerSelectionTimeout =
                    mongoDbConfiguration.ServerSelectionTimeout;
                return new MongoClient(mongoClientSettings);
            });
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
            services.AddScoped<IAssignmentAlgorithm, GeneticAssignmentAlgorithm>();
            services.AddScoped<IFitnessCalculator, FitnessCalculator>();
            services.AddScoped<IPopulationInitializer, PopulationInitializer>();
            services.AddScoped<ISelectionStrategy, TournamentSelectionStrategy>();
            services.AddScoped<ICrossoverStrategy, UniformCrossoverStrategy>();
            services.AddScoped<IMutationStrategy, SwapMutationStrategy>();
            services.AddScoped<IEliteSelector, EliteSelector>();
            services.AddScoped<IOffspringGenerator, OffspringGenerator>();
            services.AddScoped<
                Services.GeneticAssignmentAlgorithm.Evolution.Interfaces.IEvolutionStrategy,
                Services.GeneticAssignmentAlgorithm.Evolution.StandardEvolutionStrategy
            >();
            services.AddSingleton<IParallelExecutor, ParallelExecutor>();
            services.AddRepairStrategies();
            services.AddRepairPipeline();
            return services;
        }

        private static IServiceCollection AddRepairPipeline(this IServiceCollection services)
        {
            services.AddScoped<IRepairPipeline, RepairPipline>();
            return services;
        }

        private static IServiceCollection AddRepairStrategies(this IServiceCollection services)
        {
            services.AddScoped<IRepairStrategy, TypeMismatchRepairStrategy>();
            services.AddScoped<IRepairStrategy, TimeWindowRepairStrategy>();
            services.AddScoped<IRepairStrategy, OverlapRepairStrategy>();
            services.AddScoped<IRepairStrategy, DuplicateMissionRepairStrategy>();
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
            services.AddHttpClient(
                MissionServiceConstants.HttpClients.SIMULATOR_CLIENT,
                client =>
                {
                    string? baseUrl = configuration[
                        MissionServiceConstants.Configuration.SIMULATOR_BASE_URL_KEY
                    ];

                    if (!string.IsNullOrEmpty(baseUrl))
                    {
                        client.BaseAddress = new Uri(baseUrl);
                    }

                    client.Timeout = TimeSpan.FromMinutes(5);
                }
            );

            return services;
        }

        public static IServiceCollection AddUAVServices(this IServiceCollection services)
        {
            services.AddSingleton<IUAVStatusService, UAVStatus>();
            services.AddSingleton<IUAVFetcher, UAVFetcher>();
            return services;
        }

        public static IServiceCollection AddMongoDbServices(this IServiceCollection services)
        {
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            services.AddScoped<IAssignmentDBService, AssignmentDbService>();
            return services;
        }

        public static IServiceCollection AddMissionExecutor(this IServiceCollection services)
        {
            services.AddScoped<IMissionExecutor, MissionExecutor>();

            return services;
        }
    }
}
