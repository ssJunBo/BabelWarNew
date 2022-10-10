/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using _GameBase.Excel2Class;
using Common;
[Serializable]
public class BattleUnitExcelItem : ExcelItemBase
{
	/// <summary>
	/// 数据id
	/// </summary>>
	public int id;
	/// <summary>
	/// 名字
	/// </summary>>
	public string Name;
	/// <summary>
	/// 描述
	/// </summary>>
	public string Desc;
	/// <summary>
	/// 攻击类型
	/// </summary>>
	public NormalAtkType NormalAtkType;
	/// <summary>
	/// 单位类型
	/// </summary>>
	public BattleUnitType UnitType;
	/// <summary>
	/// 图片Id
	/// </summary>>
	public int IconId;
	/// <summary>
	/// 路径
	/// </summary>>
	public int PathId;
	/// <summary>
	/// 技能
	/// </summary>>
	public int[] SkillIds;
}


public class BattleUnitExcelData : ExcelDataBase
{
	public BattleUnitExcelItem[] items;

	public Dictionary<int,BattleUnitExcelItem> itemDic = new Dictionary<int,BattleUnitExcelItem>();

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
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.Name;
	}

	public string GetDesc(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.Desc;
	}

	public NormalAtkType GetNormalAtkType(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.NormalAtkType;
	}

	public BattleUnitType GetUnitType(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.UnitType;
	}

	public int GetIconId(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.IconId;
	}

	public int GetPathId(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.PathId;
	}

	public int[] GetSkillIds(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.SkillIds;
	}
	public int GetSkillIds(int id, int index)
	{
		var item0 = GetExcelItem(id) as BattleUnitExcelItem;
		if(item0 == null)
			return default;
		var item1 = item0.SkillIds;
		if(item1 == null || index < 0 || index >= item1.Length)
			return default;
		return item1[index];
	}

	#endregion
}


#if UNITY_EDITOR
public class BattleUnitAssetAssignment
{
	public static bool CreateAsset(ExcelMediumData excelMediumData, string excelAssetPath)
	{
		var allRowItemDicList = excelMediumData.GetAllRowItemDicList();
		if(allRowItemDicList == null || allRowItemDicList.Count == 0)
			return false;

		int rowCount = allRowItemDicList.Count;
		BattleUnitExcelData excelDataAsset = ScriptableObject.CreateInstance<BattleUnitExcelData>();
		excelDataAsset.items = new BattleUnitExcelItem[rowCount];

		for(int i = 0; i < rowCount; i++)
		{
			var itemRowDic = allRowItemDicList[i];
			excelDataAsset.items[i] = new BattleUnitExcelItem();
			excelDataAsset.items[i].id = StringUtility.StringToInt(itemRowDic["id"]);
			excelDataAsset.items[i].Name = itemRowDic["Name"];
			excelDataAsset.items[i].Desc = itemRowDic["Desc"];
			excelDataAsset.items[i].NormalAtkType = StringUtility.StringToEnum<NormalAtkType>(itemRowDic["NormalAtkType"]);
			excelDataAsset.items[i].UnitType = StringUtility.StringToEnum<BattleUnitType>(itemRowDic["UnitType"]);
			excelDataAsset.items[i].IconId = StringUtility.StringToInt(itemRowDic["IconId"]);
			excelDataAsset.items[i].PathId = StringUtility.StringToInt(itemRowDic["PathId"]);
			excelDataAsset.items[i].SkillIds = StringUtility.StringToIntArray(itemRowDic["SkillIds"]);
		}
		if(!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string fullPath = Path.Combine(excelAssetPath,typeof(BattleUnitExcelData).Name) + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(fullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset,fullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



