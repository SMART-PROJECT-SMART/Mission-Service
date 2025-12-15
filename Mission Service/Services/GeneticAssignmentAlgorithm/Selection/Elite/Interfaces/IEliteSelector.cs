using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Elite.Interfaces;

public interface IEliteSelector
{
    IEnumerable<AssignmentChromosome> SelectElite(
        IEnumerable<AssignmentChromosome> chromosomePopulation,
        double elitePercentageOfPopulation,
        int totalPopulationSize
    );
}
