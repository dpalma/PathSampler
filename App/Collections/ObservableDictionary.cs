using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PathFind.Collections
{
   [Serializable]
   public class ObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
   {
      private Dictionary<TKey, TValue> m_dictionary = new Dictionary<TKey, TValue>();

      public TValue this[TKey key]
      {
         get
         {
            return m_dictionary[key];
         }
         set
         {
            NotifyCollectionChangedEventArgs notifyArgs = null;
            if (m_dictionary.ContainsKey(key))
            {
               notifyArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, key, key);
            }
            else
            {
               notifyArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, key);
            }
            m_dictionary[key] = value;
            FireCollectionChanged(notifyArgs);
         }
      }

      public void Add(TKey key, TValue value)
      {
         m_dictionary.Add(key, value);
         FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, key));
      }

      public bool ContainsKey(TKey key)
      {
         return m_dictionary.ContainsKey(key);
      }

      public ICollection<TKey> Keys
      {
         get
         {
            return m_dictionary.Keys;
         }
      }

      public bool Remove(TKey key)
      {
         if (m_dictionary.Remove(key))
         {
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, key));
            return true;
         }
         return false;
      }

      public bool TryGetValue(TKey key, out TValue value)
      {
         return m_dictionary.TryGetValue(key, out value);
      }

      public ICollection<TValue> Values
      {
         get
         {
            return m_dictionary.Values;
         }
      }

      public void Add(KeyValuePair<TKey, TValue> item)
      {
         throw new NotImplementedException();
      }

      public void Clear()
      {
         m_dictionary.Clear();
         FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }

      public bool Contains(KeyValuePair<TKey, TValue> item)
      {
         throw new NotImplementedException();
      }

      public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
      {
         throw new NotImplementedException();
      }

      public int Count
      {
         get
         {
            return m_dictionary.Count;
         }
      }

      public bool IsReadOnly
      {
         get { throw new NotImplementedException(); }
      }

      public bool Remove(KeyValuePair<TKey, TValue> item)
      {
         throw new NotImplementedException();
      }

      public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
      {
         return m_dictionary.GetEnumerator();
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         throw new NotImplementedException();
      }

      #region INotifyCollectionChanged Implementation

      public event NotifyCollectionChangedEventHandler CollectionChanged;

      private void FireCollectionChanged(NotifyCollectionChangedEventArgs args)
      {
         if (args.Action == NotifyCollectionChangedAction.Add
            || args.Action == NotifyCollectionChangedAction.Remove
            || args.Action == NotifyCollectionChangedAction.Reset)
         {
            FirePropertyChanged("Count");
            FirePropertyChanged(Binding.IndexerName);
         }
         else if (args.Action == NotifyCollectionChangedAction.Replace)
         {
            FirePropertyChanged(Binding.IndexerName);
         }

         if (CollectionChanged != null)
         {
            CollectionChanged(this, args);
         }
      }

      #endregion

      #region INotifyPropertyChanged Implementation

      public event PropertyChangedEventHandler PropertyChanged;

      private void FirePropertyChanged(string propertyName)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      #endregion
   }
}
