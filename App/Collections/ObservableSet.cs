using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PathFind.Collections
{
   public class ObservableSet<T> : IObservableSet<T>
   {
      private readonly HashSet<T> m_set = new HashSet<T>();

      #region ISet<T> Members

      public bool Add(T item)
      {
         if (m_set.Add(item))
         {
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            return true;
         }
         return false;
      }

      public void ExceptWith(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      public void IntersectWith(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      public bool IsProperSubsetOf(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      public bool IsProperSupersetOf(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      public bool IsSubsetOf(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      public bool IsSupersetOf(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      public bool Overlaps(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      public bool SetEquals(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      public void SymmetricExceptWith(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      public void UnionWith(IEnumerable<T> other)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region ICollection<T> Members

      void ICollection<T>.Add(T item)
      {
         throw new NotImplementedException();
      }

      public void Clear()
      {
         m_set.Clear();
         FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }

      public bool Contains(T item)
      {
         return m_set.Contains(item);
      }

      public void CopyTo(T[] array, int arrayIndex)
      {
         throw new NotImplementedException();
      }

      public int Count
      {
         get { return m_set.Count; }
      }

      public bool IsReadOnly
      {
         get { throw new NotImplementedException(); }
      }

      public bool Remove(T item)
      {
         if (m_set.Remove(item))
         {
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;
         }
         return false;
      }

      #endregion

      #region IEnumerable<T> Members

      public IEnumerator<T> GetEnumerator()
      {
         return m_set.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         throw new NotImplementedException();
      }

      #endregion

      #region INotifyCollectionChanged implementation

      public event NotifyCollectionChangedEventHandler CollectionChanged;

      private void FireCollectionChanged(NotifyCollectionChangedEventArgs args)
      {
         if (CollectionChanged != null)
         {
            CollectionChanged(this, args);
         }
      }

      #endregion
   }
}
