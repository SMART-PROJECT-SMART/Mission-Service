using Mission_Service.Models;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Fitness_Calculator
{
    public class FitnessCalculator : IFitnessCalculator
    {
        public double CalculateFitness(AssignmentChromosome chromosome, IEnumerable<Mission> missions,
            IEnumerable<UAV> uavs)
        {
            double totalFitnessScore = 0.0;
            return 0;
        }
    }
}
