using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using PathFind.ViewModels;

namespace PathFind.Views
{
   public class CellTemplateSelector : DataTemplateSelector
   {
      public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
      {
         CellVM cellVM = item as CellVM;
         if (cellVM != null)
         {
            if (cellVM.IsBlocked)
            {
               return BlockedCellTemplate;
            }

            return BaseCellTemplate;
         }

         return base.SelectTemplate(item, container);
      }

      public DataTemplate BaseCellTemplate { get; set; }

      public DataTemplate BlockedCellTemplate { get; set; }
   }
}
