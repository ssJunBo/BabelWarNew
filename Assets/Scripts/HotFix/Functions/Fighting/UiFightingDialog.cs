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
        
        private UiFightingLogic _logic;

        public override void Init()
        {
            _logic = (UiFightingLogic)UiLogic;
        }

        public override void ShowFinished()
        {
            InitUI();
        }

        private void InitUI()
        {
            quickFightTxt.text = "x1";
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