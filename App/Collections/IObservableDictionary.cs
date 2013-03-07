using System.Collections.Generic;
using System.Collections.Specialized;

namespace PathFind.Collections
{
   public interface IObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged
   {
   }
}
