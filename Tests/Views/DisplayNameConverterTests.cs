using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathFind.Views;
using PathFind.PathFinders;

namespace PathFindTests.Views
{
   class DisplayNameConverterTests
   {
      [Test]
      public void TestConverterReturnsValueOfDisplayNameAttribute()
      {
         var converter = new DisplayNameConverter();
         var displayName = converter.Convert(typeof(AStar), typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);
         Assert.IsInstanceOf(typeof(string), displayName);
         Assert.AreEqual("A*", displayName);
      }
   }
}
