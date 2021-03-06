﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PathSampler.Core;
using PathSampler.Models;
using PathSampler.ViewModels;

namespace PathSampler.Views
{
   /// <summary>
   /// Interaction logic for MapView.xaml
   /// </summary>
   public partial class MapView : ItemsControl
   {
      public MapView()
      {
         InitializeComponent();
      }

      public static readonly DependencyProperty GridLineSizeProperty = DependencyProperty.Register("GridLineSize", typeof(int), typeof(MapView), new PropertyMetadata(1));

      public int GridLineSize
      {
         get
         {
            return (int)GetValue(GridLineSizeProperty);
         }
         set
         {
            SetValue(GridLineSizeProperty, value);
         }
      }

      public static readonly DependencyProperty CellSizeProperty = DependencyProperty.Register("CellSize", typeof(Size), typeof(MapView), new PropertyMetadata(new Size(16, 16)));

      public Size CellSize
      {
         get
         {
            return (Size)GetValue(CellSizeProperty);
         }
         set
         {
            SetValue(CellSizeProperty, value);
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

      private void ItemsControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {
         // View model will be null in design mode
         MapVM vm = DataContext as MapVM;
         if (vm != null)
         {
            vm.RedrawRequested += new EventHandler(ViewModel_RedrawRequested);
            var controller = new MouseController(this, vm);
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

         if (SelectedCells != null && SelectedCells.Count > 0)
         {
            DrawSelectedCells(drawingContext);
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

      private Rect GetCellRect(GridCoordinate cell)
      {
         Point cellPoint = new Point(cell.Column * (CellSize.Width + GridLineSize) + GridLineSize, cell.Row * (CellSize.Height + GridLineSize) + GridLineSize);
         return new Rect(cellPoint, CellSize);
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

      private static Point RectCenter(Rect rect)
      {
         return new Point((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2);
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
