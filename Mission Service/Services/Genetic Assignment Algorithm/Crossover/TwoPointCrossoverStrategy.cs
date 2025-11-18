using Mission_Service.Models;
using Mission_Service.Models.choromosomes;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Crossover
{
    public class TwoPointCrossoverStrategy : ICrossoverStrategy
    {
        public CrossoverResult CrossoverChromosomes(AssignmentChromosome firstChromosome, AssignmentChromosome secondChromosome)
        {
            int minLength = Math.Min(firstChromosome.Assignments.Count(), secondChromosome.Assignments.Count());
            int firstCrossoverPoint = Random.Shared.Next(1, minLength);
            int secondCrossoverPoint = Random.Shared.Next(firstCrossoverPoint + 1,minLength+1);
            AssignmentChromosome firstChild = CreateChildChromosome(firstChromosome, secondChromosome, firstCrossoverPoint, secondCrossoverPoint, minLength);
            AssignmentChromosome secondChild = CreateChildChromosome(secondChromosome, firstChromosome, firstCrossoverPoint, secondCrossoverPoint, minLength);

            return new CrossoverResult
            {
                FirstChromosome = firstChild,
                SecondChromosome = secondChild
            };
        }

        private AssignmentChromosome CreateChildChromosome(AssignmentChromosome firstChromosome,
            AssignmentChromosome secondChromosome, int firstCrossoverPoint, int secondCrossoverPoint, int length)
        {
            AssignmentChromosome childChromosome = new AssignmentChromosome();
            for (int assignmentIndex = 0; assignmentIndex < length; assignmentIndex++)
            {
                AssignmentGene sourceGene;
                if (assignmentIndex >= firstCrossoverPoint && assignmentIndex < secondCrossoverPoint)
                {
                    sourceGene = secondChromosome.Assignments.ElementAt(assignmentIndex);
                }
                else
                {
                    sourceGene = firstChromosome.Assignments.ElementAt(assignmentIndex);
                }

                AssignmentGene newGene = new AssignmentGene
                {
                    Mission = sourceGene.Mission,
                    UAV = sourceGene.UAV,
                    StartTime = sourceGene.StartTime,
                    Duration = sourceGene.Duration
                };
                childChromosome.Assignments = childChromosome.Assignments.Append(newGene);
            }
            return childChromosome;
        }
    }
}
