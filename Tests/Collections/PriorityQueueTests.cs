using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Collections;

namespace PathFindTests.Collections
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
      [ExpectedException(typeof(InvalidOperationException))]
      public void TestDequeueThrowsExceptionForEmptyQueue()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         queue.Dequeue();
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
      [ExpectedException(typeof(InvalidOperationException))]
      public void TestPeekThrowsExceptionForEmptyQueue()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         queue.Peek();
      }

      [Test]
      public void TestGetEnumeratorReturnsAllItems()
      {
         PriorityQueue<int> queue = new PriorityQueue<int>();
         for (int i = 10; i <= 100; i += 10)
         {
            queue.Enqueue(i);
         }
         int itemsEnumerated = 0;
         IEnumerator enumerator = queue.GetEnumerator();
         while (enumerator.MoveNext())
         {
            Assert.IsTrue(((int)enumerator.Current % 10) == 0);
            itemsEnumerated++;
         }
         Assert.AreEqual(queue.Count, itemsEnumerated);
      }
   }
}
