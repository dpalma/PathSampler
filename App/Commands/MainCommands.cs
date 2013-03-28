using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PathFind.Commands
{
   public static class MainCommands
   {
      static MainCommands()
      {
         exitCommand = new RoutedCommand("Exit", typeof(MainCommands));
      }

      public static RoutedCommand Exit
      {
         get { return exitCommand; }
      }

      static RoutedCommand exitCommand;
   }
}
