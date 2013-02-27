using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathFind.Models
{
   public class Map : INotifyPropertyChanged
   {
      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion
   }
}
