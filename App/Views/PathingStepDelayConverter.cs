using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace PathSampler.Views
{
   public class PathingStepDelayConverter : IValueConverter
   {
      public TimeSpan Minimum;
      public TimeSpan Maximum;

      public PathingStepDelayConverter()
      {
         Minimum = TimeSpan.Zero;
         Maximum = TimeSpan.FromMilliseconds(1000);
      }

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value is TimeSpan)
         {
            TimeSpan timeSpan = (TimeSpan)value;
            if (targetType.Equals(typeof(double)))
            {
               return (timeSpan - Minimum).TotalMilliseconds / (Maximum - Minimum).TotalMilliseconds;
            }
         }
         return value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value is double)
         {
            var doubleValue = (double)value;
            return Minimum.Add(TimeSpan.FromMilliseconds(Maximum.Subtract(Minimum).TotalMilliseconds * doubleValue));
         }
         return value;
      }
   }
}
