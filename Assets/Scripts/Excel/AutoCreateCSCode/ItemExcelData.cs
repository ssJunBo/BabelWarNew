/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using _GameBase.Excel2Class;
using Common;
[Serializable]
public class ItemExcelItem : ExcelItemBase
{
	/// <summary>
	/// 数据id
	/// </summary>>
	public int id;
	/// <summary>
	/// item图片
	/// </summary>>
	public string Icon;
	/// <summary>
	/// item名字
	/// </summary>>
	public string Name;
}


public class ItemExcelData : ExcelDataBase
{
	public ItemExcelItem[] items;

	public Dictionary<int,ItemExcelItem> itemDic = new Dictionary<int,ItemExcelItem>();

	public override void Init()
	{
		base.Init();
		itemDic.Clear();
		if(items != null && items.Length > 0)
		{
			for(int i = 0; i < items.Length; i++)
			{
				itemDic.Add(items[i].id, items[i]);
			}
		}
	}

	public override ExcelItemBase GetExcelItem(int id)
	{
		if(itemDic.ContainsKey(id))
			return itemDic[id];
		else
			return null;
	}
	#region --- Get Method ---

	public string GetIcon(int id)
	{
		var item = GetExcelItem(id) as ItemExcelItem;
		if(item == null)
			return default;
		return item.Icon;
	}

	public string GetName(int id)
	{
		var item = GetExcelItem(id) as ItemExcelItem;
		if(item == null)
			return default;
		return item.Name;
	}

	#endregion
}


#if UNITY_EDITOR
public class ItemAssetAssignment
{
	public static bool CreateAsset(ExcelMediumData excelMediumData, string excelAssetPath)
	{
		var allRowItemDicList = excelMediumData.GetAllRowItemDicList();
		if(allRowItemDicList == null || allRowItemDicList.Count == 0)
			return false;

		int rowCount = allRowItemDicList.Count;
		ItemExcelData excelDataAsset = ScriptableObject.CreateInstance<ItemExcelData>();
		excelDataAsset.items = new ItemExcelItem[rowCount];

		for(int i = 0; i < rowCount; i++)
		{
			var itemRowDic = allRowItemDicList[i];
			excelDataAsset.items[i] = new ItemExcelItem();
			excelDataAsset.items[i].id = StringUtility.StringToInt(itemRowDic["id"]);
			excelDataAsset.items[i].Icon = itemRowDic["Icon"];
			excelDataAsset.items[i].Name = itemRowDic["Name"];
		}
		if(!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string fullPath = Path.Combine(excelAssetPath,typeof(ItemExcelData).Name) + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(fullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset,fullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



