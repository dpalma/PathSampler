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

      WeakReference controller;

      public MapView()
      {
         controller = new WeakReference(new PassabilityController(this));
      }

      public GridCoordinate GetHitCell(System.Windows.Input.MouseEventArgs mouseEventArgs)
      {
         Point mouse = mouseEventArgs.GetPosition(this);

         double hitX = mouse.X / (CellSize.Width + GridLineSize);
         double hitY = mouse.Y / (CellSize.Height + GridLineSize);

         return new GridCoordinate() { Column = (int)hitX, Row = (int)hitY };
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
         base.OnRender(drawingContext);

         drawingContext.DrawRectangle(Brushes.White, null, new Rect(new Size(ActualWidth, ActualHeight)));

         DrawGrid(drawingContext);

         DrawBlockedCells(drawingContext);

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
