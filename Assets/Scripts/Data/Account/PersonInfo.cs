using System.Collections.Generic;
using Helpers;
using Managers;

namespace Data.Account
{
    /// <summary>
    /// 个人信息 存本地json文件
    /// </summary>
    public class PersonInfo
    {
        public int IconExcelId;
        public string Name;
        public int LevelId; // 当前通过最高关卡 id
        public List<int> HeroInfos; // 拥有英雄 id
        public List<CardInfo> OwnCardsList; // 当前拥有卡池信息
    }

    public struct CardInfo
    {
        // 前三位卡片id 后两位卡片星级 最高九级卡
        public int CombineId;

        public int StarLev=>IDParseHelp.GetCardLev(CombineId);

        public static int CardDamage(CardInfo info)
        {
            return ExcelManager.Instance.GetExcelData<CardExcelData>().GetDamage(info.CombineId, info.StarLev - 1);
        }
    }
}