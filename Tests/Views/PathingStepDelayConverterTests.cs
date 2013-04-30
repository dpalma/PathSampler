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
      public void TestConvertToDoubleReturnsZeroForMin()
      {
         var converter = new PathingStepDelayConverter();
         var result = converter.Convert(converter.Minimum, typeof(double), null, null);
         Assert.AreEqual(0, result);
      }

      [Test]
      public void TestConvertToDoubleReturnsOneForMax()
      {
         var converter = new PathingStepDelayConverter();
         var result = converter.Convert(converter.Maximum, typeof(double), null, null);
         Assert.AreEqual(1, result);
      }

      [Test]
      public void TestConvertToDoubleReturnsPointFiveForMiddleValue()
      {
         var converter = new PathingStepDelayConverter();
         var middle = TimeSpan.FromMilliseconds((converter.Maximum + converter.Minimum).TotalMilliseconds / 2);
         var result = converter.Convert(middle, typeof(double), null, null);
         Assert.AreEqual(0.5, result);
      }

      [Test]
      public void TestConvertBackReturnsMinForZeroDouble()
      {
         var converter = new PathingStepDelayConverter();
         var result = converter.ConvertBack(0.0, typeof(TimeSpan), null, null);
         Assert.AreEqual(converter.Minimum, result);
      }

      [Test]
      public void TestConvertBackReturnsMaxForOneDouble()
      {
         var converter = new PathingStepDelayConverter();
         var result = converter.ConvertBack(1.0, typeof(TimeSpan), null, null);
         Assert.AreEqual(converter.Maximum, result);
      }

      [Test]
      public void TestConvertBackReturnsMiddleForPointFiveDouble()
      {
         var converter = new PathingStepDelayConverter();
         converter.Minimum = TimeSpan.FromMinutes(0);
         converter.Maximum = TimeSpan.FromMinutes(30);
         var result = converter.ConvertBack(0.5, typeof(TimeSpan), null, null);
         Assert.AreEqual(TimeSpan.FromMinutes(15), result);
      }
   }
}
