using Managers;
using TMPro;
using UIExtension.ScrollRectExt;
using UnityEngine;
using UnityEngine.UI;

namespace Functions.UICardPackage
{
    public class CardPackageItem : LoopItem
    {
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI descTxt;
        [SerializeField] private Image iconImg;
        
        public override void SetUi(CellInfo cellInfo)
        {
            if (cellInfo is CardItemInfo cardPackageInfo)
            {
                var fightCardExcelItem = cardPackageInfo.FightCardExcelItem;
                
                nameTxt.text = fightCardExcelItem.Name;
                descTxt.text = fightCardExcelItem.Desc;
                iconImg.sprite =
                    AtlasManager.Instance.GetSprite("FightCard", fightCardExcelItem.Icon.ToString());
            }
        }
    }
}
