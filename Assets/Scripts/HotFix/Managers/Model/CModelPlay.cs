using System;
using HotFix.Common;
using HotFix.Functions.Babel;
using HotFix.Functions.Fighting;
using HotFix.Functions.Loading;
using HotFix.Functions.Main;
using HotFix.Functions.PersonDetailInfo;
using HotFix.Functions.UICardPackage;

namespace HotFix.Managers.Model
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
        #endregion

        public void Create()
        {

        }

        public void Release()
        {
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

        public void OpenUiByType(EUiType eUiType)
        {
            switch (eUiType)
            {
                case EUiType.PersonDetailInfo:
                    UiPersonDetailInfoLogic.Open();
                    break;
                case EUiType.Main:
                    UiMainLogic.Open();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eUiType), eUiType, null);
            }
        }
    }
}
