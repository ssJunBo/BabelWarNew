using _GameBase.UIBase;
using Common;
using Managers;
using Managers.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Functions.Setting
{
    public class UiSettingLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/Setting/UiSettingDialog";
        protected override EUiID UiId => EUiID.Setting;
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