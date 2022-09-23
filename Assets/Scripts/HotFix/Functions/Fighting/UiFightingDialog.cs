using System.Collections.Generic;
using HotFix.FuncLogic;
using HotFix.Managers;
using HotFix.SystemTools.EventSys;
using Main.Game.Base;
using TMPro;
using UnityEngine;
using HotFix.Common;
using HotFix.UIBase;

namespace HotFix.Functions.Fighting
{
    public class UiFightingDialog : UiDialogBase
    {
        [SerializeField] private TextMeshProUGUI quickFightTxt;
        [SerializeField] private RectTransform cardMoveTrs;
        [SerializeField] private Transform contentTrs;
        [SerializeField] private FightCardItem fightCardItemPre;

        private UiFightingLogic _logic;

        #region Override

        public override void Init()
        {
            _logic = (UiFightingLogic)UiLogic;
        }

        public override void ShowFinished()
        {
            InitUI();
        }

        public override void Release()
        {
            for (int i = _cardItems.Count - 1; i >= 0; i--)
            {
                Destroy(_cardItems[i].gameObject);
            }

            _cardItems = null;
            
            base.Release();
        }

        #endregion

        private List<FightCardItem> _cardItems;

        private void InitUI()
        {
            quickFightTxt.text = "x1";

            FightCardExcelData allFightCard = ExcelManager.Instance.GetExcelData<FightCardExcelData>();

            _cardItems = new List<FightCardItem>(allFightCard.items.Length);

            foreach (var fightCardExcelItem in allFightCard.items)
            {
                FightCardItem fightCardItem = Instantiate(fightCardItemPre, contentTrs);
                fightCardItem.Init(cardMoveTrs);
                fightCardItem.SetData(fightCardExcelItem);
                _cardItems.Add(fightCardItem);
            }
        }

        public void QuickFight()
        {
            FightManager.Instance.OpenQuickFight = !FightManager.Instance.OpenQuickFight;

            EventManager.DispatchEvent(EventMessageType.ChangeTimeScale, FightManager.Instance.OpenQuickFight ? 2 : 1);
            quickFightTxt.text = FightManager.Instance.OpenQuickFight ? "x2" : "x1";
        }

        public void QuitFightScene()
        {
            _logic.modelPlay.UiLoadingLogic.Open(ConStr.Main, () =>
            {
                EventManager.DispatchEvent(EventMessageType.ChangeTimeScale, 1);
                GameManager.Instance.ModelPlay.UiMainLogic.Open();
            });
            _logic.Close();
        }
    }
}