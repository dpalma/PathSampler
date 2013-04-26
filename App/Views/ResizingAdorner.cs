using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using PathSampler.Models;
using PathSampler.ViewModels;

namespace PathSampler.Views
{
   public class ResizingAdorner : Adorner
   {
      private readonly Thumb m_bottomRight;
      private readonly Thumb m_right;
      private readonly Thumb m_bottom;

      private readonly VisualCollection m_visualChildren;

      public ResizingAdorner(UIElement adornedElement)
         : base(adornedElement)
      {
         m_visualChildren = new VisualCollection(this);

         m_bottomRight = BuildAdornerThumb(Cursors.SizeNWSE);
         m_bottomRight.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
         m_bottomRight.DragStarted += new DragStartedEventHandler(BottomRight_DragStarted);

         m_right = BuildAdornerThumb(Cursors.SizeWE);
         m_right.DragStarted += new DragStartedEventHandler(Right_DragStarted);
         m_right.DragDelta += new DragDeltaEventHandler(Right_DragDelta);

         m_bottom = BuildAdornerThumb(Cursors.SizeNS);
         m_bottom.DragStarted += new DragStartedEventHandler(Bottom_DragStarted);
         m_bottom.DragDelta += new DragDeltaEventHandler(Bottom_DragDelta);

         MapView mapView = AdornedElement as MapView;
         this.DataContext = mapView.DataContext;
         this.SetBinding(MapProperty, "Map");
         this.SetBinding(CellSizeProperty, "CellSize");
         this.SetBinding(GridLineSizeProperty, "GridLineSize");
      }

      private static readonly DependencyProperty MapProperty = DependencyProperty.Register("Map", typeof(Map), typeof(ResizingAdorner));

      public Map Map
      {
         get { return (Map)GetValue(MapProperty); }
         set { SetValue(MapProperty, value); }
      }

      private Point m_mousePositionAtDragStart;

      private int m_rowCountAtDragStart;
      private int m_columnCountAtDragStart;

      private static readonly DependencyProperty CellSizeProperty = DependencyProperty.Register("CellSize", typeof(Size), typeof(ResizingAdorner));

      public Size CellSize
      {
         get { return (Size)GetValue(CellSizeProperty); }
         set { SetValue(CellSizeProperty, value); }
      }

      private static readonly DependencyProperty GridLineSizeProperty = DependencyProperty.Register("GridLineSize", typeof(int), typeof(ResizingAdorner));

      public int GridLineSize
      {
         get { return (int)GetValue(GridLineSizeProperty); }
         set { SetValue(GridLineSizeProperty, value); }
      }

      private void sharedDragStart()
      {
         m_mousePositionAtDragStart = Mouse.GetPosition(AdornedElement);
         m_rowCountAtDragStart = Map.RowCount;
         m_columnCountAtDragStart = Map.ColumnCount;
      }

      private void sharedDragDelta(out int newRowCount, out int newColumnCount)
      {
         Point mousePosition = Mouse.GetPosition(AdornedElement);

         Vector displacement = mousePosition - m_mousePositionAtDragStart;

         int addRows = (int)(displacement.Y / (CellSize.Height + GridLineSize));
         int addCols = (int)(displacement.X / (CellSize.Width + GridLineSize));

         newRowCount = m_rowCountAtDragStart + addRows;
         newColumnCount = m_columnCountAtDragStart + addCols;
      }

      void Right_DragStarted(object sender, DragStartedEventArgs e)
      {
         sharedDragStart();
      }

      void Right_DragDelta(object sender, DragDeltaEventArgs args)
      {
         int newRowCount;
         int newColumnCount;
         sharedDragDelta(out newRowCount, out newColumnCount);

         if (newColumnCount > 0)
         {
            Map.ColumnCount = newColumnCount;
         }
      }

      void Bottom_DragStarted(object sender, DragStartedEventArgs e)
      {
         sharedDragStart();
      }

      void Bottom_DragDelta(object sender, DragDeltaEventArgs args)
      {
         int newRowCount;
         int newColumnCount;
         sharedDragDelta(out newRowCount, out newColumnCount);

         if (newRowCount > 0)
         {
            Map.RowCount = newRowCount;
         }
      }

      void BottomRight_DragStarted(object sender, DragStartedEventArgs e)
      {
         sharedDragStart();
      }

      void BottomRight_DragDelta(object sender, DragDeltaEventArgs args)
      {
         int newRowCount;
         int newColumnCount;
         sharedDragDelta(out newRowCount, out newColumnCount);

         if (newRowCount > 0)
         {
            Map.RowCount = newRowCount;
         }

         if (newColumnCount > 0)
         {
            Map.ColumnCount = newColumnCount;
         }
      }

      protected override Size ArrangeOverride(Size finalSize)
      {
         // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
         // These will be used to place the ResizingAdorner at the corners of the adorned element.  
         double desiredWidth = AdornedElement.DesiredSize.Width;
         double desiredHeight = AdornedElement.DesiredSize.Height;

         // adornerWidth & adornerHeight are used for placement as well.
         double adornerWidth = this.DesiredSize.Width;
         double adornerHeight = this.DesiredSize.Height;

         double adornerOffsetH = -adornerWidth / 2;
         double adornerOffsetV = -adornerHeight / 2;

         m_bottomRight.Arrange(new Rect(desiredWidth + adornerOffsetH, desiredHeight + adornerOffsetV, adornerWidth, adornerHeight));

         m_right.Arrange(new Rect(desiredWidth + adornerOffsetH, (desiredHeight / 2) + adornerOffsetV, adornerWidth, adornerHeight));

         m_bottom.Arrange(new Rect((desiredWidth / 2) + adornerOffsetH, desiredHeight + adornerOffsetV, adornerWidth, adornerHeight));

         return finalSize;
      }

      private Thumb BuildAdornerThumb(Cursor cursor)
      {
         Thumb thumb = new Thumb();

         thumb.Cursor = cursor;
         thumb.Height = 8;
         thumb.Width = 8;
         thumb.Opacity = 0.40;
         thumb.Background = Brushes.Black;

         m_visualChildren.Add(thumb);

         return thumb;
      }

      protected override int VisualChildrenCount
      {
         get
         {
            return m_visualChildren.Count;
         }
      }

      protected override Visual GetVisualChild(int index)
      {
         return m_visualChildren[index];
      }
   }
}
