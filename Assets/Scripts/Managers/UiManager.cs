using System.Collections.Generic;
using _GameBase;
using _GameBase.UIBase;
using Common;
using Managers.Model;
using UnityEngine;

namespace Managers
{
    public class UiManager : Singleton<UiManager>
    {
        /// <summary>
        /// 打开的UI列表
        /// </summary>
        private readonly Stack<UiLogicBase> _uiLogicBaseStack = new();

        /// <summary>
        /// 打开过的dialog预制体缓存池 缓存所有dialog预制体
        /// </summary>
        private readonly Dictionary<EUiID, UiDialogBase> _dialogDict = new();

        private CModelPlay _modelPlay;
        private CModelPlay ModelPlay => _modelPlay ?? GameManager.Instance.ModelPlay;
        public void PushUi(UiLogicBase ui)
        {
            _uiLogicBaseStack.Push(ui);
        }

        public void OpenUi(EUiID uiID, params object[] data)
        {
            switch (uiID)
            {
                case EUiID.UiMain:
                    ModelPlay.UiMainLogic.Open();
                    break;
                case EUiID.UiPersonDetailInfo:
                    ModelPlay.UiPersonDetailInfoLogic.Open();
                    break;
                case EUiID.UiFighting:
                    ModelPlay.UiFightingLogic.Open((int)data[0]);
                    break;
                case EUiID.UiBabel:
                    ModelPlay.UiBabelLogic.Open();
                    break;
                case EUiID.UICardPackage:
                    ModelPlay.UiCardPackageLogic.Open();
                    break;
                case EUiID.UiSetting:
                    ModelPlay.UiSettingLogic.Open();
                    break;
            }
        }

        public void CloseUi(UiLogicBase uiLogicBase)
        {
            var stackTopUI = _uiLogicBaseStack.Pop();
            stackTopUI.Release();

            if (stackTopUI != uiLogicBase)
            {
                Debug.LogError("UIManager 逻辑出问题啦！");
            }
        }

        public void CloseAllUiDialog()
        {
            while (_uiLogicBaseStack.Count>0)
            {
                UiLogicBase closeUiLogicBase = _uiLogicBaseStack.Pop();
                closeUiLogicBase.Release();
            }
        }

        public void CloseCurrentUIDialog()
        {
            while (_uiLogicBaseStack.Count > 0)
            {
                UiLogicBase uiLogicBase = _uiLogicBaseStack.Pop();
                uiLogicBase.Release();
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

        /// <summary>
        /// 移除dialog
        /// </summary>
        public void RemoveUiDialog(EUiID uiID)
        {
            if (_dialogDict.ContainsKey(uiID))
            {
                _dialogDict.Remove(uiID);
            }
        }
    }
}
