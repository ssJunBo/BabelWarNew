using Managers;
using TMPro;
using UIExtension.ScrollRectExt;
using UnityEngine;
using UnityEngine.UI;

namespace Functions.UICardPackage
{
    public class HeroCardItem : LoopItem
    {
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI descTxt;
        [SerializeField] private Image iconImg;
        
        public override void SetUi(CellInfo cellInfo)
        {
            if (cellInfo is HeroItemInfo cardPackageInfo)
            {
                var fightCardExcelItem = cardPackageInfo.BattleUnitExcelItem;
                
                nameTxt.text = fightCardExcelItem.Name;
                descTxt.text = fightCardExcelItem.Desc;
                iconImg.sprite = AtlasManager.Instance.GetSprite("FightCard", fightCardExcelItem.IconId.ToString());
            }
        }
    }
}
