using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using WinEnvEditor.Models;
using Microsoft.Win32;
using System.IO;

using WinEnvEditor.Properties;


namespace WinEnvEditor.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        #region EnvSetList変更通知プロパティ
        private EnvItemSetList _EnvSetList;

        public EnvItemSetList EnvSetList
        {
            get
            { return _EnvSetList; }
            set
            { 
                if (_EnvSetList == value)
                    return;
                _EnvSetList = value;
                RaisePropertyChanged("EnvSetList");
            }
        }
        #endregion

        #region CurrentSetIndex変更通知プロパティ
        private int _CurrentSetIndex;

        public int CurrentSetIndex
        {
            get
            { return _CurrentSetIndex; }
            set
            { 
                if (_CurrentSetIndex == value)
                    return;
                _CurrentSetIndex = value;
                RaisePropertyChanged("CurrentSetIndex");
            }
        }
        #endregion

        #region CurrentTabIndex変更通知プロパティ
        private int _CurrentTabIndex;

        public int CurrentTabIndex
        {
            get
            { return _CurrentTabIndex; }
            set
            { 
                if (_CurrentTabIndex == value)
                    return;
                _CurrentTabIndex = value;
                Settings.Default.LastSelectedTabIndex = value;
                RaisePropertyChanged("CurrentTabIndex");
            }
        }
        #endregion


        public void Initialize()
        {
            Console.WriteLine("MainWindowViewModel");

            var lastLoaded = Settings.Default.LastLoadedSetListFileName;
            if (File.Exists(lastLoaded))
            {
                ImportSetList(lastLoaded);
            }
            CurrentTabIndex = Settings.Default.LastSelectedTabIndex;
        }

        #region メニュー
        #region ファイル
        #region 環境セット
        public void ImportSet()
        {
            var form = new OpenFileDialog()
            {
                RestoreDirectory = true,
            };
            if (form.ShowDialog() == false) return;

            var found = EnvSetList.SetList.Where(set => set.FilePath == form.FileName);
            if (found.Count() > 0) return;

            var envSet = new EnvItemSet();
            if (!envSet.Load(form.FileName))
            {
                Console.WriteLine("cannot load envset: " + form.FileName);
                return;
            }

            EnvSetList.SetList.Add(envSet);
            CurrentSetIndex = EnvSetList.SetList.Count - 1;

            OneTimeMessenger.Send(typeof(EnvSetControlViewModel), "EnvSetList", EnvSetList);
        }

        public void ExportSetConfig()
        {
            var form = new OpenFileDialog()
            {
                RestoreDirectory = true,
            };
            if (form.ShowDialog() == false) return;

            var currentSet = EnvSetList.SetList[CurrentSetIndex];
            if (!currentSet.Save(form.FileName))
            {
                Console.WriteLine("cannot export envset: " + form.FileName);
                return;
            }
        }

        public void ExportSetBatch()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 環境セットリスト
        public void ImportSetList(string filename)
        {
            if (!File.Exists(filename))
            {
                var form = new OpenFileDialog()
                {
                    RestoreDirectory = true,
                };
                if (form.ShowDialog() == false) return;
                filename = form.FileName;
            }

            var envSetList = new EnvItemSetList();
            if (!envSetList.Load(filename))
            {
                Console.WriteLine("cannot import setlist: " + filename);
                return;
            }
            Settings.Default.LastLoadedSetListFileName = filename;
            envSetList.CurrentSetIndex = Settings.Default.LastSelectedSetIndex;

            EnvSetList = envSetList;

            OneTimeMessenger.Send(typeof(EnvSetControlViewModel), "EnvSetList", EnvSetList);
        }

        public void ExportSetList()
        {
            var form = new OpenFileDialog()
            {
                CheckFileExists = false,
                RestoreDirectory = true,
            };
            if (form.ShowDialog() == false) return;

            if (!EnvSetList.Save(form.FileName))
            {
                Console.WriteLine("cannot export setlist: " + form.FileName);
            }
        }
        #endregion
        #endregion

        #region 外部ツール
        public void OpenSystemPropertyWindow()
        {
            WindowsShell.StartProcess("control", new string[] { "sysdm.cpl" });
        }

        public void OpenRegistryEditor()
        {
            WindowsShell.StartProcess("regedit.exe");
        }
        #endregion
        #endregion
    }
}
