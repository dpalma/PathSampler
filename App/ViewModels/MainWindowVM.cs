﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Input;
using PathSampler.Commands;
using PathSampler.Models;
using PathSampler.PathFinders;

namespace PathSampler.ViewModels
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

            if (MapVM == null)
            {
               MapVM = new MapVM(Map);
            }
            else
            {
               MapVM.Map = Map;
            }
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
            if (MapVM != null)
            {
               MapVM.PathingFinished -= new EventHandler(MapVM_PathingFinished);
            }

            m_mapVM = value;
            FirePropertyChanged("MapVM");

            MapVM.PathingFinished += new EventHandler(MapVM_PathingFinished);
         }
      }

      private MapVM m_mapVM;

      private void MapVM_PathingFinished(object sender, EventArgs e)
      {
         UpdatePathingCommands();
      }

      private const string FileExtension = "mapx";

      private static string FileFilter
      {
         get { return String.Format("Map Files (.{0})|*.{0}", FileExtension); }
      }

      private int DefaultFileCounter = 1;

      public string CurrentMapFileName
      {
         get
         {
            return m_currentMapFileName;
         }
         private set
         {
            m_currentMapFileName = value;
            FirePropertyChanged("CurrentMapFileName");
         }
      }
      private string m_currentMapFileName = String.Empty;

      public void NewMap()
      {
         Map.Assign(new Map());
         CurrentMapFileName = String.Empty;
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
                           NewMap();
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
            if (!OpenMap(openDialog.FileName))
            {
               MessageBox.Show(String.Format("Unable to open {0}", openDialog.FileName),
                  null, MessageBoxButton.OK, MessageBoxImage.Hand);
            }
         }
      }

      public bool OpenMap(string mapFileName)
      {
         bool success = false;
         try
         {
            using (Stream stream = File.OpenRead(mapFileName))
            {
               IFormatter formatter = new BinaryFormatter();
               Map newMap = (Map)formatter.Deserialize(stream);
               Map.Assign(newMap);
               CurrentMapFileName = mapFileName;
               success = true;
            }
         }
         catch (SerializationException ex)
         {
            System.Diagnostics.Debug.WriteLine(ex.Message);
         }
         catch (FileNotFoundException ex)
         {
            System.Diagnostics.Debug.WriteLine(ex.Message);
         }
         return success;
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
         string mapFileName = CurrentMapFileName;

         if (String.IsNullOrEmpty(mapFileName))
         {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = String.Format("Map{0}", DefaultFileCounter); // Default file name
            saveDialog.DefaultExt = "." + FileExtension;
            saveDialog.Filter = FileFilter;

            bool? result = saveDialog.ShowDialog();

            if (result == true)
            {
               DefaultFileCounter++;
               mapFileName = saveDialog.FileName;
            }
         }

         if (mapFileName != null)
         {
            SaveMap(mapFileName);
         }
      }

      public void SaveMap(string mapFileName)
      {
         using (Stream stream = File.OpenWrite(mapFileName))
         {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, m_map);
            CurrentMapFileName = mapFileName;
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

      delegate void MethodInvoker();

      private void UpdatePathingCommands()
      {
         if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
         {
            if (m_stopPathingCommand != null)
            {
               m_stopPathingCommand.RaiseCanExecuteChanged();
            }
            if (m_startPathingCommand != null)
            {
               m_startPathingCommand.RaiseCanExecuteChanged();
            }
            if (m_pathSlowerCommand != null)
            {
               m_pathSlowerCommand.RaiseCanExecuteChanged();
            }
            if (m_pathFasterCommand != null)
            {
               m_pathFasterCommand.RaiseCanExecuteChanged();
            }
         }
         else
         {
            Application.Current.Dispatcher.BeginInvoke(new MethodInvoker(UpdatePathingCommands));
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
                        t => { return MapVM.CanStopPathing; });
            }
            return m_stopPathingCommand;
         }
      }

      private DelegateCommand m_pathSlowerCommand;
      public ICommand PathSlowerCommand
      {
         get
         {
            if (m_pathSlowerCommand == null)
            {
               m_pathSlowerCommand = new DelegateCommand(
                  t => { MapVM.PathSlower(); UpdatePathingCommands(); },
                  t => { return MapVM.CanPathSlower; });
            }
            return m_pathSlowerCommand;
         }
      }

      private DelegateCommand m_pathFasterCommand;
      public ICommand PathFasterCommand
      {
         get
         {
            if (m_pathFasterCommand == null)
            {
               m_pathFasterCommand = new DelegateCommand(
                  t => { MapVM.PathFaster(); UpdatePathingCommands(); },
                  t => { return MapVM.CanPathFaster; });
            }
            return m_pathFasterCommand;
         }
      }
   }
}
