using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace PathFind.Views
{
   public class DisplayNameConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value is Type && targetType.Equals(typeof(string)))
         {
            Type t = value as Type;

            object[] attributes = t.GetCustomAttributes(typeof(DisplayNameAttribute), false);

            if (attributes.Length == 0)
            {
               return t.Name;
            }
            else
            {
               DisplayNameAttribute displayName = attributes.Single() as DisplayNameAttribute;

               if (displayName != null)
               {
                  return displayName.DisplayName;
               }
            }
         }

         return value;
      }
      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         return value;
      }
   }
}
