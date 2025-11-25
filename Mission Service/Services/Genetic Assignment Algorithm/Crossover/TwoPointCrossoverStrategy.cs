using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Common.Constants;

namespace Mission_Service.Services.Genetic_Assignment_Algorithm.Crossover
{
    public class TwoPointCrossoverStrategy : ICrossoverStrategy
    {
        public CrossoverResult CrossoverChromosomes(
            AssignmentChromosome firstChromosome,
 AssignmentChromosome secondChromosome
        )
 {
       int minLength = Math.Min(
         firstChromosome.Assignments.Count(),
        secondChromosome.Assignments.Count()
            );

            if (minLength < MissionServiceConstants.Crossover.MIN_CHROMOSOMES_FOR_CROSSOVER)
  {
           return new CrossoverResult
     {
   FirstChromosome = firstChromosome,
            SecondChromosome = secondChromosome,
     };
       }

            int firstCrossoverPoint = Random.Shared.Next(
            MissionServiceConstants.Crossover.MIN_CROSSOVER_POINT,
        minLength - 1
        );
       int secondCrossoverPoint = Random.Shared.Next(firstCrossoverPoint + 1, minLength);

         AssignmentChromosome firstChild = CreateChildChromosome(
          firstChromosome,
        secondChromosome,
        firstCrossoverPoint,
  secondCrossoverPoint,
                minLength
            );
   AssignmentChromosome secondChild = CreateChildChromosome(
secondChromosome,
       firstChromosome,
        firstCrossoverPoint,
              secondCrossoverPoint,
        minLength
      );

 return new CrossoverResult
         {
     FirstChromosome = firstChild,
                SecondChromosome = secondChild,
            };
        }

        private AssignmentChromosome CreateChildChromosome(
 AssignmentChromosome firstChromosome,
        AssignmentChromosome secondChromosome,
            int firstCrossoverPoint,
       int secondCrossoverPoint,
            int length
        )
   {
            AssignmentChromosome childChromosome = new AssignmentChromosome();
     List<AssignmentGene> childGenes = new List<AssignmentGene>(length);

    for (int assignmentIndex = 0; assignmentIndex < length; assignmentIndex++)
      {
      AssignmentGene sourceGene;
          if (
       assignmentIndex >= firstCrossoverPoint
      && assignmentIndex < secondCrossoverPoint
                )
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
          Duration = sourceGene.Duration,
    };
 childGenes.Add(newGene);
         }

        childChromosome.Assignments = childGenes;
return childChromosome;
        }
    }
}
