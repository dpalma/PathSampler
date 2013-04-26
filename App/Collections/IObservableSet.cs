using System.Collections.Generic;
using System.Collections.Specialized;

namespace PathSampler.Collections
{
   public interface IObservableSet<T> : ISet<T>, INotifyCollectionChanged
   {
   }
}
