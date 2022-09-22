/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using HotFix.Common;
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
	/// 攻击类型
	/// </summary>>
	public NormalAtkType NormalAtkType;
	/// <summary>
	/// 角度大小
	/// </summary>>
	public int Angle;
	/// <summary>
	/// 半径
	/// </summary>>
	public int Radius;
	/// <summary>
	/// 攻击力
	/// </summary>>
	public int Atk;
	/// <summary>
	/// 攻击距离
	/// </summary>>
	public int AtkDistance;
	/// <summary>
	/// 攻击速度
	/// </summary>>
	public float AtkSpeed;
	/// <summary>
	/// 单位类型
	/// </summary>>
	public BattleUnitType UnitType;
	/// <summary>
	/// 血量
	/// </summary>>
	public int Hp;
	/// <summary>
	/// 防御
	/// </summary>>
	public float Def;
	/// <summary>
	/// 移动速度
	/// </summary>>
	public int MoveSpeed;
	/// <summary>
	/// 路径
	/// </summary>>
	public int PathId;
	/// <summary>
	/// 技能
	/// </summary>>
	public int[] SkillIds;
	/// <summary>
	/// 额外参数
	/// </summary>>
	public string param;
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

	public NormalAtkType GetNormalAtkType(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.NormalAtkType;
	}

	public int GetAngle(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.Angle;
	}

	public int GetRadius(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.Radius;
	}

	public int GetAtk(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.Atk;
	}

	public int GetAtkDistance(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.AtkDistance;
	}

	public float GetAtkSpeed(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.AtkSpeed;
	}

	public BattleUnitType GetUnitType(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.UnitType;
	}

	public int GetHp(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.Hp;
	}

	public float GetDef(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.Def;
	}

	public int GetMoveSpeed(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.MoveSpeed;
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

	public string GetParam(int id)
	{
		var item = GetExcelItem(id) as BattleUnitExcelItem;
		if(item == null)
			return default;
		return item.param;
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
			excelDataAsset.items[i].NormalAtkType = StringUtility.StringToEnum<NormalAtkType>(itemRowDic["NormalAtkType"]);
			excelDataAsset.items[i].Angle = StringUtility.StringToInt(itemRowDic["Angle"]);
			excelDataAsset.items[i].Radius = StringUtility.StringToInt(itemRowDic["Radius"]);
			excelDataAsset.items[i].Atk = StringUtility.StringToInt(itemRowDic["Atk"]);
			excelDataAsset.items[i].AtkDistance = StringUtility.StringToInt(itemRowDic["AtkDistance"]);
			excelDataAsset.items[i].AtkSpeed = StringUtility.StringToFloat(itemRowDic["AtkSpeed"]);
			excelDataAsset.items[i].UnitType = StringUtility.StringToEnum<BattleUnitType>(itemRowDic["UnitType"]);
			excelDataAsset.items[i].Hp = StringUtility.StringToInt(itemRowDic["Hp"]);
			excelDataAsset.items[i].Def = StringUtility.StringToFloat(itemRowDic["Def"]);
			excelDataAsset.items[i].MoveSpeed = StringUtility.StringToInt(itemRowDic["MoveSpeed"]);
			excelDataAsset.items[i].PathId = StringUtility.StringToInt(itemRowDic["PathId"]);
			excelDataAsset.items[i].SkillIds = StringUtility.StringToIntArray(itemRowDic["SkillIds"]);
			excelDataAsset.items[i].param = itemRowDic["param"];
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



