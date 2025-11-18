using Microsoft.Extensions.Options;
using Mission_Service.Config;
using Mission_Service.Models;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness_Calculator
{
    public class FitnessCalculator : IFitnessCalculator
    {
        private readonly TelemetryWeightsConfiguration _telemetryWeightsConfiguration;
        private readonly FitnessWeightsConfiguration _fitnessWeightsConfiguration;
        
        public FitnessCalculator(IOptions<TelemetryWeightsConfiguration> telemetryWeightsConfiguration,
            IOptions<FitnessWeightsConfiguration> fitnessWeightsConfiguration)
        {
            _telemetryWeightsConfiguration = telemetryWeightsConfiguration.Value;
            _fitnessWeightsConfiguration = fitnessWeightsConfiguration.Value;
        }
        
        public double CalculateFitness(AssignmentChromosome chromosome, IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs)
        {
            double totalFitnessScore = 0.0;
            return 0;
        }
    }
}
