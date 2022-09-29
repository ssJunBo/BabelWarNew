using System.Collections.Generic;
using System.IO;
using _GameBase.Excel2Class;
using UnityEditor;
using UnityEngine;

namespace _GameBase.Editor.Excel
{
    public class BuildExcelWindow : EditorWindow
    {
        [MenuItem("Tools/Excel转换器",priority = 100)]
        public static void ShowReadExcelWindow()
        {
            BuildExcelWindow window = GetWindow<BuildExcelWindow>(true);
            window.Show();
            window.minSize = new Vector2(475,475);
        }
 
        //Excel读取路径，绝对路径，放在Assets同级路径
        private static string _excelReadAbsolutePath;
 
        //自动生成C#类文件路径，绝对路径
        private static string _scriptSaveAbsolutePath;
        private static string _scriptSaveRelativePath;
        //自动生成Asset文件路径，相对路径
        private static string _assetSaveRelativePath;
 
        private List<string> fileNameList = new List<string>();
        private List<string> filePathList = new List<string>();
 
        private void Awake()
        {
            titleContent.text = "Excel配置表读取";
 
            _excelReadAbsolutePath = Application.dataPath.Replace("Assets","Excel");
            _scriptSaveAbsolutePath = Application.dataPath + CheckEditorPath("/Scripts/Excel/AutoCreateCSCode");
            _scriptSaveRelativePath = CheckEditorPath("Assets/Scripts/Excel/AutoCreateCSCode");
            _assetSaveRelativePath = CheckEditorPath("Assets/Resources/ExcelAssets/AutoCreateAsset");
        }
 
        private void OnEnable()
        {
            RefreshExcelFile();
        }
 
        private void OnDisable()
        {
            fileNameList.Clear();
            filePathList.Clear();
        }
 
