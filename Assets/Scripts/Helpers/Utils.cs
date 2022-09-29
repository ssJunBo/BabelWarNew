using Managers;
using UnityEngine;

namespace Helpers
{
    public static class Utils
    {
        public static Sprite GetSprite(int iconExcelId)
        {
            int iconId = DataManager.Instance.PersonInfo.iconExcelId;
            var iconExcelItem = ExcelManager.Instance.GetExcelItem<IconExcelData, IconExcelItem>(iconId);

            return Resources.Load<Sprite>(iconExcelItem.iconPath);
        }
    }
}