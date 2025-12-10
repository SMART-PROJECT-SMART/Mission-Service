using Microsoft.Extensions.Options;
using Mission_Service.Config;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Selection
{
    public class TournamentSelectionStrategy : ISelectionStrategy
    {
        private readonly AssignmentAlgorithmConfiguration _algorithmConfiguration;

        public TournamentSelectionStrategy(
            IOptions<AssignmentAlgorithmConfiguration> algorithmConfiguration
        )
        {
            _algorithmConfiguration = algorithmConfiguration.Value;
        }

        public AssignmentChromosome SelectParentChromosome(
            IEnumerable<AssignmentChromosome> population
        )
        {
            List<AssignmentChromosome> tournamentContestants = new List<AssignmentChromosome>();
            List<AssignmentChromosome> populationList = population.ToList();
            for (
                int contestantIndex = 0;
                contestantIndex < _algorithmConfiguration.TournamentSize;
                contestantIndex++
            )
            {
                int randomIndex = Random.Shared.Next(populationList.Count);
                tournamentContestants.Add(populationList[randomIndex]);
            }
            AssignmentChromosome bestChromosome = tournamentContestants
                .OrderByDescending(chromosome => chromosome.FitnessScore)
                .First();
            return bestChromosome;
        }
    }
}
