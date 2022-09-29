using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace _GameBase.Editor.Excel
{
	public class ExcelTools : EditorWindow
	{
		/// <summary>
		/// 当前编辑器窗口实例
		/// </summary>
		private static ExcelTools _instance;

		/// <summary>
		/// Excel文件列表
		/// </summary>
		private static List<string> _excelList;

		/// <summary>
		/// 项目根路径	
		/// </summary>
		private static string _pathRoot;

		/// <summary>
		/// 滚动窗口初始位置
		/// </summary>
		private static Vector2 _scrollPos;

		/// <summary>
		/// 输出格式索引
		/// </summary>
		private static int _indexOfFormat;

		/// <summary>
		/// 输出格式
		/// </summary>
		private static readonly string[] FormatOption = { "JSON", "CSV", "XML" };

		/// <summary>
		/// 编码索引
		/// </summary>
		private static int _indexOfEncoding;

		/// <summary>
		/// 编码选项
		/// </summary>
		private static readonly string[] EncodingOption = { "UTF-8", "GB2312" };

		/// <summary>
		/// 是否保留原始文件
		/// </summary>
		private static bool _keepSource = true;

		/// <summary>
		/// 显示当前窗口	
		/// </summary>
		[MenuItem("Tools/ExcelTools (Obs)")]
		static void ShowExcelTools()
		{
			Init();
			//加载Excel文件
			LoadExcel();
			
			_instance.Show();
		}

		void OnGUI()
		{
			DrawOptions();
			DrawExport();
		}

		/// <summary>
		/// 绘制插件界面配置项
		/// </summary>
		private void DrawOptions()
		{
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("请选择格式类型:", GUILayout.Width(85));
			_indexOfFormat = EditorGUILayout.Popup(_indexOfFormat, FormatOption, GUILayout.Width(125));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("请选择编码类型:", GUILayout.Width(85));
			_indexOfEncoding = EditorGUILayout.Popup(_indexOfEncoding, EncodingOption, GUILayout.Width(125));
			GUILayout.EndHorizontal();

			_keepSource = GUILayout.Toggle(_keepSource, "保留Excel源文件");
		}

		/// <summary>
		/// 绘制插件界面输出项
		/// </summary>
		private void DrawExport()
		{
			if (_excelList == null) return;
			if (_excelList.Count < 1)
			{
				EditorGUILayout.LabelField("目前没有Excel文件被选中哦!");
			}
			else
			{
				EditorGUILayout.LabelField("下列项目将被转换为" + FormatOption[_indexOfFormat] + ":");
				GUILayout.BeginVertical();
				_scrollPos = GUILayout.BeginScrollView(_scrollPos, false, true, GUILayout.Height(150));
				foreach (string s in _excelList)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Toggle(true, s);
					GUILayout.EndHorizontal();
				}

				GUILayout.EndScrollView();
				GUILayout.EndVertical();

				//输出
				if (GUILayout.Button("转换"))
				{
					Convert();
				}
			}
		}

		/// <summary>
		/// 转换Excel文件
		/// </summary>
		private static void Convert()
		{
			if (_excelList.Count > 0)
			{
				string dataPath = Application.dataPath + "/Resources/Data";
				if (!Directory.Exists(dataPath))
				{
					Directory.CreateDirectory(dataPath);
				}
				else
				{
					DirectoryInfo direction = new DirectoryInfo(dataPath);
					FileInfo[] files = direction.GetFiles("*");
					foreach (var file in files)
					{
						file.Delete();
					}
				}
			}

			string excel2JsonOutputPath = Application.dataPath + "/Resources/Data/Excel";

			foreach (string excelName in _excelList)
			{
				//获取Excel文件的绝对路径
				string excelPath = _pathRoot + "/DataExcels/" + excelName;
				//构造Excel工具类
				ExcelUtility excel = new ExcelUtility(excelPath);

				//判断编码类型
				Encoding encoding = null;
				if (_indexOfEncoding == 0)
				{
					encoding = Encoding.GetEncoding("utf-8");
				}
				else if (_indexOfEncoding == 1)
				{
					encoding = Encoding.GetEncoding("gb2312");
				}

				//判断输出类型
				string output;
				if (_indexOfFormat == 0)
				{
					output = (excel2JsonOutputPath + "/" + excelName).Replace(".xlsx", ".json");
					excel.ConvertToJson(output, encoding);
				}

				// else if (_indexOfFormat == 1)
				// {
				// 	output = excelPath.Replace(".xlsx", ".csv");
				// 	excel.ConvertToCsv(output, encoding);
				// }
				// else if (_indexOfFormat == 2)
				// {
				// 	output = excelPath.Replace(".xlsx", ".xml");
				// 	excel.ConvertToXml(output);
				// }

				//判断是否保留源文件
				if (!_keepSource)
				{
					FileUtil.DeleteFileOrDirectory(excelPath);
				}

				//刷新本地资源
				AssetDatabase.Refresh();
			}

			//转换完后关闭插件
			//这样做是为了解决窗口
			//再次点击时路径错误的Bug
			_instance.Close();

		}

		/// <summary>
		/// 加载Excel
		/// </summary>
		private static void LoadExcel()
		{
			if (_excelList == null) _excelList = new List<string>();
			_excelList.Clear();
		
			// 获取根目录下 data excels 文件
			string excelsPath = _pathRoot + "/" + "DataExcels";
			if (Directory.Exists(excelsPath))
			{
				DirectoryInfo direction = new DirectoryInfo(excelsPath);
				FileInfo[] files = direction.GetFiles("*");
				foreach (var file in files)
				{
					if (file.Name.EndsWith(".xlsx"))
					{
						_excelList.Add(file.Name);
					}
				}
			}
			
			// //获取选中的对象
			// Object[] selection = Selection.objects;
			// //判断是否有对象被选中
			// if (selection.Length == 0)
			// 	return;
			// //遍历每一个对象判断不是Excel文件
			// foreach (Object obj in selection)
			// {
			// 	string objPath = AssetDatabase.GetAssetPath(obj);
			// 	if (objPath.EndsWith(".xlsx"))
			// 	{
			// 		_excelList.Add(objPath);
			// 	}
			// }
		}

		private static void Init()
		{
			//获取当前实例
			_instance = GetWindow<ExcelTools>();
			//初始化
			_pathRoot = Application.dataPath;
			//注意这里需要对路径进行处理
			//目的是去除Assets这部分字符以获取项目目录
			//我表示Windows的/符号一直没有搞懂
			_pathRoot = _pathRoot.Substring(0, _pathRoot.LastIndexOf("/", StringComparison.Ordinal));
			_excelList = new List<string>();
			_scrollPos = new Vector2(_instance.position.x, _instance.position.y + 75);
		}

		// void OnSelectionChange()
		// {
		// 	//当选择发生变化时重绘窗体
		// 	Show();
		// 	LoadExcel();
		// 	Repaint();
		// }
	}
}
