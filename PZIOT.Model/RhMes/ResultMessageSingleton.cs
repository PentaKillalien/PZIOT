using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Model.RhMes
{
    /// <summary>
    /// 返回值统一处理单例
    /// </summary>
    public class MessagePrefixSetter
    {
        /// <summary>
        /// 前缀
        /// </summary>
        public string MessagePrefix { get; set; }
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly MessagePrefixSetter Instance;
        static MessagePrefixSetter()
        {
            Instance = new MessagePrefixSetter();
        }
        /// <summary>
        /// 设置消息前缀
        /// </summary>
        /// <param name="messagePrefix">消息前缀</param>
        public void SetMessagePrefix(string messagePrefix)
        {
            MessagePrefix = messagePrefix;
        }
        private MessagePrefixSetter()
        {
        }
    }
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtendMethod
    {
        /// <summary>
        /// 转换为带有项目信息的返回值
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string AsProjectMessage(this string msg)
        {
            if (!string.IsNullOrEmpty(MessagePrefixSetter.Instance.MessagePrefix))
            {
                if (msg.Contains(MessagePrefixSetter.Instance.MessagePrefix))
                {
                    return msg;
                }
                return MessagePrefixSetter.Instance.MessagePrefix + msg;
            }
            return msg;
        }
    }
}
