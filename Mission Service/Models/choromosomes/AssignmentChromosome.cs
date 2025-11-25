namespace Mission_Service.Models.choromosomes
{
    public class AssignmentChromosome
    {
        public IEnumerable<AssignmentGene> Assignments { get; set; }
        public double FitnessScore { get; set; }
        public bool IsValid { get; set; }
    }
}
