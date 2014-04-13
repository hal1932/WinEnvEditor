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
using System.Collections;
using WinEnvEditor.Properties;

namespace WinEnvEditor.ViewModels
{
    public class EnvEditorControlViewModel : ViewModel
    {
        public class EnvCategory
        {
            public string Name { get; internal set; }
            public ObservableSynchronizedCollection<EnvItem>
                ItemList { get; set; }
        }


        #region EnvCategoryList変更通知プロパティ
        private EnvCategory[] _EnvCategoryList;

        public EnvCategory[] EnvCategoryList
        {
            get
            { return _EnvCategoryList; }
            set
            { 
                if (_EnvCategoryList == value)
                    return;
                _EnvCategoryList = value;
                RaisePropertyChanged("EnvCategoryList");
            }
        }
        #endregion

        #region CurrentCategoryIndex変更通知プロパティ
        private int _CurrentCategoryIndex;

        public int CurrentCategoryIndex
        {
            get
            { return _CurrentCategoryIndex; }
            set
            { 
                if (_CurrentCategoryIndex == value || value < 0)
                    return;
                _CurrentCategoryIndex = value;
                Settings.Default.LastSelectedCategoryIndex = value;
                RaisePropertyChanged("CurrentCategoryIndex");
            }
        }
        #endregion


        public EnvEditorControlViewModel()
        {
            var categoryList = new EnvCategory[]
            {
                new EnvCategory() { Name = "ユーザ環境変数" },
                new EnvCategory() { Name = "システム環境変数" },
            };

            var itemList = new List<EnvItem>();
            foreach(DictionaryEntry item in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User))
            {
                itemList.Add(new EnvItem()
                {
                    Key = item.Key as string,
                    Value = item.Value as string,
                });
            }
            categoryList[0].ItemList = new ObservableSynchronizedCollection<EnvItem>(itemList);

            itemList = new List<EnvItem>();
            foreach (DictionaryEntry item in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine))
            {
                itemList.Add(new EnvItem()
                {
                    Key = item.Key as string,
                    Value = item.Value as string,
                });
            }
            categoryList[1].ItemList = new ObservableSynchronizedCollection<EnvItem>(itemList);

            EnvCategoryList = categoryList;
            CurrentCategoryIndex = Settings.Default.LastSelectedCategoryIndex;
        }
    }
}
