using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Core;
using PathFind.Models;
using PathFind.ViewModels;

namespace PathFindTests.ViewModels
{
   class MapVMTests
   {
      [Test]
      public void TestMapChangeTriggersRedrawRequest()
      {
         Map map = new Map();
         MapVM vm = new MapVM();
         var redrawRequests = new List<EventArgs>();
         vm.RedrawRequested += (sender, eventArgs) =>
            {
               redrawRequests.Add(eventArgs);
            };
         vm.Map = map;
         GridCoordinate cell = new GridCoordinate() { Row = 1, Column = 1 };
         map.BlockedCells[cell] = 1;
         Assert.AreEqual(1, redrawRequests.Count);
      }
   }
}
