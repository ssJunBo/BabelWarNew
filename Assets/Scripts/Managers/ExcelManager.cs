using System;
using System.Collections.Generic;
using System.IO;
using _GameBase;
using _GameBase.Excel2Class;
using ET;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace Managers
{
    public class ExcelManager : Singleton<ExcelManager>
    {
        private const string ExcelDataPath = "ExcelAssets/AutoCreateAsset/";

        private const string ExcelDataABPath = "AutoCreateAsset";
        
        public async ETTask InitData()
        {
            if (!Define.IsEditor)
                await ResourcesLoaderComponent.Instance.LoadAsync(ExcelDataABPath.StringToAB());
            
            string dataPath=Application.dataPath+"/ABRes/ExcelAssets/AutoCreateAsset";

            string assetPath = "Assets/ABRes/ExcelAssets/AutoCreateAsset/";
            
            if (Directory.Exists(dataPath))
            {
                DirectoryInfo direction = new DirectoryInfo(dataPath);
                FileInfo[] files = direction.GetFiles("*.asset");
                foreach (var file in files)
                {
                    var nameArr = file.Name.Split('.');
                    var type = Type.GetType(nameArr[0]);  
                    // 加载配置数据
                    // ExcelDataBase tmpInfo = UnityEngine.Resources.Load<ExcelDataBase>(ExcelDataPath + nameArr[0]);

                    ExcelDataBase tmpInfo;
                    if (Define.IsEditor)
                    {
                        tmpInfo = (ExcelDataBase)Define.LoadAssetAtPath(assetPath + file.Name);
                    }
                    else
                    {
                         tmpInfo =(ExcelDataBase)ResourcesComponent.Instance.GetAsset(ExcelDataABPath.StringToAB(), nameArr[0]);
                    }
                    
                    tmpInfo?.Init();
                    

                    if (type != null)
                    {
                        _excelDataDic[type] = tmpInfo;
                    }
                    else
                    {
                        Debug.Log("type 异常！");
                    }
                }
            }
        }

        private readonly Dictionary<Type, object> _excelDataDic = new();

        // 获取当前表
        public T GetExcelData<T>() where T : ExcelDataBase
        {
            Type type = typeof(T);
            if (_excelDataDic.ContainsKey(type) && _excelDataDic[type] is T)
                return _excelDataDic[type] as T;

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