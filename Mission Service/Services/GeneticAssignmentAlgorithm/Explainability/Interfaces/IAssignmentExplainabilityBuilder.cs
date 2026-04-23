using Mission_Service.Models;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Explainability.Interfaces;

public interface IAssignmentExplainabilityBuilder
{
    AssignmentExplainability Build(
        List<AssignmentGene> assignments,
        IReadOnlyCollection<UAV> availableUavs
    );
}
