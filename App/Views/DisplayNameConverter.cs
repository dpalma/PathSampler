using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace PathSampler.Views
{
   public class DisplayNameConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value is Type && targetType.Equals(typeof(string)))
         {
            Type t = value as Type;

            return PathSampler.PathFinders.PathFinder.GetDisplayName(t);
         }

         return value;
      }
      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return value;
      }
   }
}
