namespace Mission_Service.Config
{
    public class AssignmentAlgorithmConfiguration
    {
        public int PopulationSize { get; set; }
        public int MaxGenerations { get; set; }
        public double CrossoverProbability { get; set; }
        public double MutationProbability { get; set; }
        public double ElitePrecentage { get; set; }
        public int TornumentSize { get; set; }
        public int StagnationLimit { get; set; }

        public AssignmentAlgorithmConfiguration(
            int populationSize,
            int maxGenerations,
            double crossoverProbability,
            double mutationProbability,
            double elitePrecentage,
            int tornumentSize,
            int stagnationLimit
        )
        {
            PopulationSize = populationSize;
            MaxGenerations = maxGenerations;
            CrossoverProbability = crossoverProbability;
            MutationProbability = mutationProbability;
            ElitePrecentage = elitePrecentage;
            TornumentSize = tornumentSize;
            StagnationLimit = stagnationLimit;
        }

        public AssignmentAlgorithmConfiguration() { }
    }
}
