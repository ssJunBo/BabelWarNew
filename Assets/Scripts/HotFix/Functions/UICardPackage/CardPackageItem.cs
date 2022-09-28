using HotFix.Managers;
using HotFix.UIExtension.ScrollRectExt;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.Functions.UICardPackage
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

        public override void OnSpawned()
        {
            base.OnSpawned();
            
            transform.localScale=Vector3.one;
            transform.localRotation=Quaternion.identity;
            
        }
    }
}
