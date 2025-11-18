namespace Mission_Service.Config
{
    public class AlgorithmConfig
    {
        public int PopulationSize { get; set; }
        public int MaxGenerations { get; set; }
        public double CrossoverProbability { get; set; }
        public double MutationProbability { get; set; }
        public double ElitePrecentage { get; set; }
        public int TornumentSize { get; set; }
        public int StagnationLimit { get; set; }
    }
}
