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
using WinEnvEditor.Properties;

namespace WinEnvEditor.ViewModels
{
    public class EnvSetControlViewModel : ViewModel
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

        #region CurrentSet変更通知プロパティ
        private EnvItem[] _CurrentSet;

        public EnvItem[] CurrentSet
        {
            get
            { return _CurrentSet; }
            set
            { 
                if (_CurrentSet == value)
                    return;
                _CurrentSet = value;
                RaisePropertyChanged("CurrentSet");
            }
        }
        #endregion

        #region TargetList変更通知プロパティ
        private KeyValuePair<EditTarget, string>[] _TargetList;

        public KeyValuePair<EditTarget, string>[] TargetList
        {
            get
            { return _TargetList; }
            set
            { 
                if (_TargetList == value)
                    return;
                _TargetList = value;
                RaisePropertyChanged("TargetList");
            }
        }
        #endregion

        #region CurrentTargetIndex変更通知プロパティ
        private int _CurrentTargetIndex;

        public int CurrentTargetIndex
        {
            get
            { return _CurrentTargetIndex; }
            set
            {
                if (_CurrentTargetIndex == value || value < 0)
                    return;
                _CurrentTargetIndex = value;
                Settings.Default.LastSelectedEditTargetIndex = value;
                RaisePropertyChanged("CurrentTargetIndex");
            }
        }
        #endregion

        #region OptionList変更通知プロパティ
        private KeyValuePair<EditOption, string>[] _OptionList;

        public KeyValuePair<EditOption, string>[] OptionList
        {
            get
            { return _OptionList; }
            set
            {
                if (_OptionList == value)
                    return;
                _OptionList = value;
                RaisePropertyChanged("OptionList");
            }
        }
        #endregion

        #region CurrentOptionIndex変更通知プロパティ
        private int _CurrentOptionIndex;

        public int CurrentOptionIndex
        {
            get
            { return _CurrentOptionIndex; }
            set
            {
                if (_CurrentOptionIndex == value || value < 0)
                    return;
                _CurrentOptionIndex = value;
                Settings.Default.LastSelectedEditOptionIndex = value;
                RaisePropertyChanged("CurrentOptionIndex");
            }
        }
        #endregion

        #region ModeList変更通知プロパティ
        private KeyValuePair<EditMode, string>[] _ModeList;

        public KeyValuePair<EditMode, string>[] ModeList
        {
            get
            { return _ModeList; }
            set
            { 
                if (_ModeList == value)
                    return;
                _ModeList = value;
                RaisePropertyChanged("ModeList");
            }
        }
        #endregion

        #region CurrentModeIndex変更通知プロパティ
        private int _CurrentModeIndex;

        public int CurrentModeIndex
        {
            get
            { return _CurrentModeIndex; }
            set
            {
                if (_CurrentModeIndex == value || value < 0)
                    return;
                _CurrentModeIndex = value;
                Settings.Default.LastSelectedEditModeIndex = value;
                RaisePropertyChanged("CurrentModeIndex");
            }
        }
        #endregion


        public EnvSetControlViewModel()
        {
            // 環境セットリスト
            OneTimeMessenger.Register(typeof(EnvSetControlViewModel), (key, value) =>
            {
                if (key != "EnvSetList") return false;

                // 登録
                EnvSetList = value as EnvItemSetList;
                CurrentSet = EnvSetList
                    .SetList[Settings.Default.LastSelectedSetIndex]
                    .ItemList
                    .ToArray();

                // 以後の変更はイベントで
                EnvSetList.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "CurrentSetIndex")
                    {
                        var selected = EnvSetList.CurrentSetIndex;
                        CurrentSet = EnvSetList.SetList[selected].ItemList.ToArray();
                        Settings.Default.LastSelectedSetIndex = selected;
                    }
                };
                return true;
            });

            // 反映先
            TargetList = new KeyValuePair<EditTarget, string>[]
            {
                new KeyValuePair<EditTarget, string>(EditTarget.UserEnv, "ユーザ環境変数"),
                new KeyValuePair<EditTarget, string>(EditTarget.SystemEnv, "システム環境変数"),
            };
            CurrentTargetIndex = Settings.Default.LastSelectedEditTargetIndex;

            // 反映オプション
            OptionList = new KeyValuePair<EditOption, string>[]
            {
                new KeyValuePair<EditOption, string>(EditOption.Overwrite, "上書き"),
                new KeyValuePair<EditOption, string>(EditOption.Diff, "差分"),
            };
            CurrentOptionIndex = Settings.Default.LastSelectedEditOptionIndex;

            // 反映方法
            ModeList = new KeyValuePair<EditMode, string>[]
            {
                new KeyValuePair<EditMode, string>(EditMode.Add, "追加"),
                new KeyValuePair<EditMode, string>(EditMode.Remove, "削除"),
            };
            CurrentModeIndex = Settings.Default.LastSelectedEditModeIndex;
        }

        public void ApplyEnvSet()
        {
            var target = TargetList[CurrentTargetIndex];
            var option = OptionList[CurrentOptionIndex];
            var mode = ModeList[CurrentModeIndex];
            Console.WriteLine("{0}, {1}, {2}", target, option, mode);
        }
    }
}
