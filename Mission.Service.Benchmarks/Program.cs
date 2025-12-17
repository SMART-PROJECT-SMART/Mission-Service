using BenchmarkDotNet.Running;

namespace Mission.Service.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowMenu();
                return;
            }

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }

        private static void ShowMenu()
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("  SMART UAV Genetic Algorithm Benchmarks");
            Console.WriteLine("=================================================");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run -c Release -- [options]");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  # Run all benchmarks");
            Console.WriteLine("  dotnet run -c Release");
            Console.WriteLine();
            Console.WriteLine("  # Run only Component benchmarks");
            Console.WriteLine("  dotnet run -c Release -- --filter * --category Component");
            Console.WriteLine();
            Console.WriteLine("  # Run only Fitness benchmarks");
            Console.WriteLine("  dotnet run -c Release -- --filter * --category Fitness");
            Console.WriteLine();
            Console.WriteLine("  # Run specific benchmark");
            Console.WriteLine("  dotnet run -c Release -- --filter *FitnessCalculation*");
            Console.WriteLine();
            Console.WriteLine("  # List all benchmarks");
            Console.WriteLine("  dotnet run -c Release -- --list flat");
            Console.WriteLine();
            Console.WriteLine("Categories:");
            Console.WriteLine("  - Component    : Individual component tests");
            Console.WriteLine("  - Fitness      : Fitness calculation tests");
            Console.WriteLine("  - Crossover    : Crossover operation tests");
            Console.WriteLine("  - Mutation     : Mutation operation tests");
            Console.WriteLine("  - Population   : Population initialization tests");
            Console.WriteLine("  - Repair       : Repair pipeline tests");
            Console.WriteLine("  - Parallel     : Parallel execution tests");
            Console.WriteLine("  - Allocation   : Memory allocation tests");
            Console.WriteLine("  - EndToEnd     : Complete algorithm tests");
            Console.WriteLine();
            Console.WriteLine("=================================================");
        }
    }
}
