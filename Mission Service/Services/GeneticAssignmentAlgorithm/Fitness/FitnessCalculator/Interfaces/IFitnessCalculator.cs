using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Fitness.FitnessCalculator.Interfaces
{
    public interface IFitnessCalculator
    {
        public double CalculateFitness(AssignmentChromosome chromosome);
    }
}
