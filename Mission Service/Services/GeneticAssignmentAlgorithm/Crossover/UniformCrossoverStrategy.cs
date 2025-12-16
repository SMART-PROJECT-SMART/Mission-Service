using System.Runtime.InteropServices;
using Mission_Service.Common.Constants;
using Mission_Service.Models;
using Mission_Service.Models.choromosomes;
using Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover.Interfaces;

namespace Mission_Service.Services.GeneticAssignmentAlgorithm.Crossover
{
    public class UniformCrossoverStrategy : ICrossoverStrategy
    {
        public CrossoverResult CrossoverChromosomes(
            AssignmentChromosome firstChromosome,
            AssignmentChromosome secondChromosome
        )
        {
            AssignmentChromosome firstChild = CreateMissionBasedChild(
                firstChromosome,
                secondChromosome
            );
            AssignmentChromosome secondChild = CreateMissionBasedChild(
                secondChromosome,
                firstChromosome
            );

            return new CrossoverResult(firstChild, secondChild);
        }

        private AssignmentChromosome CreateMissionBasedChild(
            AssignmentChromosome primaryParent,
            AssignmentChromosome secondaryParent
        )
        {
            ReadOnlySpan<AssignmentGene> primaryParentGenes = CollectionsMarshal.AsSpan(
                primaryParent.AssignmentsList
            );
            ReadOnlySpan<AssignmentGene> secondaryParentGenes = CollectionsMarshal.AsSpan(
                secondaryParent.AssignmentsList
            );

            Dictionary<string, AssignmentGene> secondaryParentLookup = BuildMissionLookupFromGenes(
                secondaryParentGenes
            );

            List<AssignmentGene> childGenes = new(
                primaryParentGenes.Length + secondaryParentGenes.Length
            );
            HashSet<string> processedMissionIds = new(primaryParentGenes.Length);

            InheritGenesFromPrimaryParent(
                primaryParentGenes,
                secondaryParentLookup,
                childGenes,
                processedMissionIds
            );
            InheritUniqueGenesFromSecondaryParent(
                secondaryParentGenes,
                processedMissionIds,
                childGenes
            );

            return new AssignmentChromosome { Assignments = childGenes, IsValid = true };
        }

        private Dictionary<string, AssignmentGene> BuildMissionLookupFromGenes(
            ReadOnlySpan<AssignmentGene> genes
        )
        {
            Dictionary<string, AssignmentGene> missionLookup = new(genes.Length);

            foreach (AssignmentGene gene in genes)
            {
                missionLookup[gene.Mission.Id] = gene;
            }

            return missionLookup;
        }

        private void InheritGenesFromPrimaryParent(
            ReadOnlySpan<AssignmentGene> primaryParentGenes,
            Dictionary<string, AssignmentGene> secondaryParentLookup,
            List<AssignmentGene> childGenes,
            HashSet<string> processedMissionIds
        )
        {
            foreach (AssignmentGene primaryGene in primaryParentGenes)
            {
                AssignmentGene selectedGene = SelectGeneFromParents(
                    primaryGene,
                    secondaryParentLookup
                );

                childGenes.Add(selectedGene.Clone());
                processedMissionIds.Add(selectedGene.Mission.Id);
            }
        }

        private AssignmentGene SelectGeneFromParents(
            AssignmentGene primaryParentGene,
            Dictionary<string, AssignmentGene> secondaryParentLookup
        )
        {
            bool secondaryParentHasSameMission = secondaryParentLookup.TryGetValue(
                primaryParentGene.Mission.Id,
                out AssignmentGene? secondaryParentGene
            );

            if (!secondaryParentHasSameMission)
            {
                return primaryParentGene;
            }

            bool shouldUseSecondaryParentGene =
                Random.Shared.NextDouble() < MissionServiceConstants.Crossover.GENE_SELECTION_PROBABILITY;

            return shouldUseSecondaryParentGene ? secondaryParentGene! : primaryParentGene;
        }

        private void InheritUniqueGenesFromSecondaryParent(
            ReadOnlySpan<AssignmentGene> secondaryParentGenes,
            HashSet<string> processedMissionIds,
            List<AssignmentGene> childGenes
        )
        {
            foreach (AssignmentGene secondaryGene in secondaryParentGenes)
            {
                bool isUniqueMission = processedMissionIds.Add(secondaryGene.Mission.Id);

                if (isUniqueMission)
                {
                    childGenes.Add(secondaryGene.Clone());
                }
            }
        }
    }
}
