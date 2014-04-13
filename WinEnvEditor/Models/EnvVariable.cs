using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.ComponentModel;
using System.Collections;
using System.IO;

namespace WinEnvEditor.Models
{
    // KeyValuePair<> は public setter ついてない
    public class EnvItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class EnvItemSet
    {
        #region 取得+設定可能
        public ObservableSynchronizedCollection<EnvItem>
            ItemList { get; set; }
        #endregion

        #region 取得のみ
        public string FilePath { get; private set; }
        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(FilePath); }
        }
        #endregion

        public bool Load(string filename)
        {
            if (!File.Exists(filename)) return false;

            var itemList = new List<EnvItem>();
            using (var reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    var items = reader.ReadLine().Split(',');
                    if (items.Length != 2) continue;

                    var key = items[0].Trim();
                    var value = items[1].Trim();
                    itemList.Add(new EnvItem() { Key = key, Value = value });
                }
            }

            FilePath = filename;
            ItemList = new ObservableSynchronizedCollection<EnvItem>(itemList);

            return true;
        }

        public bool Save(string filename = null)
        {
            if (filename == null) filename = FilePath;
            if (!File.Exists(filename)) return false;

            using (var writer = new StreamWriter(filename))
            {
                foreach (var item in ItemList)
                {
                    writer.WriteLine("{0},{1}", item.Key, item.Value);
                }
            }

            return true;
        }
    }

    public class EnvItemSetList :NotificationObject
    {
        #region 取得+設定可能
        public ObservableSynchronizedCollection<EnvItemSet>
            SetList { get; set; }

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
        #endregion

        #region 取得のみ
        public string FilePath { get; private set; }
        public string[] NameList
        {
            get { return SetList.Select(item => item.Name).ToArray(); }
        }
        #endregion


        public bool Load(string filename)
        {
            if (!File.Exists(filename)) return false;

            var setList = new List<EnvItemSet>();
            using (var reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    var listfile = reader.ReadLine().Trim();
                    if (!File.Exists(listfile)) continue;

                    var set = new EnvItemSet();
                    if (!set.Load(listfile)) continue;

                    setList.Add(set);
                }
            }

            FilePath = filename;
            SetList = new ObservableSynchronizedCollection<EnvItemSet>(setList);
            CurrentSetIndex = 0;

            return true;
        }

        public bool Save(string filename = null)
        {
            if (filename == null) filename = FilePath;

            using (var writer = new StreamWriter(filename))
            {
                foreach (var set in SetList)
                {
                    if (!set.Save()) continue;
                    writer.WriteLine(set.FilePath);
                }
            }

            return true;
        }
    }
}
