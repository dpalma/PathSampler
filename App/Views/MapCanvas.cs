using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PathFind.ViewModels;

namespace PathFind.Views
{
   public class MapCanvas : Canvas
   {
      public MapCanvas()
      {
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
         base.OnRender(drawingContext);
      }

      protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
      {
         var contentPresenter = visualAdded as ContentPresenter;
         if (contentPresenter != null)
         {
            CellVM vm = (CellVM)contentPresenter.DataContext;
            if (vm != null)
            {
               Canvas.SetLeft(contentPresenter, vm.CellPoint.X);
               Canvas.SetTop(contentPresenter, vm.CellPoint.Y);
               contentPresenter.Width = vm.CellRect.Width;
               contentPresenter.Height = vm.CellRect.Height;
            }
         }

         base.OnVisualChildrenChanged(visualAdded, visualRemoved);
      }
   }
}
