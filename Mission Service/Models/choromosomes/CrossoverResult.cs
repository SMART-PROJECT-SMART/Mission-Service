namespace Mission_Service.Models.choromosomes
{
    public struct CrossoverResult
    {
        public AssignmentChromosome FirstChromosome { get; set; }
        public AssignmentChromosome SecondChromosome { get; set; }

        public CrossoverResult(AssignmentChromosome firstChromosome, AssignmentChromosome secondChromosome)
        {
            FirstChromosome = firstChromosome;
            SecondChromosome = secondChromosome;
        }

        public CrossoverResult()
        {
        }
    }
}
