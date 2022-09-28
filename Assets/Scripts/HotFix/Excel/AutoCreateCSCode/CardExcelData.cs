/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using HotFix.Common;
using Main.Game.Excel2Class;
[Serializable]
public class CardExcelItem : ExcelItemBase
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
	public int[] damage;
}


public class CardExcelData : ExcelDataBase
{
	public CardExcelItem[] items;

	public Dictionary<int,CardExcelItem> itemDic = new Dictionary<int,CardExcelItem>();

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
		var item = GetExcelItem(id) as CardExcelItem;
		if(item == null)
			return default;
		return item.Name;
	}

	public string GetDesc(int id)
	{
		var item = GetExcelItem(id) as CardExcelItem;
		if(item == null)
			return default;
		return item.Desc;
	}

	public int GetIcon(int id)
	{
		var item = GetExcelItem(id) as CardExcelItem;
		if(item == null)
			return default;
		return item.Icon;
	}

	public int[] GetDamage(int id)
	{
		var item = GetExcelItem(id) as CardExcelItem;
		if(item == null)
			return default;
		return item.damage;
	}
	public int GetDamage(int id, int index)
	{
		var item0 = GetExcelItem(id) as CardExcelItem;
		if(item0 == null)
			return default;
		var item1 = item0.damage;
		if(item1 == null || index < 0 || index >= item1.Length)
			return default;
		return item1[index];
	}

	#endregion
}


#if UNITY_EDITOR
public class CardAssetAssignment
{
	public static bool CreateAsset(ExcelMediumData excelMediumData, string excelAssetPath)
	{
		var allRowItemDicList = excelMediumData.GetAllRowItemDicList();
		if(allRowItemDicList == null || allRowItemDicList.Count == 0)
			return false;

		int rowCount = allRowItemDicList.Count;
		CardExcelData excelDataAsset = ScriptableObject.CreateInstance<CardExcelData>();
		excelDataAsset.items = new CardExcelItem[rowCount];

		for(int i = 0; i < rowCount; i++)
		{
			var itemRowDic = allRowItemDicList[i];
			excelDataAsset.items[i] = new CardExcelItem();
			excelDataAsset.items[i].id = StringUtility.StringToInt(itemRowDic["id"]);
			excelDataAsset.items[i].Name = itemRowDic["Name"];
			excelDataAsset.items[i].Desc = itemRowDic["Desc"];
			excelDataAsset.items[i].Icon = StringUtility.StringToInt(itemRowDic["Icon"]);
			excelDataAsset.items[i].damage = StringUtility.StringToIntArray(itemRowDic["damage"]);
		}
		if(!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string fullPath = Path.Combine(excelAssetPath,typeof(CardExcelData).Name) + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(fullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset,fullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



