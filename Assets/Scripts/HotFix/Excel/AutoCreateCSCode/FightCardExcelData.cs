/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using HotFix.Common;
[Serializable]
public class FightCardExcelItem : ExcelItemBase
{
	/// <summary>
	/// 数据id
	/// </summary>>
	public int id;
	/// <summary>
	/// 卡片名
	/// </summary>>
	public string Name;
	/// <summary>
	/// 描述
	/// </summary>>
	public string Desc;
	/// <summary>
	/// 图片
	/// </summary>>
	public int Icon;
	/// <summary>
	/// 伤害
	/// </summary>>
	public int damage;
}


public class FightCardExcelData : ExcelDataBase
{
	public FightCardExcelItem[] items;

	public Dictionary<int,FightCardExcelItem> itemDic = new Dictionary<int,FightCardExcelItem>();

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

	public string GetName(int id)
	{
		var item = GetExcelItem(id) as FightCardExcelItem;
		if(item == null)
			return default;
		return item.Name;
	}

	public string GetDesc(int id)
	{
		var item = GetExcelItem(id) as FightCardExcelItem;
		if(item == null)
			return default;
		return item.Desc;
	}

	public int GetIcon(int id)
	{
		var item = GetExcelItem(id) as FightCardExcelItem;
		if(item == null)
			return default;
		return item.Icon;
	}

	public int GetDamage(int id)
	{
		var item = GetExcelItem(id) as FightCardExcelItem;
		if(item == null)
			return default;
		return item.damage;
	}

	#endregion
}


#if UNITY_EDITOR
public class FightCardAssetAssignment
{
	public static bool CreateAsset(ExcelMediumData excelMediumData, string excelAssetPath)
	{
		var allRowItemDicList = excelMediumData.GetAllRowItemDicList();
		if(allRowItemDicList == null || allRowItemDicList.Count == 0)
			return false;

		int rowCount = allRowItemDicList.Count;
		FightCardExcelData excelDataAsset = ScriptableObject.CreateInstance<FightCardExcelData>();
		excelDataAsset.items = new FightCardExcelItem[rowCount];

		for(int i = 0; i < rowCount; i++)
		{
			var itemRowDic = allRowItemDicList[i];
			excelDataAsset.items[i] = new FightCardExcelItem();
			excelDataAsset.items[i].id = StringUtility.StringToInt(itemRowDic["id"]);
			excelDataAsset.items[i].Name = itemRowDic["Name"];
			excelDataAsset.items[i].Desc = itemRowDic["Desc"];
			excelDataAsset.items[i].Icon = StringUtility.StringToInt(itemRowDic["Icon"]);
			excelDataAsset.items[i].damage = StringUtility.StringToInt(itemRowDic["damage"]);
		}
		if(!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string fullPath = Path.Combine(excelAssetPath,typeof(FightCardExcelData).Name) + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(fullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset,fullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



