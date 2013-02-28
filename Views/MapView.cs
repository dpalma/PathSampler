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
      private int m_gridLineSize = 1;
      public int GridLineSize
      {
         get { return m_gridLineSize; }
         set { m_gridLineSize = value; }
      }

      private Size m_cellSize = new Size(16, 16);
      public Size CellSize
      {
         get { return m_cellSize; }
         set { m_cellSize = value; }
      }

      public Size Dimensions
      {
         get
         {
            var vm = DataContext as MapVM;
            return vm.Dimensions;
         }
      }

      public MapView()
      {
         MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(MapView_MouseLeftButtonDown);
         MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MapView_MouseLeftButtonUp);
         MouseMove += new System.Windows.Input.MouseEventHandler(MapView_MouseMove);
         LostMouseCapture += new System.Windows.Input.MouseEventHandler(MapView_LostMouseCapture);

         Initialized += new EventHandler(MapView_Initialized);
      }

      void MapView_Initialized(object sender, EventArgs e)
      {
         Width = (CellSize.Width + GridLineSize) * Dimensions.Width;
         Height = (CellSize.Height + GridLineSize) * Dimensions.Height;
      }

      private bool m_mouseDragging = false;
      public bool MouseDragging
      {
         get { return m_mouseDragging; }
         private set
         {
            m_mouseDragging = value;
            if (!m_mouseDragging)
            {
               m_selectedCells.Clear();
               InvalidateVisual();
            }
         }
      }

      private HashSet<GridCoordinate> m_selectedCells = new HashSet<GridCoordinate>();

      void MapView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         if (CaptureMouse())
         {
            HitTestAndAddSelected(e);
            MouseDragging = true;
         }
      }

      void MapView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         if (MouseDragging)
         {
            var vm = DataContext as MapVM;
            if (vm != null)
            {
               vm.SetPassability(m_selectedCells, 1);
            }

            ReleaseMouseCapture();
            MouseDragging = false;
         }
      }

      private GridCoordinate GetHitCell(System.Windows.Input.MouseEventArgs mouseEventArgs)
      {
         Point mouse = mouseEventArgs.GetPosition(this);

         double hitX = mouse.X / (CellSize.Width + GridLineSize);
         double hitY = mouse.Y / (CellSize.Height + GridLineSize);

         return new GridCoordinate() { Column = (int)hitX, Row = (int)hitY };
      }

      private void HitTestAndAddSelected(System.Windows.Input.MouseEventArgs mouseEventArgs)
      {
         GridCoordinate hitCell = GetHitCell(mouseEventArgs);

         if (m_selectedCells.Add(hitCell))
         {
            InvalidateVisual();
         }
      }

      void MapView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
      {
         if (MouseDragging)
         {
            HitTestAndAddSelected(e);
         }
      }

      void MapView_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
      {
         m_mouseDragging = false;
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
         base.OnRender(drawingContext);

         drawingContext.DrawRectangle(Brushes.White, null, new Rect(new Size(ActualWidth, ActualHeight)));

         DrawGrid(drawingContext);

         DrawBlockedCells(drawingContext);

         if (m_selectedCells != null && m_selectedCells.Count > 0)
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

      private void DrawSelectedCells(DrawingContext dc)
      {
         foreach (var cell in m_selectedCells)
         {
            dc.DrawRectangle(Brushes.Tan, null, GetCellRect(cell));
         }
      }
   }
}
