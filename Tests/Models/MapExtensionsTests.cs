using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Models;

namespace PathFindTests.Models
{
   class MapExtensionsTests
   {
      private Map map;

      [SetUp]
      public void SetUp()
      {
         map = new Map();
      }

      [Test]
      public void TestGetCenter()
      {
         Assert.AreEqual(map.RowCount / 2, map.GetCenter().Row);
         Assert.AreEqual(map.ColumnCount / 2, map.GetCenter().Column);
      }
   }
}
