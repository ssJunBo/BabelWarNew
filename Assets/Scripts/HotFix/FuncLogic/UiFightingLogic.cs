using System.Collections.Generic;
using HotFix.Common;
using HotFix.Managers;
using HotFix.Managers.Model;
using HotFix.UIBase;

namespace HotFix.FuncLogic
{
    public class UiFightingLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UIFight/UiFightingDialog";
        public override EUiID UiId => EUiID.Fighting;

        protected override EUiLayer UiLayer => EUiLayer.High_2D;

        public readonly List<CardExcelItem> CardExcelItems = new();
        
        public CModelPlay modelPlay;
        public UiFightingLogic(CModelPlay modelPlay)
        {
            this.modelPlay = modelPlay;
        }
        
        public override void Open()
        {
            base.Open();

            InitData();
        }

        private void InitData()
        {
            CardExcelItems.Clear();

            var cardIdLis = UserDataManager.Instance.PersonInfo.cardsList;
            foreach (var cardInfo in cardIdLis)
            {
                var cardItem = ExcelManager.Instance.GetExcelItem<CardExcelData, CardExcelItem>(cardInfo.id);
                CardExcelItems.Add(cardItem);
            }
        }
        
    }
}