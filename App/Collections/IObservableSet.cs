using System.Collections.Generic;
using System.Collections.Specialized;

namespace PathFind.Collections
{
   public interface IObservableSet<T> : ISet<T>, INotifyCollectionChanged
   {
   }
}
