using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Collections;

namespace PathFindTests.Collections
{
   class HeapUtilsTests
   {
      [Test]
      [TestCase(8, 4, 7, 1, 3, 5, 2)]
      [TestCase(15, 7, 4, 7, 5, 3, 1, 2, 6)]
      [TestCase(20, 14, 17, 8, 6, 9, 4, 1)]
      public void TestIsHeapReturnsTrueWhenHeapConditionSatisfied(params int[] heapArray)
      {
         List<int> heap = new List<int>(heapArray);
         Assert.IsTrue(heap.IsHeap());
      }

      [Test]
      [TestCase(7, 2, 5, 1, 3, 8, 4)]
      [TestCase(6, 7, 4, 7, 5, 3, 1, 2, 15)]
      [TestCase(20, 14, 4, 8, 6, 9, 17, 1)]
      public void TestIsHeapReturnsFalseWhenHeapConditionNotSatisfied(params int[] heapArray)
      {
         List<int> heap = new List<int>(heapArray);
         Assert.IsFalse(heap.IsHeap());
      }

      [Test]
      public void TestHeapify()
      {
         List<int> test = new List<int>() { 15, 6, 4, 8, 5, 3, 1, 2, 7 };
         test.Heapify(1);
         Assert.AreEqual(new List<int>() { 15, 8, 4, 7, 5, 3, 1, 2, 6 }, test);
      }
   }
}
