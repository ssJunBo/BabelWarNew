using _GameBase;
using Common;
using Managers;
using Managers.Model;
using UnityEngine;
using UnityEngine.UI;

namespace UIFunctions
{
    public class UiSettingLogic : UiLogicBase
    {
        public override EUiID UiId => EUiID.UISetting;
        protected override EUiLayer UiLayer => EUiLayer.High_2D;
    }

    public class UiSettingDialog : UiDialogBase
    {
        [SerializeField] private Slider _slider;
        public override void Init()
        {
            
        }

        public override void ShowFinished()
        {
            _slider.onValueChanged.AddListener(SliderValChange);
            _slider.value=AudioManager.Instance.GetBgSoundSize();
        }

        private void SliderValChange(float val)
        {
            AudioManager.Instance.SetBgSoundSize(val);
        }
    }
}