using System.Collections.Generic;
using HotFix.Common;
using HotFix.Data.Account;
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

        private readonly List<CardExcelItem> _cardExcelItems = new();
        
        public readonly CModelPlay modelPlay;
        public UiFightingLogic(CModelPlay modelPlay)
        {
            this.modelPlay = modelPlay;
        }

        public  List<CardExcelItem> GetCardExcelItems(List<CardInfo> cardInfos)
        {
            _cardExcelItems.Clear();

            var cardIdLis = cardInfos;
            foreach (var cardInfo in cardIdLis)
            {
                var cardItem = ExcelManager.Instance.GetExcelItem<CardExcelData, CardExcelItem>(cardInfo.ID);
                _cardExcelItems.Add(cardItem);
            }

            return _cardExcelItems;
        }
        
    }
}