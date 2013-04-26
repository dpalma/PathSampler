using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathSampler.Core;

namespace PathSamplerTests.Core
{
   class GridCoordinateTests
   {
      [Test]
      public void TestHashCodesAreDifferentIfRowColumnTransposed()
      {
         GridCoordinate coord1 = new GridCoordinate() { Row = 9, Column = 3 };
         GridCoordinate coord2 = new GridCoordinate() { Row = 3, Column = 9 };
         Assert.AreNotEqual(coord1.GetHashCode(), coord2.GetHashCode());
      }

      [Test]
      public void TestEquality()
      {
         GridCoordinate coord1 = new GridCoordinate() { Row = 2, Column = 7 };
         GridCoordinate coord2 = new GridCoordinate() { Row = 2, Column = 7 };
         Assert.AreNotSame(coord1, coord2);
         Assert.AreEqual(coord1, coord2);
      }
   }
}
