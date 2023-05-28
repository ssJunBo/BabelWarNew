using _GameBase.UIBase;
using Common;
using Managers;
using Managers.Model;
using UnityEngine;
using UnityEngine.UI;

namespace UIFunctions.Setting
{
    public class UiSettingLogic : UiLogicBase
    {
        public override EUiID UiId => EUiID.UiSetting;
        protected override EUiLayer UiLayer => EUiLayer.High_2D;
        
        private readonly CModelPlay _model;

        public UiSettingLogic(CModelPlay model)
        {
            _model = model;
        }
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