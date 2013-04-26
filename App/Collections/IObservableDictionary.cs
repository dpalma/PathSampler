using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PathSampler.Collections
{
   public interface IObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
   {
   }
}
