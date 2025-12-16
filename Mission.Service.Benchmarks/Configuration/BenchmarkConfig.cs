using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;

namespace Mission.Service.Benchmarks.Configuration
{
    public class SmartBenchmarkConfig : ManualConfig
    {
        public SmartBenchmarkConfig()
        {
            AddJob(Job.Default
                .WithLaunchCount(1)
                .WithWarmupCount(5)
                .WithIterationCount(15)
                .WithGcServer(true)
                .WithGcConcurrent(true)
                .WithGcForce(true)
            );

            AddDiagnoser(MemoryDiagnoser.Default);
            AddDiagnoser(ThreadingDiagnoser.Default);

            AddColumn(StatisticColumn.Mean);
            AddColumn(StatisticColumn.StdDev);
            AddColumn(StatisticColumn.Error);
            AddColumn(StatisticColumn.P95);
            AddColumn(StatisticColumn.Median);
            AddColumn(RankColumn.Arabic);

            AddExporter(MarkdownExporter.GitHub);
            AddExporter(HtmlExporter.Default);
            AddExporter(CsvExporter.Default);

            AddLogger(ConsoleLogger.Default);

            WithOptions(ConfigOptions.DisableOptimizationsValidator);
            WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest));
        }
    }

    public class QuickBenchmarkConfig : ManualConfig
    {
        public QuickBenchmarkConfig()
        {
            AddJob(Job.ShortRun
                .WithGcServer(true)
                .WithIterationCount(3)
                .WithWarmupCount(1)
            );

            AddDiagnoser(MemoryDiagnoser.Default);
            AddExporter(MarkdownExporter.GitHub);
            AddLogger(ConsoleLogger.Default);
        }
    }
}