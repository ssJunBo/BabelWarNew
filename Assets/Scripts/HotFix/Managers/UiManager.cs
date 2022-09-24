using System.Collections.Generic;
using HotFix.Common;
using HotFix.UIBase;
using Main.Game.Base;
using UnityEngine;

namespace HotFix.Managers
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

        public void PushUi(UiLogicBase ui)
        {
            _uiLogicBaseStack.Push(ui);
        }

        public void Back()
        {
            if (_uiLogicBaseStack.Count > 1)
            {
                UiLogicBase closeUiLogicBase = _uiLogicBaseStack.Pop();
                closeUiLogicBase.Close();

                UiLogicBase openUiLogicBase = _uiLogicBaseStack.Peek();
                openUiLogicBase.Open();
            }
            else
            {
                Debug.Log("Ui 堆栈里无多余界面...");
            }
        }

        public void CloseCurrentUIDialog()
        {
            while (_uiLogicBaseStack.Count > 0)
            {
                UiLogicBase uiLogicBase = _uiLogicBaseStack.Pop();
                uiLogicBase.Close();
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
