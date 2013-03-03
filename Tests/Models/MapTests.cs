using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Models;

namespace PathFindTests.Models
{
   class MapTests
   {
      [Test]
      public void TestTopLeftCellHasOnlyThreeNeighbors()
      {
         Map map = new Map();
         GridCoordinate[] neighbors = map.GetNeighbors(new GridCoordinate());
         Assert.AreEqual(3, neighbors.Length);
      }
   }
}
