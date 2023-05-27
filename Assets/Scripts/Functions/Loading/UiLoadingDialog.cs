using System;
using _GameBase.UIBase;
using Common;
using Managers;
using Managers.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Functions.Loading
{
    public class UiLoadingLogic : UiLogicBase
    {
        public override EUiID UiId => EUiID.UiLoading;
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
    
    public class UiLoadingDialog : UiDialogBase
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI progressTxt;
        [SerializeField] private TextMeshProUGUI loadingTxt;

        private UiLoadingLogic _uiLogic;
        private SceneManager _sceneManager;

        public override void Init()
        {
            _uiLogic = (UiLoadingLogic)UiLogic;
            _sceneManager = GameManager.Instance.SceneManager;

            _sceneManager.LoadSceneFinishAct = LoadSceneFinishAct;
            _sceneManager.RefreshProgressAct = RefreshProgress;
        }

        public override void ShowFinished()
        {
            SetUiDefaultInfo();
            _sceneManager.AsyncLoadScene(_uiLogic.sceneName);
        }

        private void SetUiDefaultInfo()
        {
            slider.value = 0;
            progressTxt.text = "0%";
            loadingTxt.text = "Loading";
        }

        private void RefreshProgress(float progressVal)
        {
            slider.value = progressVal;
            progressTxt.text = $"{(int)(progressVal * 100)}%";
        }

        private void LoadSceneFinishAct()
        {
            // 等一帧 在调用 要展示界面
            TimerEventManager.Instance.DelayFrames(2, () =>
            {
                _uiLogic.Close();
                _uiLogic.SceneOpenFinishAct?.Invoke();
            });
        }
    }
}
