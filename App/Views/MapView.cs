﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PathFind.Core;
using PathFind.Models;
using PathFind.ViewModels;

namespace PathFind.Views
{
   public class MapView : Canvas, IMapView
   {
      public int GridLineSize
      {
         get
         {
            return m_gridLineSize;
         }
         set
         {
            m_gridLineSize = value;
         }
      }
      private int m_gridLineSize = 1;

      public Size CellSize
      {
         get
         {
            return m_cellSize;
         }
         set
         {
            m_cellSize = value;
         }
      }
      private Size m_cellSize = new Size(16, 16);

      public double ViewWidth
      {
         get
         {
            return (CellSize.Width + GridLineSize) * ColumnCount;
         }
      }

      public double ViewHeight
      {
         get
         {
            return (CellSize.Height + GridLineSize) * RowCount;
         }
      }

      public static readonly DependencyProperty SelectedCellsProperty = DependencyProperty.Register("SelectedCells", typeof(ICollection<GridCoordinate>), typeof(MapView), new PropertyMetadata(new PropertyChangedCallback(SelectedCells_PropertyChanged)));

      private static void SelectedCells_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
      {
         MapView mapView = d as MapView;
         if (mapView != null)
         {
            ((INotifyCollectionChanged)d.GetValue(e.Property)).CollectionChanged += new NotifyCollectionChangedEventHandler(mapView.SelectedCells_CollectionChanged);
         }
      }

      private void SelectedCells_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         InvalidateVisual();
      }

      public ICollection<GridCoordinate> SelectedCells
      {
         get { return (ICollection<GridCoordinate>)GetValue(SelectedCellsProperty); }
         set { SetValue(SelectedCellsProperty, value); }
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
            var controller = new MouseController(this, this, vm);
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

         DrawColoredCells(drawingContext);

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

      public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register("RowCount", typeof(int), typeof(MapView), new PropertyMetadata(Map.DefaultRowColumnCount));
      public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register("ColumnCount", typeof(int), typeof(MapView), new PropertyMetadata(Map.DefaultRowColumnCount));

      public int RowCount
      {
         get { return (int)GetValue(RowCountProperty); }
         set { SetValue(RowCountProperty, value); }
      }

      public int ColumnCount
      {
         get { return (int)GetValue(ColumnCountProperty); }
         set { SetValue(ColumnCountProperty, value); }
      }

      private void DrawGrid(DrawingContext dc)
      {
         Size horizontalGridLineSize = new Size(Width, GridLineSize);
         Size verticalGridLineSize = new Size(GridLineSize, Height);

         // Horizontal grid lines
         for (int i = 0; i <= RowCount; i++)
         {
            int y = (int)(i * (CellSize.Height + GridLineSize));
            Rect gridLineRect = new Rect(new Point(0, y), horizontalGridLineSize);
            dc.DrawRectangle(GridLineBrush, null, gridLineRect);
         }

         // Vertical grid lines
         for (int j = 0; j <= ColumnCount; j++)
         {
            int x = (int)(j * (CellSize.Width + GridLineSize));
            Rect gridLineRect = new Rect(new Point(x, 0), verticalGridLineSize);
            dc.DrawRectangle(GridLineBrush, null, gridLineRect);
         }
      }

      private Rect GetCellRect(GridCoordinate cell)
      {
         Point cellPoint = new Point(cell.Column * (CellSize.Width + GridLineSize) + GridLineSize, cell.Row * (CellSize.Height + GridLineSize) + GridLineSize);
         return new Rect(cellPoint, CellSize);
      }

      public Color BlockedCellColor
      {
         get
         {
            return m_blockedCellColor;
         }
         set
         {
            m_blockedCellColor = value;
         }
      }
      private Color m_blockedCellColor = Colors.Black;

      public Color UnblockedCellColor
      {
         get
         {
            return m_unblockedCellColor;
         }
         set
         {
            m_unblockedCellColor = value;
         }
      }
      private Color m_unblockedCellColor = Colors.White;

      private void DrawBlockedCells(DrawingContext dc)
      {
         Brush blockedBrush = new SolidColorBrush(BlockedCellColor);
         Brush unblockedBrush = new SolidColorBrush(UnblockedCellColor);

         var vm = DataContext as MapVM;
         if (vm != null)
         {
            foreach (var cellEntry in vm.Map.BlockedCells)
            {
               dc.DrawRectangle(cellEntry.Value != 0 ? blockedBrush : unblockedBrush, null, GetCellRect(cellEntry.Key));
            }
         }
      }

      private void DrawColoredCells(DrawingContext dc)
      {
         MapVM vm = DataContext as MapVM;
         foreach (var entry in vm.ColoredCells)
         {
            dc.DrawRectangle(entry.Value, null, GetCellRect(entry.Key));
         }
      }

      private static readonly DependencyProperty GoalCellBrushProperty = DependencyProperty.Register("GoalCellBrush", typeof(Brush), typeof(MapView));
      private static readonly DependencyProperty StartCellBrushProperty = DependencyProperty.Register("StartCellBrush", typeof(Brush), typeof(MapView));

      public Brush StartCellBrush
      {
         get { return (Brush)GetValue(StartCellBrushProperty); }
         set { SetValue(StartCellBrushProperty, value); }
      }

      public Brush GoalCellBrush
      {
         get { return (Brush)GetValue(GoalCellBrushProperty); }
         set { SetValue(GoalCellBrushProperty, value); }
      }

      private static readonly DependencyProperty GoalCellProperty = DependencyProperty.Register("GoalCell", typeof(GridCoordinate), typeof(MapView));
      private static readonly DependencyProperty StartCellProperty = DependencyProperty.Register("StartCell", typeof(GridCoordinate), typeof(MapView));

      public GridCoordinate StartCell
      {
         get { return (GridCoordinate)GetValue(StartCellProperty); }
         set { SetValue(StartCellProperty, value); }
      }

      public GridCoordinate GoalCell
      {
         get { return (GridCoordinate)GetValue(GoalCellProperty); }
         set { SetValue(GoalCellProperty, value); }
      }

      private void DrawStartAndGoalCells(DrawingContext dc)
      {
         if (GoalCell != null)
         {
            DrawCellEllipse(dc, GoalCell, GoalCellBrush);
         }

         if (StartCell != null)
         {
            DrawCellEllipse(dc, StartCell, StartCellBrush);
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

      private static readonly DependencyProperty SelectedCellBrushProperty = DependencyProperty.Register("SelectedCellBrush", typeof(Brush), typeof(MapView));

      public Brush SelectedCellBrush
      {
         get
         {
            return (Brush)GetValue(SelectedCellBrushProperty);
         }
         set
         {
            SetValue(SelectedCellBrushProperty, value);
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
