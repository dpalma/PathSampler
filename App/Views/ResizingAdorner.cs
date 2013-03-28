using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;

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

         m_bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
      }

      void HandleBottomRight(object sender, DragDeltaEventArgs args)
      {
         FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
         Thumb hitThumb = sender as Thumb;

         if (adornedElement == null || hitThumb == null)
            return;
         FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

         //System.Diagnostics.Debug.WriteLine(String.Format("H {0}, V {0}", args.HorizontalChange, args.VerticalChange));

         // Ensure that the Width and Height are properly initialized after the resize.
         EnforceSize(adornedElement);

         // Change the size by the amount the user drags the mouse, as long as it's larger 
         // than the width or height of an adorner, respectively.
         adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
         adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
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

      // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
      // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
      // need to be set first.  It also sets the maximum size of the adorned element.
      void EnforceSize(FrameworkElement adornedElement)
      {
         if (adornedElement.Width.Equals(Double.NaN))
            adornedElement.Width = adornedElement.DesiredSize.Width;
         if (adornedElement.Height.Equals(Double.NaN))
            adornedElement.Height = adornedElement.DesiredSize.Height;

         FrameworkElement parent = adornedElement.Parent as FrameworkElement;
         if (parent != null)
         {
            adornedElement.MaxHeight = parent.ActualHeight;
            adornedElement.MaxWidth = parent.ActualWidth;
         }
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
