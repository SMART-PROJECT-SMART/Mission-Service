namespace Mission_Service.Models.choromosomes
{
    public class AssignmentChromosome
    {
        private List<AssignmentGene>? _cachedAssignmentsList;

        public IEnumerable<AssignmentGene> Assignments { get; set; }
        public double FitnessScore { get; set; }
        public bool IsValid { get; set; }

        public List<AssignmentGene> AssignmentsList
        {
            get
            {
                if (
                    _cachedAssignmentsList == null
                    || !ReferenceEquals(_cachedAssignmentsList, Assignments)
                )
                {
                    _cachedAssignmentsList =
                        Assignments as List<AssignmentGene> ?? Assignments.ToList();
                }
                return _cachedAssignmentsList;
            }
        }

        public int AssignmentCount => AssignmentsList.Count;

        public AssignmentChromosome Clone()
        {
            return new AssignmentChromosome
            {
                Assignments = AssignmentsList.Select(gene => gene.Clone()).ToList(),
                FitnessScore = FitnessScore,
                IsValid = IsValid
            };
        }
    }
}
