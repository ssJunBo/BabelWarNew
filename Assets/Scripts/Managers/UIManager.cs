using System.Collections.Generic;
using System.Linq;
using _GameBase;
using _GameBase.UIBase;
using Common;
using Managers.Model;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// 打开的UI列表
        /// </summary>
        private readonly List<UiLogicBase> _uiLogicBaseLis = new();

        /// <summary>
        /// 打开过的dialog预制体缓存池 缓存所有dialog预制体
        /// </summary>
        private readonly Dictionary<EUiID, UiDialogBase> _dialogDict = new();

        private CModelPlay _modelPlay;
        private CModelPlay ModelPlay => _modelPlay ?? GameManager.Instance.ModelPlay;
        
        public void AddUi(UiLogicBase ui)
        {
            _uiLogicBaseLis.Add(ui);
        }

        public void OpenUi(EUiID uiID, params object[] data)
        {
            switch (uiID)
            {
                case EUiID.UiMain:
                    ModelPlay.UiMainLogic.Open(data);
                    break;
                case EUiID.UiPersonDetailInfo:
                    ModelPlay.UiPersonDetailInfoLogic.Open(data);
                    break;
                case EUiID.UiFighting:
                    ModelPlay.UiFightingLogic.Open(data);
                    break;
                case EUiID.UiBabel:
                    ModelPlay.UiBabelLogic.Open(data);
                    break;
                case EUiID.UICardPackage:
                    ModelPlay.UiCardPackageLogic.Open(data);
                    break;
                case EUiID.UiSetting:
                    ModelPlay.UiSettingLogic.Open(data);
                    break;
            }
        }

        public void RemoveUi(UiLogicBase uiLogicBase)
        {
            _uiLogicBaseLis.Remove(uiLogicBase);
            if (_dialogDict.ContainsKey(uiLogicBase.UiId))
            {
                _dialogDict.Remove(uiLogicBase.UiId);
            }
        }

        // 关闭所有界面 保留主界面
        public void CloseAllUiDialog()
        {
            while (_uiLogicBaseLis.Count>1)
            {
                UiLogicBase closeUiLogicBase = _uiLogicBaseLis.Last();
                closeUiLogicBase.Close();
            }
        }

        public UiDialogBase GetUiDialog(EUiID uiID)
        {
            return _dialogDict.ContainsKey(uiID) ? _dialogDict[uiID] : null;
        }

        /// <summary>
        /// 添加dialog
        /// </summary>
        /// <param name="uiID"></param>
        /// <param name="uiDialogBase"></param>
        public void AddUiDialog(EUiID uiID, UiDialogBase uiDialogBase)
        {
            if (!_dialogDict.ContainsKey(uiID))
            {
                _dialogDict[uiID] = uiDialogBase;
            }
        }
    }
}
