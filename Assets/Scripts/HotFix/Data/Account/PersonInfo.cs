using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using HotFix.Managers;
using UnityEngine;

namespace HotFix.Data.Account
{
    /// <summary>
    /// 个人信息 存本地json文件
    /// </summary>
    public class PersonInfo
    {
        public int iconExcelId;
        public string name;
        public int LevelId; // 当前关卡id
        public List<int> HeroInfos; // 英雄id
        public List<CardInfo> OwnCardsList; // 当前卡池信息
    }

    public struct CardInfo
    {
        public int ID;

        // 卡片星级 最高九级卡
        public int StarLev;

        public static int CardDamage(CardInfo info)
        {
            return ExcelManager.Instance.GetExcelData<CardExcelData>().GetDamage(info.ID, info.StarLev - 1);
        }
    }
}