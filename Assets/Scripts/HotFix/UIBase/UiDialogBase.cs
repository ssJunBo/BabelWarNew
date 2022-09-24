using HotFix.Managers;
using UnityEngine;

namespace HotFix.UIBase
{
    public abstract class UiDialogBase : MonoBehaviour
    {
        //引用GameObject
        // 界面对应logic
        protected UiLogicBase UiLogic;

        public void SetLogic(UiLogicBase uiLogic)
        {
            UiLogic = uiLogic;
        }

        public abstract void Init();

        public abstract void ShowFinished();

        public virtual void Release() { }

        // 关闭当前界面 并打开上一个界面
        public void Close()
        {
            UiManager.Instance.Back();
        }
    }
}