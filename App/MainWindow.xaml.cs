using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PathFind.Models;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PathFind
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      private const string FileExtension = "mapx";

      private static string FileFilter
      {
         get { return String.Format("Map Files (.{0})|*.{0}", FileExtension); }
      }

      private int DefaultFileCounter = 1;

      private string CurrentMapFileName;

      private void OnOpen(object sender, ExecutedRoutedEventArgs e)
      {
         Map map = DataContext as Map;
         if (map == null)
         {
            throw new InvalidOperationException("Window has no model");
         }

         Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
         openDialog.Filter = FileFilter;
         openDialog.ShowReadOnly = false;

         bool? result = openDialog.ShowDialog();

         if (result == true)
         {
            CurrentMapFileName = openDialog.FileName;

            using (Stream stream = openDialog.OpenFile())
            {
               IFormatter formatter = new BinaryFormatter();
               Map newMap = (Map)formatter.Deserialize(stream);
               map.Assign(newMap);
            }
         }
      }

      private void OnSave(object sender, ExecutedRoutedEventArgs e)
      {
         Map map = DataContext as Map;
         if (map == null)
         {
            throw new InvalidOperationException("Window has no model to save");
         }

         if (CurrentMapFileName == null)
         {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = String.Format("Map{0}", DefaultFileCounter); // Default file name
            saveDialog.DefaultExt = "." + FileExtension;
            saveDialog.Filter = FileFilter;

            bool? result = saveDialog.ShowDialog();

            if (result == true)
            {
               DefaultFileCounter++;
               CurrentMapFileName = saveDialog.FileName;
            }
         }

         if (CurrentMapFileName != null)
         {
            using (Stream stream = new FileStream(CurrentMapFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
               IFormatter formatter = new BinaryFormatter();
               formatter.Serialize(stream, map);
            }
         }

      }

      private void OnNew(object sender, ExecutedRoutedEventArgs e)
      {
         Map map = DataContext as Map;
         if (map == null)
         {
            throw new InvalidOperationException("Window has no model");
         }

         map.Assign(new Map());
      }

      private void OnExit(object sender, ExecutedRoutedEventArgs e)
      {
         Application.Current.Shutdown();
      }
   }
}
