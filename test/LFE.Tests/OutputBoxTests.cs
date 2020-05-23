using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LFE.Tests
{
    public class OutputBoxTests
    {
        [Fact]
        public void Test()
        {
            var data = Enumerable.Range(0, 1000).ToArray();
            var sum = data.Sum();
            
            var box = new OutputBox<int>(data);

            var workers =
                Enumerable
                    .Repeat(0, 4)
                    .Select(async (_) =>
                    {
                        int result = 0;
                        
                        while (box.HasItems)
                        {
                            if (box.TryTake(out var item))
                            {
                                result += item;
                            }
                        }

                        return result;
                    })
                    .ToArray();

            Task.WaitAll(workers);
            var result = workers.Sum(w => w.Result);

            Assert.Equal(sum, result);
        }
    }
}