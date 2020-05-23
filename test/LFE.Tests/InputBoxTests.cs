using System;
using System.Collections.Concurrent.System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LFE.Tests
{
    public class InputBoxTests
    {
        private readonly Random _rnd;
        private readonly IEnumerable<int> _data;
        private readonly int _count;
        private readonly int _result;

        public InputBoxTests()
        {
            _rnd = new Random((int) DateTime.UtcNow.Ticks);
            _data = Enumerable.Range(0, 1000).ToArray();
            
            _count = _data.Count();
            _result = _data.Sum();
        }

        [Fact]
        public void Test()
        {
            var box = new InputBox<int>();

            var readerTask = Task<int>.Run(() =>
            {
                int readed = 0;
                int result = 0;
                do
                {
                    foreach (var it in box.TakeAll())
                    {
                        ++readed;
                        result += it;
                    }
                } while (readed != _count);

                return result;
            });

            Parallel.ForEach(_data, i =>
            {
                while (!box.TryAdd(i))
                {
                    Thread.Sleep(_rnd.Next(0, 1000));
                }
            });

            Task.WaitAll(readerTask);
            
            Assert.Equal(_result, readerTask.Result);
        }
    }
}