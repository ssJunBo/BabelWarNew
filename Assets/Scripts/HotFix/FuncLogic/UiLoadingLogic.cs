using System;
using HotFix.Common;
using HotFix.Managers;
using HotFix.Managers.Model;
using HotFix.UIBase;

namespace HotFix.FuncLogic
{
    public class UiLoadingLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UiLoadingDialog";
        public override EUiID UiId => EUiID.Loading;
        protected override EUiLayer UiLayer => EUiLayer.High_2D;
        
        #region data
        private CModelPlay modelPlay;
        public string sceneName;
        #endregion

        public Action SceneOpenFinishAct { get; private set; }

        public UiLoadingLogic(CModelPlay modelPlay)
        {
            this.modelPlay = modelPlay;
        }
        public void Open(string sceneName,Action sceneOpenFinishAct)
        {
            this.sceneName = sceneName;
            UiManager.Instance.CloseCurrentUIDialog();
            SceneOpenFinishAct = sceneOpenFinishAct;
            Open();
        }
    }
}
