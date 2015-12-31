using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathSampler.Collections;

namespace PathSamplerTests.Collections
{
   class PriorityQueueTests
   {
      [Test]
      public void TestEnqueueUpdatesCount()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         queue.Enqueue(99);
         Assert.AreEqual(1, queue.Count);
      }

      [Test]
      public void TestDequeueUpdatesCount()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         queue.Enqueue(99);
         queue.Enqueue(88);
         Assert.AreEqual(2, queue.Count);
         queue.Dequeue();
         Assert.AreEqual(1, queue.Count);
         queue.Dequeue();
         Assert.AreEqual(0, queue.Count);
      }

      [Test]
      public void TestDequeueThrowsExceptionForEmptyQueue()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
      }

      [Test]
      public void TestPeekDoesNotChangeCount()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         queue.Enqueue(1);
         queue.Enqueue(2);
         Assert.AreEqual(2, queue.Peek());
      }

      [Test]
      public void TestPeekThrowsExceptionForEmptyQueue()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         Assert.Throws<InvalidOperationException>(() => queue.Peek());
      }

      [Test]
      public void TestContainsReturnsTrueForItemsPresent()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         queue.Enqueue(50);
         Assert.IsTrue(queue.Contains(50));
      }

      [Test]
      public void TestContainsReturnsFalseForNonExistentItems()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         Assert.IsFalse(queue.Contains(8));
      }

      [Test]
      public void TestGenericGetEnumeratorReturnsAllItems()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         for (int i = 10; i <= 100; i += 10)
         {
            queue.Enqueue(i);
         }
         int itemsEnumerated = 0;
         foreach (var item in queue)
         {
            Assert.IsTrue(((int)item % 10) == 0);
            itemsEnumerated++;
         }
         Assert.AreEqual(queue.Count, itemsEnumerated);
      }

      [Test]
      public void TestRemoveFromMiddleMaintainsHeapCondition()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         for (int i = 1; i <= 20; ++i)
         {
            queue.Enqueue(i);
         }
         queue.Remove(20);
         while (queue.Count > 0)
         {
            int next = queue.Dequeue();
            if (queue.Count > 0)
            {
               Assert.IsTrue(next >= queue.Max());
            }
         }
      }

      [Test]
      public void TestPriorityQueueWorksWithWhere()
      {
         PriorityQueue<Node> queue = new PriorityQueue<Node>();
         queue.Enqueue(new Node() { Priority = 100, Data = "TestNode1" });
         queue.Enqueue(new Node() { Priority = 200, Data = "TestNode2" });
         var result = queue.Where(n => n.Priority == 100);
         Assert.AreEqual(1, result.Count());
         Node node = result.First();
         Assert.AreEqual("TestNode1", node.Data);
      }
   }

   internal class Node : IComparable<Node>
   {
      public int Priority;
      public String Data;

      #region IComparable<Node> Members

      public int CompareTo(Node other)
      {
         return Priority - other.Priority;
      }

      #endregion
   }
}
