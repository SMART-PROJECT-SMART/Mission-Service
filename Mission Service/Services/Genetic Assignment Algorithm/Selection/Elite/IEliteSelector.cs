using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Selection.Elite;

public interface IEliteSelector
{
    IEnumerable<AssignmentChromosome> SelectElite(
        IEnumerable<AssignmentChromosome> chromosomePopulation,
        double elitePercentageOfPopulation,
        int totalPopulationSize
    );
}
