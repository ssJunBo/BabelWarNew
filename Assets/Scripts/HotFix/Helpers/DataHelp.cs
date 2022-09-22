using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace HotFix.Helpers
{
    /// <summary>
    /// 功能存档帮助类
    /// </summary>
    public static class DataHelp
    {
        // 系统需要存的数据路径
        private static readonly string Model2JsonPath = Application.dataPath + "/Resources/Data/Model2Json";
        
        // 存成json数据 缓存本地
        public static void Model2Json<T>(T modelInfo)
        {
            string info = JsonConvert.SerializeObject(modelInfo);
            // 写入文件
            File.WriteAllText(Model2JsonPath + "/" + modelInfo.GetType().Name + ".json", info);
        }

        // 读取json 到对应类
        public static T Json2Model<T>(string json)
        {
            // 读文件
            T model = JsonConvert.DeserializeObject<T>(json);
            return model;
        }
    }
}