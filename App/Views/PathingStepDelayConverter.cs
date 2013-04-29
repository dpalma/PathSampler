using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PathSampler.Views
{
   public class PathingStepDelayConverter : TypeConverter
   {
      public TimeSpan Minimum;
      public TimeSpan Maximum;

      public PathingStepDelayConverter()
      {
         Minimum = TimeSpan.Zero;
         Maximum = TimeSpan.FromMilliseconds(1000);
      }

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
         if (destinationType.Equals(typeof(double)))
         {
            return true;
         }
         return base.CanConvertTo(context, destinationType);
      }

      public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
      {
         if (value is TimeSpan)
         {
            TimeSpan timeSpan = (TimeSpan)value;
            if (destinationType.Equals(typeof(double)))
            {
               return (timeSpan - Minimum).TotalMilliseconds / (Maximum - Minimum).TotalMilliseconds;
            }
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }

      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
         if (sourceType.Equals(typeof(double)))
         {
            return true;
         }
         return base.CanConvertFrom(context, sourceType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
      {
         if (value is double)
         {
            var doubleValue = (double)value;
            return Minimum.Add(TimeSpan.FromMilliseconds(Maximum.Subtract(Minimum).TotalMilliseconds * doubleValue));
         }
         return base.ConvertFrom(context, culture, value);
      }
   }
}
