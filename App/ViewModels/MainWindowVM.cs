using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Input;
using PathFind.Commands;
using PathFind.Models;
using PathFind.PathFinders;

namespace PathFind.ViewModels
{
   public class MainWindowVM : ViewModel
   {
      public MainWindowVM()
      {
      }

      public Map Map
      {
         get
         {
            return m_map;
         }
         set
         {
            if (value == null)
            {
               throw new ArgumentNullException("Map");
            }

            if (Map != null)
            {
               Map.PropertyChanged -= new PropertyChangedEventHandler(Map_PropertyChanged);
            }

            m_map = value;
            FirePropertyChanged("Map");

            Map.PropertyChanged += new PropertyChangedEventHandler(Map_PropertyChanged);

            MapVM = new MapVM(Map);
         }
      }

      void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         UpdatePathingCommands();
      }

      private Map m_map;

      public MapVM MapVM
      {
         get
         {
            return m_mapVM;
         }
         private set
         {
            m_mapVM = value;
            FirePropertyChanged("MapVM");
         }
      }
      private MapVM m_mapVM;

      private const string FileExtension = "mapx";

      private static string FileFilter
      {
         get { return String.Format("Map Files (.{0})|*.{0}", FileExtension); }
      }

      private int DefaultFileCounter = 1;

      private string CurrentMapFileName;

      private void New()
      {
         Map.Assign(new Map());
      }

      public ICommand NewCommand
      {
         get
         {
            if (m_newCommand == null)
            {
               m_newCommand = new DelegateCommand(
                        t =>
                        {
                           New();
                        });
            }
            return m_newCommand;
         }
      }
      private DelegateCommand m_newCommand;

      private void Open()
      {
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
               Map.Assign(newMap);
            }
         }
      }

      public ICommand OpenCommand
      {
         get
         {
            if (m_openCommand == null)
            {
               m_openCommand = new DelegateCommand(
                        t =>
                        {
                           Open();
                        });
            }
            return m_openCommand;
         }
      }
      private DelegateCommand m_openCommand;

      private void Save()
      {
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
               formatter.Serialize(stream, Map);
            }
         }
      }

      public ICommand SaveCommand
      {
         get
         {
            if (m_saveCommand == null)
            {
               m_saveCommand = new DelegateCommand(
                        t =>
                        {
                           Save();
                        });
            }
            return m_saveCommand;
         }
      }
      private DelegateCommand m_saveCommand;

      private void UpdatePathingCommands()
      {
         if (m_stopPathingCommand != null)
         {
            m_stopPathingCommand.RaiseCanExecuteChanged();
         }
         if (m_startPathingCommand != null)
         {
            m_startPathingCommand.RaiseCanExecuteChanged();
         }
      }

      private DelegateCommand m_startPathingCommand;
      public ICommand StartPathingCommand
      {
         get
         {
            if (m_startPathingCommand == null)
            {
               m_startPathingCommand = new DelegateCommand(
                        t =>
                        {
                           MapVM.StartPathing();
                           UpdatePathingCommands();
                        },
                        t => { return MapVM.CanStartPathing; });
            }
            return m_startPathingCommand;
         }
      }

      private DelegateCommand m_stopPathingCommand;
      public ICommand StopPathingCommand
      {
         get
         {
            if (m_stopPathingCommand == null)
            {
               m_stopPathingCommand = new DelegateCommand(
                        t =>
                        {
                           MapVM.StopPathing();
                           UpdatePathingCommands();
                        },
                        t => { return MapVM.IsPathing; });
            }
            return m_stopPathingCommand;
         }
      }
   }
}
