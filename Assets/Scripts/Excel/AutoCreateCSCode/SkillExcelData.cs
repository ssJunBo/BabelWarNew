/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using _GameBase.Excel2Class;
using Common;
[Serializable]
public class SkillExcelItem : ExcelItemBase
{
	/// <summary>
	/// 数据id
	/// </summary>>
	public int id;
	/// <summary>
	/// 技能名字
	/// </summary>>
	public string Name;
	/// <summary>
	/// 技能类型
	/// </summary>>
	public SkillType SkillType;
	/// <summary>
	/// 开启等级
	/// </summary>>
	public int OpenLevel;
	/// <summary>
	/// 充能次数
	/// </summary>>
	public int ChargeCount;
	/// <summary>
	/// CD时间
	/// </summary>>
	public float CDTime;
	/// <summary>
	/// Buff类型
	/// </summary>>
	public BuffType BuffType;
	/// <summary>
	/// 技能形状
	/// </summary>>
	public SkillShape SkillShape;
	/// <summary>
	/// 角度大小
	/// </summary>>
	public int Angle;
	/// <summary>
	/// 半径
	/// </summary>>
	public int Radius;
	/// <summary>
	/// 伤害
	/// </summary>>
	public int Damages;
	/// <summary>
	/// 不同属性对应不同定义
	/// </summary>>
	public string param;
}


public class SkillExcelData : ExcelDataBase
{
	public SkillExcelItem[] items;

	public Dictionary<int,SkillExcelItem> itemDic = new Dictionary<int,SkillExcelItem>();

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
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.Name;
	}

	public SkillType GetSkillType(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.SkillType;
	}

	public int GetOpenLevel(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.OpenLevel;
	}

	public int GetChargeCount(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.ChargeCount;
	}

	public float GetCDTime(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.CDTime;
	}

	public BuffType GetBuffType(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.BuffType;
	}

	public SkillShape GetSkillShape(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.SkillShape;
	}

	public int GetAngle(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.Angle;
	}

	public int GetRadius(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.Radius;
	}

	public int GetDamages(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.Damages;
	}

	public string GetParam(int id)
	{
		var item = GetExcelItem(id) as SkillExcelItem;
		if(item == null)
			return default;
		return item.param;
	}

	#endregion
}


#if UNITY_EDITOR
public class SkillAssetAssignment
{
	public static bool CreateAsset(ExcelMediumData excelMediumData, string excelAssetPath)
	{
		var allRowItemDicList = excelMediumData.GetAllRowItemDicList();
		if(allRowItemDicList == null || allRowItemDicList.Count == 0)
			return false;

		int rowCount = allRowItemDicList.Count;
		SkillExcelData excelDataAsset = ScriptableObject.CreateInstance<SkillExcelData>();
		excelDataAsset.items = new SkillExcelItem[rowCount];

		for(int i = 0; i < rowCount; i++)
		{
			var itemRowDic = allRowItemDicList[i];
			excelDataAsset.items[i] = new SkillExcelItem();
			excelDataAsset.items[i].id = StringUtility.StringToInt(itemRowDic["id"]);
			excelDataAsset.items[i].Name = itemRowDic["Name"];
			excelDataAsset.items[i].SkillType = StringUtility.StringToEnum<SkillType>(itemRowDic["SkillType"]);
			excelDataAsset.items[i].OpenLevel = StringUtility.StringToInt(itemRowDic["OpenLevel"]);
			excelDataAsset.items[i].ChargeCount = StringUtility.StringToInt(itemRowDic["ChargeCount"]);
			excelDataAsset.items[i].CDTime = StringUtility.StringToFloat(itemRowDic["CDTime"]);
			excelDataAsset.items[i].BuffType = StringUtility.StringToEnum<BuffType>(itemRowDic["BuffType"]);
			excelDataAsset.items[i].SkillShape = StringUtility.StringToEnum<SkillShape>(itemRowDic["SkillShape"]);
			excelDataAsset.items[i].Angle = StringUtility.StringToInt(itemRowDic["Angle"]);
			excelDataAsset.items[i].Radius = StringUtility.StringToInt(itemRowDic["Radius"]);
			excelDataAsset.items[i].Damages = StringUtility.StringToInt(itemRowDic["Damages"]);
			excelDataAsset.items[i].param = itemRowDic["param"];
		}
		if(!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string fullPath = Path.Combine(excelAssetPath,typeof(SkillExcelData).Name) + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(fullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset,fullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



