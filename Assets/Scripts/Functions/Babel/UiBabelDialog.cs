using _GameBase.UIBase;
using Common;
using Managers.Model;
using UnityEngine;

namespace Functions.Babel
{
    public class UiBabelLogic: UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UIBabel/UiBabelDialog";
        protected override EUiID UiId  => EUiID.Babel;
        protected override EUiLayer UiLayer => EUiLayer.High_2D;

        public CModelPlay modelPlay;
        public UiBabelLogic(CModelPlay modelPlay)
        {
            this.modelPlay = modelPlay;
        }
        
        public override void Open()
        {
            base.Open();
        }
    }
    
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
