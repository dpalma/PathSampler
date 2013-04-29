using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PathSampler.Views;

namespace PathSamplerTests.Views
{
   class PathingStepDelayConverterTests
   {
      [Test]
      public void TestCanConvertToReturnsTrueForDouble()
      {
         var converter = new PathingStepDelayConverter();
         Assert.IsTrue(converter.CanConvertTo(typeof(double)));
      }

      [Test]
      public void TestConvertToDoubleReturnsZeroForMin()
      {
         var converter = new PathingStepDelayConverter();
         var result = converter.ConvertTo(converter.Minimum, typeof(double));
         Assert.AreEqual(0, result);
      }

      [Test]
      public void TestConvertToDoubleReturnsOneForMax()
      {
         var converter = new PathingStepDelayConverter();
         var result = converter.ConvertTo(converter.Maximum, typeof(double));
         Assert.AreEqual(1, result);
      }

      [Test]
      public void TestConvertToDoubleReturnsPointFiveForMiddleValue()
      {
         var converter = new PathingStepDelayConverter();
         var middle = TimeSpan.FromMilliseconds((converter.Maximum + converter.Minimum).TotalMilliseconds / 2);
         var result = converter.ConvertTo(middle, typeof(double));
         Assert.AreEqual(0.5, result);
      }

      [Test]
      public void TestCanConvertFromReturnsTrueForDouble()
      {
         var converter = new PathingStepDelayConverter();
         Assert.IsTrue(converter.CanConvertFrom(typeof(double)));
      }

      [Test]
      public void TestConvertFromReturnsMinForZeroDouble()
      {
         var converter = new PathingStepDelayConverter();
         var result = converter.ConvertFrom(0.0);
         Assert.AreEqual(converter.Minimum, result);
      }

      [Test]
      public void TestConvertFromReturnsMaxForOneDouble()
      {
         var converter = new PathingStepDelayConverter();
         var result = converter.ConvertFrom(1.0);
         Assert.AreEqual(converter.Maximum, result);
      }

      [Test]
      public void TestConvertFromReturnsMiddleForPointFiveDouble()
      {
         var converter = new PathingStepDelayConverter();
         converter.Maximum = TimeSpan.FromMinutes(30);
         var result = converter.ConvertFrom(0.5);
         Assert.AreEqual(TimeSpan.FromMinutes(15), result);
      }
   }
}
