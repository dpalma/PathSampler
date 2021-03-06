﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathSampler.Collections;

namespace PathSamplerTests.Collections
{
   class ObservableDictionaryTests
   {
      [Test]
      public void TestAddUpdatesCount()
      {
         ObservableDictionary<string, object> dict = new ObservableDictionary<string, object>();
         dict.Add("test", new object());
         Assert.AreEqual(1, dict.Count);
      }

      [Test]
      public void TestAddFiresAddEvent()
      {
         ObservableDictionary<string, object> dict = new ObservableDictionary<string, object>();
         var eventArgsReceived = new List<NotifyCollectionChangedEventArgs>();
         dict.CollectionChanged += (sender, eventArgs) =>
         {
            eventArgsReceived.Add(eventArgs);
         };
         dict.Add("test", new object());
         Assert.AreEqual(1, eventArgsReceived.Count);
         Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgsReceived.First().Action);
      }

      [Test]
      public void TestAddPairFiresAddEvent()
      {
         ObservableDictionary<string, object> dict = new ObservableDictionary<string, object>();
         var eventArgsReceived = new List<NotifyCollectionChangedEventArgs>();
         dict.CollectionChanged += (sender, eventArgs) =>
         {
            eventArgsReceived.Add(eventArgs);
         };
         dict.Add(new KeyValuePair<string, object>("Key", new object()));
         Assert.AreEqual(1, eventArgsReceived.Count);
         Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgsReceived.First().Action);
      }

      [Test]
      public void TestContainsKeyReturnsTrueForExistingKeys()
      {
         IDictionary<string, object> dict = new ObservableDictionary<string, object>();
         dict.Add("key", new object());
         Assert.IsTrue(dict.ContainsKey("key"));
      }

      [Test]
      public void TestContainsKeyReturnsFalseForNonExistentKeys()
      {
         IDictionary<string, object> dict = new ObservableDictionary<string, object>();
         Assert.IsFalse(dict.ContainsKey("key"));
      }

      [Test]
      public void TestKeysCollection()
      {
         IDictionary<string, object> dict = new ObservableDictionary<string, object>();
         dict.Add("key1", new object());
         dict.Add("key2", new object());
         Assert.AreEqual(2, dict.Keys.Count);
         Assert.IsTrue(dict.Keys.Contains("key1"));
         Assert.IsTrue(dict.Keys.Contains("key2"));
      }

      [Test]
      public void TestRemoveByKeyUpdatesCount()
      {
         IDictionary<string, object> dict = new ObservableDictionary<string, object>();
         dict.Add("key1", new object());
         dict.Add("key2", new object());
         Assert.AreEqual(2, dict.Count);
         dict.Remove("key1");
         Assert.AreEqual(1, dict.Count);
      }

      [Test]
      public void TestRemoveByKeyReturnsFalseWhenKeyNotFound()
      {
         IDictionary<string, object> dict = new ObservableDictionary<string, object>();
         Assert.IsFalse(dict.Remove("NotFound"));
      }

      [Test]
      public void TestRemoveByKeyFiresRemoveEvent()
      {
         IObservableDictionary<string, object> dict = new ObservableDictionary<string, object>();
         dict.Add("key", new object());
         var eventArgsReceived = new List<NotifyCollectionChangedEventArgs>();
         dict.CollectionChanged += (sender, eventArgs) =>
         {
            eventArgsReceived.Add(eventArgs);
         };
         dict.Remove("key");
         Assert.AreEqual(1, eventArgsReceived.Count);
         Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgsReceived.First().Action);
      }

      [Test]
      public void TestTryGetValue()
      {
         IDictionary<string, object> dict = new ObservableDictionary<string, object>();
         var value = new object();
         dict.Add("key", value);
         object retrievedValue = null;
         Assert.IsTrue(dict.TryGetValue("key", out retrievedValue));
         Assert.AreSame(value, retrievedValue);
      }

      [Test]
      public void TestValuesProperty()
      {
         IDictionary<string, object> dict = new ObservableDictionary<string, object>();
         var value1 = new object();
         var value2 = new object();
         dict.Add("key1", value1);
         dict.Add("key2", value2);
         Assert.AreEqual(2, dict.Values.Count);
         Assert.AreSame(value1, dict.Values.First());
         Assert.AreSame(value2, dict.Values.Skip(1).Single());
      }

      [Test]
      public void TestClearUpdatesCount()
      {
         IDictionary<string, object> dict = new ObservableDictionary<string, object>();
         dict.Add("key", new object());
         Assert.AreEqual(1, dict.Count);
         dict.Clear();
         Assert.AreEqual(0, dict.Count);
      }

      [Test]
      public void TestClearFiresResetEvent()
      {
         IObservableDictionary<string, object> dict = new ObservableDictionary<string, object>();
         dict.Add("key", new object());
         var eventArgsReceived = new List<NotifyCollectionChangedEventArgs>();
         dict.CollectionChanged += (sender, eventArgs) =>
         {
            eventArgsReceived.Add(eventArgs);
         };
         dict.Clear();
         Assert.AreEqual(1, eventArgsReceived.Count);
         Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsReceived.First().Action);
      }

      [Test]
      public void TestIndexerFiresAddEventForNewKeys()
      {
         IObservableDictionary<string, object> dict = new ObservableDictionary<string, object>();
         var eventArgsReceived = new List<NotifyCollectionChangedEventArgs>();
         dict.CollectionChanged += (sender, eventArgs) =>
         {
            eventArgsReceived.Add(eventArgs);
         };
         dict["key"] = new object();
         Assert.AreEqual(1, eventArgsReceived.Count);
         Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgsReceived.First().Action);
      }

      [Test]
      public void TestIndexerFiresReplaceEventForExistingKeys()
      {
         IObservableDictionary<string, object> dict = new ObservableDictionary<string, object>();
         dict["key"] = new object();
         var eventArgsReceived = new List<NotifyCollectionChangedEventArgs>();
         dict.CollectionChanged += (sender, eventArgs) =>
         {
            eventArgsReceived.Add(eventArgs);
         };
         dict["key"] = new object();
         Assert.AreEqual(1, eventArgsReceived.Count);
         Assert.AreEqual(NotifyCollectionChangedAction.Replace, eventArgsReceived.First().Action);
      }

      [Test]
      public void TestExplicitGetEnumeratorReturnsAllPairs()
      {
         var dict = new ObservableDictionary<string, int>();
         for (int i = 0; i < 10; ++i)
            dict[String.Format("Key{0}", i)] = i;
         System.Collections.IEnumerator enumerator = ((System.Collections.IEnumerable)dict).GetEnumerator();
         while (enumerator.MoveNext())
         {
            KeyValuePair<string, int> pair = (KeyValuePair<string, int>)enumerator.Current;
            Assert.AreEqual(String.Format("Key{0}", pair.Value), pair.Key);
         }
      }

      [Test]
      public void TestGetEnumeratorReturnsAllPairs()
      {
         IObservableDictionary<string, object> dict = new ObservableDictionary<string, object>();
         string[] keys = new string[] { "key1", "key2", "key3" };
         foreach (var key in keys)
         {
            dict[key] = new object();
         }
         int index = 0;
         IEnumerator<KeyValuePair<string, object>> enumerator = dict.GetEnumerator();
         while (enumerator.MoveNext())
         {
            Assert.AreEqual(keys[index++], enumerator.Current.Key);
            Assert.IsNotNull(enumerator.Current.Value);
         }
      }

      [Test]
      public void TestAddItemFiresCountPropertyChangedEvent()
      {
         var dict = new ObservableDictionary<string, object>();
         var propertyNamesChanged = new List<string>();
         dict.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
         {
            propertyNamesChanged.Add(e.PropertyName);
         };
         dict["test"] = new object();
         Assert.IsTrue(propertyNamesChanged.Contains("Count"));
      }

      [Test]
      public void TestRemoveItemFiresCountPropertyChangedEvent()
      {
         var dict = new ObservableDictionary<string, object>();
         dict["test"] = new object();
         var propertyNamesChanged = new List<string>();
         dict.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
         {
            propertyNamesChanged.Add(e.PropertyName);
         };
         dict.Remove("test");
         Assert.IsTrue(propertyNamesChanged.Contains("Count"));
      }

      [Test]
      public void TestSetItemFiresIndexerPropertyChangedEvent()
      {
         var dict = new ObservableDictionary<string, object>();
         var propertyNamesChanged = new List<string>();
         dict.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
         {
            propertyNamesChanged.Add(e.PropertyName);
         };
         dict["test"] = new object();
         Assert.IsTrue(propertyNamesChanged.Contains("Item[]"));
      }
   }
}
