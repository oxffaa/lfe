using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace LFE.Benchmarks
{
    public class CommonBenchmark
    {
        private const int OPERATION_COUNT = 1000;

        [Benchmark]
        public void InputBoxBenchmark()
        {
            var box = new InputBox<int>();

            Parallel.ForEach(Enumerable.Range(0, OPERATION_COUNT), i => box.Add(i));

            var ignored = box.TakeAll();

            GC.Collect();
        }
    }
}
