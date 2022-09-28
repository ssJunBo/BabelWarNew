using System.Collections.Generic;
using HotFix.Common;
using HotFix.Managers;
using HotFix.Managers.Model;
using HotFix.UIBase;
using HotFix.UIExtension.ScrollRectExt;
using TMPro;
using UnityEngine;

namespace HotFix.Functions.UICardPackage
{
    public class UICardPackageLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UICardPackage/UICardPackageDialog";
        public override EUiID UiId => EUiID.CardPackage;

        protected override EUiLayer UiLayer => EUiLayer.High_2D;

        private readonly CModelPlay _model;

        public UICardPackageLogic(CModelPlay model)
        {
            _model = model;
        }

        public List<CellInfo> GenerateCardCellInfo()
        {
            List<CellInfo> list = new List<CellInfo>();

            foreach (var cardInfo in DataManager.Instance.OwnCardsList)
            {
                CellInfo cardItemInfo = new CardItemInfo
                {
                    FightCardExcelItem = CardManager.Instance.GetCardExcelItem(cardInfo)
                };

                list.Add(cardItemInfo);
            }

            return list;
        }
    }

    public class CardItemInfo : CellInfo
    {
        public CardExcelItem FightCardExcelItem;
    }

    public class UICardPackageDialog : UiDialogBase
    {
        [SerializeField] private UiCircularSv scrollView;

        private UICardPackageLogic _uiLogic;

        #region override

        public override void Init()
        {
            _uiLogic = (UICardPackageLogic)UiLogic;
        }

        public override void ShowFinished()
        {
            var data = _uiLogic.GenerateCardCellInfo();

            Debug.LogError("var data = _uiLogic.GenerateCardCellInfo();   " + data.Count);
            
            scrollView.Init();
            scrollView.SetData(data);
        }

        #endregion

        #region btn event

        public void OnClickQuitDialog()
        {
            _uiLogic.Close();
        }

        #endregion
    }
}
