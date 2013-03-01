using PathFind.Models;
using PathFind.ViewModels;
using System.Windows;

namespace PathFind.Views
{
   class PassabilityController : MouseController
   {
      public PassabilityController(FrameworkElement view, MapVM mapViewModel)
         : base(view, mapViewModel)
      {
      }

      override protected void Finish()
      {
         MapViewModel.SetPassability(MapViewModel.SelectedCells, 1);
      }

      override protected void SelectCell(GridCoordinate cell)
      {
         if (MapViewModel.SelectedCells.Add(cell))
         {
            View.InvalidateVisual();
         }
      }
   }
}
