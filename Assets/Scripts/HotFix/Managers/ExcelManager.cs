using System;
using System.Collections.Generic;
using System.IO;
using Main.Game.Base;
using Main.Game.ResourceFrame;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace HotFix.Managers
{
    public class ExcelManager : Singleton<ExcelManager>
    {
        private const string ExcelDataPath = "ExcelAssets/AutoCreateAsset/";

        public void InitData()
        {
            string dataPath=Application.dataPath+"/Resources/ExcelAssets/AutoCreateAsset";
            if (Directory.Exists(dataPath))
            {
                DirectoryInfo direction = new DirectoryInfo(dataPath);
                FileInfo[] files = direction.GetFiles("*.asset");
                foreach (var file in files)
                {
                    var nameArr = file.Name.Split('.');
                    var type = Type.GetType(nameArr[0]);  
                    // 加载配置数据
                    ExcelDataBase tmpInfo = ResManager.Instance.LoadResource<ExcelDataBase>(ExcelDataPath + nameArr[0]);
                    tmpInfo.Init();

                    if (type != null)
                    {
                        excelDataDic[type] = tmpInfo;
                    }
                    else
                    {
                        Debug.Log("type 异常！");
                    }
                }
            }
        }

        readonly Dictionary<Type, object> excelDataDic = new Dictionary<Type, object>();

        // 获取当前表
        public T GetExcelData<T>() where T : ExcelDataBase
        {
            Type type = typeof(T);
            if (excelDataDic.ContainsKey(type) && excelDataDic[type] is T)
                return excelDataDic[type] as T;

            return null;
        }

        // 获取表内指定id的数据
        public V GetExcelItem<T, V>(int targetId) where T : ExcelDataBase where V : ExcelItemBase
        {
            var excelData = GetExcelData<T>();

            if (excelData != null)
                return excelData.GetExcelItem(targetId) as V;

            return null;
        }
    }
}