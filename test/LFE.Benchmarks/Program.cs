using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace LFE.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BenchmarkRunner.Run<CommonBenchmark>(
                    ManualConfig
                        .Create(DefaultConfig.Instance)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}
