using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathSampler.Collections;
using System.Collections.Specialized;

namespace PathSamplerTests.Collections
{
   class ObservableSetTests
   {
      private ObservableSet<string> set;

      [SetUp]
      public void SetUp()
      {
         set = new ObservableSet<string>();
      }

      [Test]
      public void TestAddUpdatesCount()
      {
         set.Add("test");
         Assert.AreEqual(1, set.Count);
      }

      [Test]
      public void TestAddFiresAddEvent()
      {
         var eventArgsReceived = new List<NotifyCollectionChangedEventArgs>();
         set.CollectionChanged += (sender, eventArgs) =>
         {
            eventArgsReceived.Add(eventArgs);
         };
         set.Add("test");
         Assert.AreEqual(1, eventArgsReceived.Count);
         Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgsReceived.First().Action);
      }

      [Test]
      public void TestContainsReturnsTrueForExistingItems()
      {
         set.Add("Item");
         Assert.IsTrue(set.Contains("Item"));
      }

      [Test]
      public void TestContainsReturnsFalseForNonExistentItems()
      {
         Assert.IsFalse(set.Contains("ItemNotFound"));
      }

      [Test]
      public void TestRemoveUpdatesCount()
      {
         set.Add("item1");
         set.Add("item2");
         Assert.AreEqual(2, set.Count);
         set.Remove("item1");
         Assert.AreEqual(1, set.Count);
      }

      [Test]
      public void TestRemoveReturnsFalseWhenItemNotFound()
      {
         Assert.IsFalse(set.Remove("ItemNotFound"));
      }

      [Test]
      public void TestRemoveFiresRemoveEvent()
      {
         set.Add("item");
         var eventArgsReceived = new List<NotifyCollectionChangedEventArgs>();
         set.CollectionChanged += (sender, eventArgs) =>
         {
            eventArgsReceived.Add(eventArgs);
         };
         set.Remove("item");
         Assert.AreEqual(1, eventArgsReceived.Count);
         Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgsReceived.First().Action);
      }

      [Test]
      public void TestClearUpdatesCount()
      {
         set.Add("item");
         Assert.AreEqual(1, set.Count);
         set.Clear();
         Assert.AreEqual(0, set.Count);
      }

      [Test]
      public void TestClearFiresResetEvent()
      {
         set.Add("item");
         var eventArgsReceived = new List<NotifyCollectionChangedEventArgs>();
         set.CollectionChanged += (sender, eventArgs) =>
         {
            eventArgsReceived.Add(eventArgs);
         };
         set.Clear();
         Assert.AreEqual(1, eventArgsReceived.Count);
         Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsReceived.First().Action);
      }

      [Test]
      public void TestGetEnumeratorReturnsAllItems()
      {
         string[] items = new string[] { "item1", "item2", "item3" };
         foreach (var item in items)
         {
            set.Add(item);
         }
         int index = 0;
         IEnumerator<string> enumerator = set.GetEnumerator();
         while (enumerator.MoveNext())
         {
            Assert.AreEqual(items[index++], enumerator.Current);
         }
      }

      [Test]
      public void TestNonGenericGetEnumeratorReturnsAllItems()
      {
         string[] items = new string[] { "A", "B", "C", "D" };
         foreach (var item in items)
         {
            set.Add(item);
         }
         int index = 0;
         var enumerator = ((System.Collections.IEnumerable)set).GetEnumerator();
         while (enumerator.MoveNext())
         {
            Assert.AreEqual(items[index++], enumerator.Current);
         }
         Assert.AreEqual(items.Length, index);
      }
   }
}
