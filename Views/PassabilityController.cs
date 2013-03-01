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
   }
}
