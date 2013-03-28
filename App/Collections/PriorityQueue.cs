using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFind.Collections
{
   public class PriorityQueue<T> : IEnumerable<T>, ICollection where T : IComparable<T>
   {
      private List<T> m_list = new List<T>();

      public void Enqueue(T value)
      {
         m_list.HeapAdd(value);
      }

      public T Dequeue()
      {
         if (m_list.Count == 0)
         {
            throw new InvalidOperationException("Queue is empty");
         }
         return m_list.HeapRemove();
      }

      public T Peek()
      {
         if (m_list.Count == 0)
         {
            throw new InvalidOperationException("Queue is empty");
         }
         return m_list[0];
      }

      public bool Contains(T value)
      {
         return m_list.Contains(value);
      }

      public bool Remove(T value)
      {
         int index = m_list.IndexOf(value);
         if (index >= 0)
         {
            m_list.HeapRemoveAt(index);
            return true;
         }
         return false;
      }

      #region ICollection Members

      public void CopyTo(Array array, int index)
      {
         throw new NotImplementedException();
      }

      public int Count
      {
         get { return m_list.Count; }
      }

      public bool IsSynchronized
      {
         get { return false; }
      }

      public object SyncRoot
      {
         get { throw new NotImplementedException(); }
      }

      #endregion

      #region IEnumerable Members

      IEnumerator IEnumerable.GetEnumerator()
      {
         throw new NotImplementedException();
      }

      #endregion

      #region IEnumerable<T> Members

      IEnumerator<T> IEnumerable<T>.GetEnumerator()
      {
         return m_list.GetEnumerator();
      }

      #endregion
   }
}
