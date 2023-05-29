using Managers;
using TMPro;
using UIExtension.ScrollRectExt;
using UnityEngine;
using UnityEngine.UI;

namespace UIFunctions
{
    public class SkillCardItem : LoopItem
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

                int lev = cardPackageInfo.cardLev;
                
                if (fightCardExcelItem.paramCount==1)
                {
                    descTxt.text = string.Format(fightCardExcelItem.Desc, fightCardExcelItem.Param1[lev - 1]);
                }
                else
                {
                    descTxt.text = string.Format(fightCardExcelItem.Desc, fightCardExcelItem.Param1[lev - 1],
                        fightCardExcelItem.Param2[lev - 1]);
                }

                iconImg.sprite =
                    AtlasManager.Instance.GetSprite("FightCard", fightCardExcelItem.Icon.ToString());
            }
        }
    }
}
