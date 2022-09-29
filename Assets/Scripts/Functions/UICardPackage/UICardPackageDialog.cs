using System.Collections.Generic;
using _GameBase.UIBase;
using Common;
using Managers;
using Managers.Model;
using UIExtension.ScrollRectExt;
using UnityEngine;

namespace Functions.UICardPackage
{
    public class UICardPackageLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UICardPackage/UICardPackageDialog";
        protected override EUiID UiId => EUiID.CardPackage;

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
        [SerializeField] private UiCircularScrollView scrollView;

        private UICardPackageLogic _uiLogic;

        #region override

        public override void Init()
        {
            _uiLogic = (UICardPackageLogic)UiLogic;
        }

        public override void ShowFinished()
        {
            var data = _uiLogic.GenerateCardCellInfo();

            scrollView.Init();
            scrollView.SetData(data);
        }

        public override void Release()
        {
            base.Release();

            scrollView.CycleAllItem();
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
