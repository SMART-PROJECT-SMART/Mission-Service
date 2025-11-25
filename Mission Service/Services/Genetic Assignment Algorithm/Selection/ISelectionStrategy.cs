using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Selection
{
    public interface ISelectionStrategy
    {
        AssignmentChromosome selectParentChromosome(IEnumerable<AssignmentChromosome> population);
    }
}
