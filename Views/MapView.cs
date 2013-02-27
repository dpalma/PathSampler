using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace PathFind.Views
{
   public class MapView : Canvas
   {
      private Typeface typeface = new Typeface("Arial");

      const int GridLineSize = 1;

      public Size MapCellSize = new Size(32, 32); // TODO get from view model

      public MapView()
      {
         MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(MapView_MouseLeftButtonDown);
         MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MapView_MouseLeftButtonUp);
         MouseMove += new System.Windows.Input.MouseEventHandler(MapView_MouseMove);
      }

      void MapView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         Point mouse = e.GetPosition(this);

         double hitX = mouse.X / (MapCellSize.Width + GridLineSize);
         double hitY = mouse.Y / (MapCellSize.Height + GridLineSize);

         System.Diagnostics.Debug.WriteLine("Mouse down on cell ({0}, {1})", (int)hitX, (int)hitY);
      }

      void MapView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
      }

      void MapView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
      {
      }

      protected override void OnRender(DrawingContext drawingContext)
      {
         base.OnRender(drawingContext);

         drawingContext.DrawRectangle(Brushes.White, null, new Rect(new Size(ActualWidth, ActualHeight)));

         DrawGrid(drawingContext);

         FormattedText text = new FormattedText("TODO", System.Globalization.CultureInfo.CurrentCulture,
            System.Windows.FlowDirection.LeftToRight, typeface, 12, Brushes.Black);

         Point p = new Point();
         p.X = (ActualWidth - text.Width) / 2;
         p.Y = (ActualHeight - text.Height) / 2;

         drawingContext.DrawText(text, p);
      }

      private void DrawGrid(DrawingContext dc)
      {
         Size dims = new Size(16, 16); // TODO get from model

         Size totalSize = new Size((MapCellSize.Width + GridLineSize) * dims.Width, (MapCellSize.Height + GridLineSize) * dims.Height);

         Pen linePen = new Pen(Brushes.Black, .1);

         // Horizontal grid lines
         for (int i = 0; i < dims.Height; i++)
         {
            int y = (int)(i * (MapCellSize.Height + GridLineSize) + MapCellSize.Height);
            dc.DrawLine(linePen, new Point(0, y), new Point(totalSize.Width, y));
         }

         // Vertical grid lines
         for (int j = 0; j < dims.Width; j++)
         {
            int x = (int)(j * (MapCellSize.Width + GridLineSize) + MapCellSize.Width);
            dc.DrawLine(linePen, new Point(x, 0), new Point(x, totalSize.Height));
         }
      }
   }
}
