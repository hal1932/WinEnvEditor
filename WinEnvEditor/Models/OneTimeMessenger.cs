using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinEnvEditor.Models
{
    /// <summary>
    /// 1度だけ指定の相手にメッセージを送る
    /// </summary>
    class OneTimeMessenger
    {
        private class MessageItem
        {
            public string Key;
            public object Value;
        }

        private static Dictionary<Type, Func<string, object, bool>> _callbackDic = new Dictionary<Type, Func<string, object, bool>>();
        private static Dictionary<Type, List<MessageItem>> _messagePool = new Dictionary<Type, List<MessageItem>>();


        /// <summary>
        /// メッセージ受け取り登録
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="callback"></param>
        public static void Register(Type receiver, Func<string, object, bool> callback)
        {
            // コールバック登録
            _callbackDic[receiver] = callback;

            // 未送信メッセージがあれば送信して削除
            List<MessageItem> pooledItemList;
            if (_messagePool.TryGetValue(receiver, out pooledItemList))
            {
                for (var i = 0; i < pooledItemList.Count; ++i)
                {
                    var item = pooledItemList[i];
                    if (callback(item.Key, item.Value))
                    {
                        pooledItemList.Remove(item);
                    }
                }
            }
        }

        /// <summary>
        /// メッセージ送信
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Send(Type receiver, string key, object value)
        {
            Func<string, object, bool> callback;
            if (_callbackDic.TryGetValue(receiver, out callback))
            {
                // 受信相手が登録済みならすぐに送信
                if (!callback(key, value))
                {
                    // 受信されなかったら手許に貯めとく
                    PoolMessage(receiver, key, value);
                }
            }
            else
            {
                // 受信相手がいなかったら手許に貯めとく
                // （登録されたときに送信し直す）
                PoolMessage(receiver, key, value);
            }
        }


        private static void PoolMessage(Type receiver, string key, object value)
        {
            List<MessageItem> itemList;
            if (_messagePool.TryGetValue(receiver, out itemList))
            {
                itemList.Add(new MessageItem() { Key = key, Value = value });
            }
            else
            {
                _messagePool[receiver] = new List<MessageItem>()
                    {
                        new MessageItem() { Key = key, Value = value }
                    };
            }
        }
    }
}
