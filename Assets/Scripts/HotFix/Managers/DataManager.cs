using System.Collections.Generic;
using System.IO;
using HotFix.Data.Account;
using Main.Game.Base;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace HotFix.Managers
{
    /// <summary>
    /// 存档管理类
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        private const string PersonDataPath = "/Resources/ArchiveInfo.json";

        private PersonInfo _personInfo;

        /// <summary>
        /// 个人缓存本地数据
        /// </summary>
        public PersonInfo PersonInfo => _personInfo ??= LoadData();

        private PersonInfo LoadData()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("ArchiveInfo");
            if (textAsset != null)
            {
                return JsonConvert.DeserializeObject<PersonInfo>(textAsset.text);
            }

            return null;
        }

        // 我方卡池信息
        public List<CardInfo> OwnCardsList => PersonInfo.OwnCardsList;

        // 敌军当前卡池信息
        public readonly List<CardInfo> EnemyCardsList = new();

        // 设置当前关卡数据
        public void SetCurrentLevData(int levId)
        {
            EnemyCardsList.Clear();

            var levelExcelItem = ExcelManager.Instance.GetExcelItem<LevelExcelData, LevelExcelItem>(levId);

            foreach (var cardCombineId in levelExcelItem.enemyCardInfo)
            {
                EnemyCardsList.Add(new CardInfo
                {
                    ID = cardCombineId / 100,
                    StarLev = cardCombineId % 100
                });
            }
        }


        // 存档数据
        public void SaveData(PersonInfo personInfoList)
        {
            string filePath = Application.dataPath + PersonDataPath;
            // 找到当前路径
            FileInfo fileInfo = new FileInfo(filePath);
            // 判断有没有文件，有则打开，没有则创建后打开
            StreamWriter sw = fileInfo.CreateText();
            // 类 序列换化为json格式
            string dataJson = JsonConvert.SerializeObject(personInfoList);

            // 将转换好的字符串保存到文件
            sw.WriteLine(dataJson);
            // 释放资源
            sw.Close();
            sw.Dispose();

            AssetDatabase.Refresh();
        }
    }
}