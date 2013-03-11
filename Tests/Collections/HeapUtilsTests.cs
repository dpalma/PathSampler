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

      [Test]
      public void TestMakeHeap()
      {
         List<int> test = new List<int>();
         for (int i = 1; i < 100; ++i)
         {
            test.Add(i);
         }
         Assert.IsFalse(test.IsHeap());
         test.MakeHeap();
         Assert.IsTrue(test.IsHeap());
      }

      [Test]
      public void TestHeapAdd()
      {
         List<int> heap = new List<int> { 16, 14, 10, 8, 7, 9, 3, 2, 4, 1 };
         Assert.IsTrue(heap.IsHeap());
         heap.HeapAdd(15);
         Assert.AreEqual(new List<int> { 16, 15, 10, 8, 14, 9, 3, 2, 4, 1, 7 }, heap);
      }

      [Test]
      public void TestHeapRemove()
      {
         List<int> heap = new List<int> { 16, 14, 10, 8, 7, 9, 3, 2, 4, 1 };
         while (heap.Count > 0)
         {
            int value = heap.HeapRemove();
            if (heap.Count == 0)
               break;
            Assert.IsTrue(value > heap.Max());
            Assert.IsTrue(heap.IsHeap());
         }
      }
   }
}
