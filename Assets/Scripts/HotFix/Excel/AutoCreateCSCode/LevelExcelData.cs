/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using HotFix.Common;
[Serializable]
public class LevelExcelItem : ExcelItemBase
{
	/// <summary>
	/// 数据id
	/// </summary>>
	public int id;
	/// <summary>
	/// 怪物数据Id
	/// </summary>>
	public int[] enemyCombineId;
}


public class LevelExcelData : ExcelDataBase
{
	public LevelExcelItem[] items;

	public Dictionary<int,LevelExcelItem> itemDic = new Dictionary<int,LevelExcelItem>();

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

	public int[] GetEnemyCombineId(int id)
	{
		var item = GetExcelItem(id) as LevelExcelItem;
		if(item == null)
			return default;
		return item.enemyCombineId;
	}
	public int GetEnemyCombineId(int id, int index)
	{
		var item0 = GetExcelItem(id) as LevelExcelItem;
		if(item0 == null)
			return default;
		var item1 = item0.enemyCombineId;
		if(item1 == null || index < 0 || index >= item1.Length)
			return default;
		return item1[index];
	}

	#endregion
}


#if UNITY_EDITOR
public class LevelAssetAssignment
{
	public static bool CreateAsset(ExcelMediumData excelMediumData, string excelAssetPath)
	{
		var allRowItemDicList = excelMediumData.GetAllRowItemDicList();
		if(allRowItemDicList == null || allRowItemDicList.Count == 0)
			return false;

		int rowCount = allRowItemDicList.Count;
		LevelExcelData excelDataAsset = ScriptableObject.CreateInstance<LevelExcelData>();
		excelDataAsset.items = new LevelExcelItem[rowCount];

		for(int i = 0; i < rowCount; i++)
		{
			var itemRowDic = allRowItemDicList[i];
			excelDataAsset.items[i] = new LevelExcelItem();
			excelDataAsset.items[i].id = StringUtility.StringToInt(itemRowDic["id"]);
			excelDataAsset.items[i].enemyCombineId = StringUtility.StringToIntArray(itemRowDic["enemyCombineId"]);
		}
		if(!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string fullPath = Path.Combine(excelAssetPath,typeof(LevelExcelData).Name) + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(fullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset,fullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



