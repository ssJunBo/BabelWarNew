using Managers;
using UnityEngine;

namespace Helpers
{
    public static class Utils
    {
        public static Sprite GetSprite(int iconExcelId)
        {
            int iconId = DataManager.Instance.PersonInfo.IconExcelId;
            var iconExcelItem = ExcelManager.Instance.GetExcelItem<IconExcelData, IconExcelItem>(iconId);

            return default;
            
            return UnityEngine.Resources.Load<Sprite>(iconExcelItem.iconPath);
        }
    }
}