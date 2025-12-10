using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Selection.Elite;

public interface IEliteSelector
{
    List<AssignmentChromosome> SelectElite(
        List<AssignmentChromosome> chromosomePopulation,
        double elitePercentageOfPopulation,
        int totalPopulationSize
    );
}
