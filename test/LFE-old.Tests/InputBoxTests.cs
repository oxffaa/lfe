/*
Copyright (c) 2012 Artem Akulyakov <akulyakov.artem@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished 
to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in 
   all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
IN THE SOFTWARE.
*/

using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LFE.Tests
{
    [TestClass]
    public class InputBoxTests
    {
        private const int ThreadCount = 4;

        private static int[] _testData;
        private InputBox<int> _box;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            _testData = Enumerable.Range(0, 10000).ToArray();
        }

        [TestInitialize]
        public void TestSetUp()
        {
            _box = new InputBox<int>();
        }

        [TestMethod]
        public void SingleThreadAddingAndTakeTest()
        {
            SingleThreadingTestTemplate(_box.Add);
        }

        [TestMethod]
        public void SingleThreadTryAddingAndTakeTest()
        {
            SingleThreadingTestTemplate(it =>
                                            {
                                                while (!_box.TryAdd(it))
                                                {
                                                    // empty
                                                }
                                            });
        }

        [TestMethod]
        public void MultiThreadAddingAndTakeTest()
        {
            MultiThreadingTestTemplate(_box.Add);
        }

        [TestMethod]
        public void MultiThreadTryAddingAndTakeTest()
        {
            MultiThreadingTestTemplate(it =>
                                           {
                                               while (!_box.TryAdd(it))
                                               {
                                                   // empty
                                               }
                                           });
        }

        private void SingleThreadingTestTemplate(Action<int> action)
        {
            Array.ForEach(_testData, action);

            var result = _box.TakeAll()
                .ToEnumerable()
                .Reverse()
                .ToArray();

            AssertResult(result);
        }

        private void MultiThreadingTestTemplate(Action<int> action)
        {
            _testData.AsParallel()
                .WithDegreeOfParallelism(ThreadCount)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .ForAll(action);

            var result = _box.TakeAll()
                .ToEnumerable()
                .OrderBy(it => it)
                .ToArray();

            AssertResult(result);
        }

        private void AssertResult(int[] result)
        {
            Assert.AreEqual(_testData.Length, result.Length);

            for (var i = 0; i < _testData.Length; i++)
                Assert.AreEqual(_testData[i], result[i]);
        }
    }
}