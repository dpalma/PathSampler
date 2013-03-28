using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using PathFind.ViewModels;

namespace PathFind.Views
{
   public class MapCanvas : Canvas
   {
      public MapCanvas()
      {
         GridLineBrush = Brushes.Black;
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
         base.OnRender(drawingContext);

         DrawGrid(drawingContext);
      }

      public Brush GridLineBrush { get; set; }

      private void DrawGrid(DrawingContext dc)
      {
         MapVM vm = DataContext as MapVM;
         if (vm == null)
            return;

         Size horizontalGridLineSize = new Size(ActualWidth, vm.GridLineSize);
         Size verticalGridLineSize = new Size(vm.GridLineSize, ActualHeight);

         // Horizontal grid lines
         for (int i = 0; i <= vm.Map.RowCount; i++)
         {
            int y = (int)(i * (vm.CellSize.Height + vm.GridLineSize));
            Rect gridLineRect = new Rect(new Point(0, y), horizontalGridLineSize);
            dc.DrawRectangle(GridLineBrush, null, gridLineRect);
         }

         // Vertical grid lines
         for (int j = 0; j <= vm.Map.ColumnCount; j++)
         {
            int x = (int)(j * (vm.CellSize.Width + vm.GridLineSize));
            Rect gridLineRect = new Rect(new Point(x, 0), verticalGridLineSize);
            dc.DrawRectangle(GridLineBrush, null, gridLineRect);
         }
      }

      protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
      {
         var contentPresenter = visualAdded as ContentPresenter;
         if (contentPresenter != null)
         {
            CellVM vm = (CellVM)contentPresenter.DataContext;
            if (vm != null)
            {
               contentPresenter.SetBinding(Canvas.LeftProperty, new Binding("CellPoint.X"));
               contentPresenter.SetBinding(Canvas.TopProperty, new Binding("CellPoint.Y"));
               contentPresenter.SetBinding(WidthProperty, new Binding("MapVM.CellSizeScalar"));
               contentPresenter.SetBinding(HeightProperty, new Binding("MapVM.CellSizeScalar"));
            }
         }

         base.OnVisualChildrenChanged(visualAdded, visualRemoved);
      }
   }
}
