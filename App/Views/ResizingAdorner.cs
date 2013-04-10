using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using PathFind.Models;
using PathFind.ViewModels;

namespace PathFind.Views
{
   public class ResizingAdorner : Adorner
   {
      private readonly Thumb m_bottomRight;

      private readonly VisualCollection m_visualChildren;

      public ResizingAdorner(UIElement adornedElement)
         : base(adornedElement)
      {
         m_visualChildren = new VisualCollection(this);

         m_bottomRight = BuildAdornerCorner(Cursors.SizeNWSE);

         m_bottomRight.DragDelta += new DragDeltaEventHandler(BottomRight_DragDelta);
         m_bottomRight.DragStarted += new DragStartedEventHandler(BottomRight_DragStarted);

         MapView mapView = AdornedElement as MapView;
         this.DataContext = mapView.DataContext;
         this.SetBinding(MapProperty, "Map");
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

      private Size m_cellSize;
      private int m_gridLineSize;

      void BottomRight_DragStarted(object sender, DragStartedEventArgs e)
      {
         m_mousePositionAtDragStart = Mouse.GetPosition(AdornedElement);

         m_rowCountAtDragStart = Map.RowCount;
         m_columnCountAtDragStart = Map.ColumnCount;

         MapView mapView = AdornedElement as MapView;
         MapVM mapVM = mapView.DataContext as MapVM;
         m_cellSize = mapVM.CellSize;
         m_gridLineSize = mapVM.GridLineSize;
      }

      void BottomRight_DragDelta(object sender, DragDeltaEventArgs args)
      {
         Point mousePosition = Mouse.GetPosition(AdornedElement);

         Vector displacement = mousePosition - m_mousePositionAtDragStart;

         int addRows = (int)(displacement.Y / (m_cellSize.Height + m_gridLineSize));
         int addCols = (int)(displacement.X / (m_cellSize.Width + m_gridLineSize));

         int newRowCount = m_rowCountAtDragStart + addRows;
         int newColumnCount = m_columnCountAtDragStart + addCols;

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

         return finalSize;
      }

      private Thumb BuildAdornerCorner(Cursor customizedCursor)
      {
         Thumb cornerThumb = new Thumb();

         cornerThumb.Cursor = customizedCursor;
         cornerThumb.Height = 10;
         cornerThumb.Width = 10;
         cornerThumb.Opacity = 0.40;
         cornerThumb.Background = Brushes.Black;

         m_visualChildren.Add(cornerThumb);

         return cornerThumb;
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
