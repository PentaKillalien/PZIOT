using System.Collections.Generic;
using System.Linq;

namespace PZIOT.Model.RhMes
{
    /// <summary>
    /// 字段信息模型
    /// </summary>
    public class DataFieldInfo
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 字段描述
        /// </summary>
        public string FieldDescription { get; set; }


        /// <summary>
        /// 创建类的实例
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldDescription"></param>
        /// <returns></returns>
        public static DataFieldInfo Create(string fieldName, string fieldDescription)
        {
            DataFieldInfo fieldInfo = new DataFieldInfo();
            fieldInfo.FieldName = fieldName;
            fieldInfo.FieldDescription = fieldDescription;
            return fieldInfo;
        }
    }

    /// <summary>
    /// 扩展类
    /// </summary>
    public static class DataFieldInfoExtension
    {
        /// <summary>
        /// 添加字段信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldDescription"></param>
        /// <returns></returns>
        public static List<DataFieldInfo> Append(this DataFieldInfo info, string fieldName, string fieldDescription)
        {
            List<DataFieldInfo> fieldInfos = new List<DataFieldInfo>();
            fieldInfos.Add(info);
            if (fieldInfos.FirstOrDefault(f => f.FieldName == fieldName) == null)
                fieldInfos.Add(DataFieldInfo.Create(fieldName, fieldDescription));
            return fieldInfos;
        }
        /// <summary>
        /// 添加字段信息
        /// </summary>
        /// <param name="fieldInfos"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldDescription"></param>
        /// <returns></returns>
        public static List<DataFieldInfo> Append(this List<DataFieldInfo> fieldInfos, string fieldName, string fieldDescription)
        {
            fieldInfos.Add(DataFieldInfo.Create(fieldName, fieldDescription));
            if (fieldInfos.FirstOrDefault(f => f.FieldName == fieldName) == null)
                fieldInfos.Add(DataFieldInfo.Create(fieldName, fieldDescription));
            return fieldInfos;
        }
    }
}