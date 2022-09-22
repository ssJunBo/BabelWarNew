using System.Collections.Generic;

namespace HotFix.Data.Account
{
    /// <summary>
    /// 个人信息 存本地json文件
    /// </summary>
    public class PersonInfo
    {
        public int levelId; // 当前关卡id
        public List<int> heroInfos;// 英雄id
    }
}