        private Vector2 scrollPosition = Vector2.zero;
        private void OnGUI()
        {
            GUILayout.Space(10);
 
            scrollPosition = GUILayout.BeginScrollView(scrollPosition,GUILayout.Width(position.width),GUILayout.Height(position.height));
 
            //展示路径
            GUILayout.BeginHorizontal(GUILayout.Height(20));
            if(GUILayout.Button("Excel读取路径",GUILayout.Width(100)))
            {
                EditorUtility.OpenWithDefaultApp(_excelReadAbsolutePath);
                Debug.Log(_excelReadAbsolutePath);
            }
            if(GUILayout.Button("Script保存路径",GUILayout.Width(100)))
            {
                SelectObject(_scriptSaveRelativePath);
            }
            if(GUILayout.Button("Asset保存路径",GUILayout.Width(100)))
            {
                SelectObject(_assetSaveRelativePath);
            }
        
            if(GUILayout.Button("删除之前生成类和Asset文件",GUILayout.Width(300)))
            {
                DeleteBeforeFile();
            }
        
            GUILayout.EndHorizontal();
 
            GUILayout.Space(5);
 
            //Excel列表
 
            GUILayout.Label("Excel列表：");
            for(int i = 0; i < fileNameList.Count; i++)
            {
                GUILayout.BeginHorizontal("Box",GUILayout.Height(40));
 
                GUILayout.Label($"{i}:","Titlebar Foldout",GUILayout.Width(30),GUILayout.Height(35));
                GUILayout.Box(fileNameList[i],"MeTransitionBlock",GUILayout.MinWidth(200),GUILayout.Height(35));
                GUILayout.Space(10);
 
                //生成CS代码
                if(GUILayout.Button("Create Script",GUILayout.Width(100),GUILayout.Height(30)))
                {
                    _scriptSaveAbsolutePath = Application.dataPath + CheckEditorPath("/Scripts/Excel/AutoCreateCSCode");
                    ExcelDataReader.ReadOneExcelToCode(filePathList[i],_scriptSaveAbsolutePath);
                }
                //生成Asset文件
                if(GUILayout.Button("Create Asset",GUILayout.Width(100),GUILayout.Height(30)))
                {
                    _assetSaveRelativePath = CheckEditorPath("Assets/Resources/ExcelAssets/AutoCreateAsset");
                    ExcelDataReader.CreateOneExcelAsset(filePathList[i],_assetSaveRelativePath);
                }
 
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.Space(10);
 
            //一键处理所有Excel
 
            GUILayout.Label("一键操作：");
            GUILayout.BeginHorizontal("Box",GUILayout.Height(40));
 
            GUILayout.Label("all","Titlebar Foldout",GUILayout.Width(30),GUILayout.Height(35));
            GUILayout.Box("All Excel","MeTransitionBlock",GUILayout.MinWidth(200),GUILayout.Height(35));
            GUILayout.Space(10);
 
            if(GUILayout.Button("Create Script",GUILayout.Width(100),GUILayout.Height(30)))
            {
                _excelReadAbsolutePath = Application.dataPath.Replace("Assets","Excel");
                _scriptSaveAbsolutePath = Application.dataPath + CheckEditorPath("/Scripts/Excel/AutoCreateCSCode");

                ExcelDataReader.ReadAllExcelToCode(_excelReadAbsolutePath,_scriptSaveAbsolutePath);
            }
            if(GUILayout.Button("Create Asset",GUILayout.Width(100),GUILayout.Height(30)))
            {
                _excelReadAbsolutePath = Application.dataPath.Replace("Assets","Excel");
                _assetSaveRelativePath = CheckEditorPath("Assets/Resources/ExcelAssets/AutoCreateAsset");

                ExcelDataReader.CreateAllExcelAsset(_excelReadAbsolutePath,_assetSaveRelativePath);
            }
            GUILayout.EndHorizontal();
 
            //
            GUILayout.Space(20);
            //
            GUILayout.EndScrollView();
        }
 
        //读取指定路径下的Excel文件名
        private void RefreshExcelFile()
        {
            fileNameList.Clear();
            filePathList.Clear();
 
            _excelReadAbsolutePath = Application.dataPath.Replace("Assets","Excel");
        
            if(!Directory.Exists(_excelReadAbsolutePath))
            {
                Debug.LogError("无效路径：" + _excelReadAbsolutePath);
                return;
            }
            string[] excelFileFullPaths = Directory.GetFiles(_excelReadAbsolutePath,"*.xlsx");
 
            if(excelFileFullPaths.Length == 0)
            {
                Debug.LogError(_excelReadAbsolutePath + "路径下没有找到Excel文件");
                return;
            }
 
            filePathList.AddRange(excelFileFullPaths);
            for(int i = 0; i < filePathList.Count; i++)
            {
                fileNameList.Add(Path.GetFileName(filePathList[i]));
            }
            Debug.Log("找到Excel文件：" + fileNameList.Count + "个");
        }
 
        private void SelectObject(string targetPath)
        {
            Object targetObj = AssetDatabase.LoadAssetAtPath<Object>(targetPath);
            EditorGUIUtility.PingObject(targetObj);
            Selection.activeObject = targetObj;
            Debug.Log(targetPath);
        }

        private void DeleteBeforeFile()
        {
            if (Directory.Exists(Application.dataPath+"/Scripts/Excel"))
            {
                Directory.Delete(Application.dataPath + "/Scripts/Excel", true);
            }

            if (Directory.Exists(Application.dataPath+"/Resources/ExcelAssets"))
            {
                Directory.Delete(Application.dataPath+"/Resources/ExcelAssets",true);
            }
        
            AssetDatabase.Refresh();
        }

        private static string CheckEditorPath(string path)
        {
#if UNITY_EDITOR_WIN
            return path.Replace("/","\\");
#elif UNITY_EDITOR_OSX
        return path.Replace("\\","/");
#else
        return path;
#endif
        }
    }
}