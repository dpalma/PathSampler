using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using PathFind.Models;
using PathFind.ViewModels;

namespace PathFind.Views
{
   public class MapView : Canvas
   {
      public int GridLineSize
      {
         get
         {
            var vm = DataContext as MapVM;
            return vm.GridLineSize;
         }
      }

      public Size CellSize
      {
         get
         {
            var vm = DataContext as MapVM;
            return vm.CellSize;
         }
      }

      public Size Dimensions
      {
         get
         {
            var vm = DataContext as MapVM;
            return vm.Dimensions;
         }
      }

      public HashSet<GridCoordinate> SelectedCells
      {
         get
         {
            var vm = DataContext as MapVM;
            return vm.SelectedCells;
         }
      }

      WeakReference controllerRef;

      public MapView()
      {
         Initialized += new EventHandler(MapView_Initialized);
      }

      void MapView_Initialized(object sender, EventArgs e)
      {
         // View model will be null in design mode
         MapVM vm = DataContext as MapVM;
         if (vm != null)
         {
            vm.RedrawRequested += new EventHandler(ViewModel_RedrawRequested);
            var controller = new PassabilityController(this, vm);
            controller.Command = vm.SetPassabilityCommand;
            controllerRef = new WeakReference(controller);
         }
      }

      void ViewModel_RedrawRequested(object sender, EventArgs e)
      {
         InvalidateVisual();
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
         base.OnRender(drawingContext);

         drawingContext.DrawRectangle(Brushes.White, null, new Rect(new Size(ActualWidth, ActualHeight)));

         DrawGrid(drawingContext);

         DrawBlockedCells(drawingContext);

         DrawStartAndGoalCells(drawingContext);

         if (SelectedCells != null && SelectedCells.Count > 0)
         {
            DrawSelectedCells(drawingContext);
         }
      }

      private Brush m_gridLineBrush = Brushes.Black;
      public Brush GridLineBrush
      {
         get
         {
            return m_gridLineBrush;
         }
         set
         {
            m_gridLineBrush = value;
         }
      }

      private void DrawGrid(DrawingContext dc)
      {
         Pen linePen = new Pen(GridLineBrush, 0.1);

         // Horizontal grid lines
         for (int i = 0; i <= Dimensions.Height; i++)
         {
            int y = (int)(i * (CellSize.Height + GridLineSize));
            dc.DrawLine(linePen, new Point(0, y), new Point(Width, y));
         }

         // Vertical grid lines
         for (int j = 0; j <= Dimensions.Width; j++)
         {
            int x = (int)(j * (CellSize.Width + GridLineSize));
            dc.DrawLine(linePen, new Point(x, 0), new Point(x, Height));
         }
      }

      private Rect GetCellRect(GridCoordinate cell)
      {
         Point cellPoint = new Point(cell.Column * (CellSize.Width + GridLineSize) + GridLineSize, cell.Row * (CellSize.Height + GridLineSize) + GridLineSize);
         return new Rect(cellPoint, CellSize);
      }

      private void DrawBlockedCells(DrawingContext dc)
      {
         Color blockedColor = Color.FromRgb(0, 0, 0);
         Color unblockedColor = Color.FromRgb(255, 255, 255);

         Brush blockedBrush = new SolidColorBrush(blockedColor);
         Brush unblockedBrush = new SolidColorBrush(unblockedColor);

         var vm = DataContext as MapVM;
         if (vm != null)
         {
            foreach (var cellEntry in vm.Map.BlockedCells)
            {
               dc.DrawRectangle(cellEntry.Value != 0 ? blockedBrush : unblockedBrush, null, GetCellRect(cellEntry.Key));
            }
         }
      }

      private Brush m_startCellBrush = Brushes.Green;
      public Brush StartCellBrush
      {
         get
         {
            return m_startCellBrush;
         }
         set
         {
            m_startCellBrush = value;
         }
      }

      private Brush m_goalCellBrush = Brushes.Red;
      public Brush GoalCellBrush
      {
         get
         {
            return m_goalCellBrush;
         }
         set
         {
            m_goalCellBrush = value;
         }
      }

      private void DrawStartAndGoalCells(DrawingContext dc)
      {
         var vm = DataContext as MapVM;
         if (vm != null)
         {
            if (vm.Map.Goal != null)
            {
               DrawCellEllipse(dc, vm.Map.Goal, GoalCellBrush);
            }

            if (vm.Map.Start != null)
            {
               DrawCellEllipse(dc, vm.Map.Start, StartCellBrush);
            }
         }
      }

      private static Point RectCenter(Rect rect)
      {
         return new Point((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2);
      }

      private void DrawCellEllipse(DrawingContext dc, GridCoordinate cell, Brush brush, Pen pen = null)
      {
         Rect cellRect = GetCellRect(cell);
         dc.DrawEllipse(brush, pen, RectCenter(cellRect), cellRect.Width / 2, cellRect.Height / 2);
      }

      private Brush m_selectedCellBrush = Brushes.Tan;
      public Brush SelectedCellBrush
      {
         get
         {
            return m_selectedCellBrush;
         }
         set
         {
            m_selectedCellBrush = value;
         }
      }

      private void DrawSelectedCells(DrawingContext dc)
      {
         foreach (var cell in SelectedCells)
         {
            dc.DrawRectangle(SelectedCellBrush, null, GetCellRect(cell));
         }
      }
   }
}
