using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Selection.Interfaces
{
    public interface ISelectionStrategy
    {
        AssignmentChromosome SelectParentChromosome(IEnumerable<AssignmentChromosome> population);
    }
}
