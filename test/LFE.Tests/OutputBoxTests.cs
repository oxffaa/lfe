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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LFE.Tests
{
    [TestClass]
    public class OutputBoxTests
    {
        private static int[] _testData;
        private List<int> _testDataList; 
        private OutputBox<int> _box;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            _testData = Enumerable.Range(0, 100).ToArray();
        }

        [TestInitialize]
        public void TestSetUp()
        {
            _box = new OutputBox<int>(_testData);
            _testDataList = new List<int>(_testData);
        }

        [TestMethod]
        public void SingleThreadTakingTest()
        {
            while (_box.HasItems)
                _testDataList.Remove(_box.Take());

            Assert.AreEqual(0, _testDataList.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SingleThreadTakingEmptyTest()
        {
            while (true)
            {
                _box.Take();
            }
        }

        [TestMethod]
        public void MultiThreadTakingTest()
        {
            var takingItems = new ConcurrentStack<int>();

            var action = new Action(
                delegate
                    {
                        try
                        {
                            while (_box.HasItems)
                            {
                                var ignored = _box.Take();
                                takingItems.Push(ignored);
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // empty
                        }
                    });
            
            Parallel.Invoke(new[] {action, action, action, action});

            foreach (var takingItem in takingItems)
            {
                _testDataList.Remove(takingItem);
            }

            Assert.AreEqual(0, _testDataList.Count);
        }
    }
}