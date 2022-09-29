using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using Excel;
using Newtonsoft.Json;

namespace _GameBase.Editor.Excel
{
	public class ExcelUtility
	{

		/// <summary>
		/// 表格数据集合
		/// </summary>
		private DataSet mResultSet;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="excelFile">Excel file.</param>
		public ExcelUtility(string excelFile)
		{
			FileStream fileStream = File.Open(excelFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
			mResultSet = mExcelReader.AsDataSet();
			
			fileStream.Close();
		}

		/// <summary>
		/// 转换为实体类列表
		/// </summary>
		public List<T> ConvertToList<T>()
		{
			//判断Excel文件中是否存在数据表
			if (mResultSet.Tables.Count < 1)
				return null;
			//默认读取第一个数据表
			DataTable mSheet = mResultSet.Tables[0];

			//判断数据表内是否存在数据
			if (mSheet.Rows.Count < 1)
				return null;

			//读取数据表行数和列数
			int rowCount = mSheet.Rows.Count;
			int colCount = mSheet.Columns.Count;

			//准备一个列表以保存全部数据
			List<T> list = new List<T>();

			//读取数据
			for (int i = 1; i < rowCount; i++)
			{
				//创建实例
				Type t = typeof(T);
				ConstructorInfo ct = t.GetConstructor(Type.EmptyTypes);
				if (ct != null)
				{
					T target = (T)ct.Invoke(null);
					for (int j = 0; j < colCount; j++)
					{
						//读取第1行数据作为表头字段
						string field = mSheet.Rows[0][j].ToString();
						object value = mSheet.Rows[i][j];
						//设置属性值
						SetTargetProperty(target, field, value);
					}

					//添加至列表
					list.Add(target);
				}
			}

			return list;
		}

		/// <summary>
		/// 转换为Json
		/// </summary>
		/// <param name="jsonPath">Json文件路径</param>
		/// <param name="encoding"></param>
		public void ConvertToJson(string jsonPath, Encoding encoding)
		{
			//判断Excel文件中是否存在数据表
			if (mResultSet.Tables.Count < 1)
				return;

			//默认读取第一个数据表
			DataTable mSheet = mResultSet.Tables[0];

			//判断数据表内是否存在数据
			if (mSheet.Rows.Count < 1)
				return;

			//读取数据表行数和列数
			int rowCount = mSheet.Rows.Count;
			int colCount = mSheet.Columns.Count;

			//准备一个列表存储整个表的数据
			List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

			//读取数据
			for (int i = 1; i < rowCount; i++)
			{
				//准备一个字典存储每一行的数据
				Dictionary<string, object> row = new Dictionary<string, object>();
				for (int j = 0; j < colCount; j++)
				{
					//读取第1行数据作为表头字段
					string field = mSheet.Rows[0][j].ToString();
					//Key-Value对应
					row[field] = mSheet.Rows[i][j];
				}

				//添加到表数据中
				table.Add(row);
			}

			//生成Json字符串
			string json = JsonConvert.SerializeObject(table, Formatting.Indented);
			//写入文件
			using (FileStream fileStream = new FileStream(jsonPath, FileMode.Create, FileAccess.Write))
			{
				using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
				{
					textWriter.Write(json);
				}
			}
		}

		/// <summary>
		/// 转换为CSV
		/// </summary>
		public void ConvertToCsv(string csvPath, Encoding encoding)
		{
			//判断Excel文件中是否存在数据表
			if (mResultSet.Tables.Count < 1)
				return;

			//默认读取第一个数据表
			DataTable mSheet = mResultSet.Tables[0];

			//判断数据表内是否存在数据
			if (mSheet.Rows.Count < 1)
				return;

			//读取数据表行数和列数
			int rowCount = mSheet.Rows.Count;
			int colCount = mSheet.Columns.Count;

			//创建一个StringBuilder存储数据
			StringBuilder stringBuilder = new StringBuilder();

			//读取数据
			for (int i = 0; i < rowCount; i++)
			{
				for (int j = 0; j < colCount; j++)
				{
					//使用","分割每一个数值
					stringBuilder.Append(mSheet.Rows[i][j] + ",");
				}

				//使用换行符分割每一行
				stringBuilder.Append("\r\n");
			}

			//写入文件
			using (FileStream fileStream = new FileStream(csvPath, FileMode.Create, FileAccess.Write))
			{
				using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
				{
					textWriter.Write(stringBuilder.ToString());
				}
			}

		}

		/// <summary>
		/// 导出为Xml
		/// </summary>
		public void ConvertToXml(string xmlFile)
		{
			//判断Excel文件中是否存在数据表
			if (mResultSet.Tables.Count < 1)
				return;

			//默认读取第一个数据表
			DataTable mSheet = mResultSet.Tables[0];

			//判断数据表内是否存在数据
			if (mSheet.Rows.Count < 1)
				return;

			//读取数据表行数和列数
			int rowCount = mSheet.Rows.Count;
			int colCount = mSheet.Columns.Count;

			//创建一个StringBuilder存储数据
			StringBuilder stringBuilder = new StringBuilder();
			//创建Xml文件头
			stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			stringBuilder.Append("\r\n");
			//创建根节点
			stringBuilder.Append("<Table>");
			stringBuilder.Append("\r\n");
			//读取数据
			for (int i = 1; i < rowCount; i++)
			{
				//创建子节点
				stringBuilder.Append("  <Row>");
				stringBuilder.Append("\r\n");
				for (int j = 0; j < colCount; j++)
				{
					stringBuilder.Append("   <" + mSheet.Rows[0][j] + ">");
					stringBuilder.Append(mSheet.Rows[i][j]);
					stringBuilder.Append("</" + mSheet.Rows[0][j] + ">");
					stringBuilder.Append("\r\n");
				}

				//使用换行符分割每一行
				stringBuilder.Append("  </Row>");
				stringBuilder.Append("\r\n");
			}

			//闭合标签
			stringBuilder.Append("</Table>");
			//写入文件
			using (FileStream fileStream = new FileStream(xmlFile, FileMode.Create, FileAccess.Write))
			{
				using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.GetEncoding("utf-8")))
				{
					textWriter.Write(stringBuilder.ToString());
				}
			}
		}

		/// <summary>
		/// 设置目标实例的属性
		/// </summary>
		private void SetTargetProperty(object target, string propertyName, object propertyValue)
		{
			//获取类型
			Type mType = target.GetType();
			//获取属性集合
			PropertyInfo[] mProperty = mType.GetProperties();
			foreach (PropertyInfo property in mProperty)
			{
				if (property.Name == propertyName)
				{
					property.SetValue(target, Convert.ChangeType(propertyValue, property.PropertyType), null);
				}
			}
		}
	}
}

