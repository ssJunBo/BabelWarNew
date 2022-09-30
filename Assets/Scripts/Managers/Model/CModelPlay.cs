using System;
using Common;
using Functions.Babel;
using Functions.Fighting;
using Functions.Loading;
using Functions.Main;
using Functions.PersonDetailInfo;
using Functions.Setting;
using Functions.UICardPackage;

namespace Managers.Model
{
    public class CModelPlay : IModel
    {
        #region 所有 UiLogic 数据类

        private UiPersonDetailInfoLogic _uiPersonDetailInfoLogic;
        public UiPersonDetailInfoLogic UiPersonDetailInfoLogic => _uiPersonDetailInfoLogic ??= new UiPersonDetailInfoLogic(this);

        private UiMainLogic _uiMainLogic;
        public UiMainLogic UiMainLogic => _uiMainLogic ??= new UiMainLogic(this);

        private UiLoadingLogic _uiLoadingLogic;
        public UiLoadingLogic UiLoadingLogic => _uiLoadingLogic ??= new UiLoadingLogic(this);

        private UiBabelLogic _babelLogic;
        public UiBabelLogic UiBabelLogic => _babelLogic ??= new UiBabelLogic(this);
        
        private UiFightingLogic _uiFightingLogic;
        public UiFightingLogic UiFightingLogic => _uiFightingLogic ??= new UiFightingLogic(this);
        
        private UICardPackageLogic _uiCardPackageLogic;
        public UICardPackageLogic UICardPackageLogic => _uiCardPackageLogic ??= new UICardPackageLogic(this);
        
        private UiSettingLogic _uiSettingLogic;
        public UiSettingLogic UiSettingLogic => _uiSettingLogic ??= new UiSettingLogic(this);
        #endregion

        public void Create()
        {

        }

        public void Release()
        {
            // 释放所有logic TODO
            if (_uiPersonDetailInfoLogic != null)
            {
                _uiPersonDetailInfoLogic.Close();
                _uiPersonDetailInfoLogic = null;
            }
        }

        public void Update(float fDeltaTime)
        {

        }

        public void LateUpdate()
        {

        }

        public void OnApplicationPause(bool paused)
        {

        }
    }
}
