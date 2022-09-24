using HotFix.FuncLogic;
using HotFix.Managers;
using HotFix.UIBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.Functions.Loading
{
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
