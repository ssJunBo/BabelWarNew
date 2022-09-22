using HotFix.FuncLogic;
using HotFix.UIBase;
using UnityEngine;

namespace HotFix.Functions.Babel
{
    public class UiBabelDialog : UiDialogBase
    {
        [SerializeField] private Transform contentTrs;
        [SerializeField] private BabelItem babelItemPrefab;


        private UiBabelLogic logic;

        public override void Init()
        {
            logic = (UiBabelLogic)UiLogic;
        }

        public override void ShowFinished()
        {
            SetData();
        }

        private void SetData()
        {
            for (int i = 0; i < 20; i++)
            {
                BabelItem go = Instantiate(babelItemPrefab, contentTrs);
                go.Init(logic);
                go.SetData(i);
            }
        }
    }
}
