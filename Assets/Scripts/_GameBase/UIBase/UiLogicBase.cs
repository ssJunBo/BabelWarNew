using Common;
using Managers;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _GameBase.UIBase
{
    public abstract class UiLogicBase
    {
        protected abstract string Path { get; }
        // 具体ui
        protected abstract EUiID UiId { get; }
        protected virtual EUiLayer UiLayer => EUiLayer.Low_2D;

        private bool _isShowing;
        private GameObject _mObj;
        private UiDialogBase _mDialog;

        private GameManager _gameManager;        
        public virtual void Open()
        {
            if (_isShowing) return;
            _gameManager = GameManager.Instance;
            
            DoOpen();
        }

        public virtual void Close()
        {
            UiManager.Instance.CloseUi(this);
        }

        public void Release()
        {
            if (_mDialog != null)
            {
                _isShowing = false;
                _mDialog.Release();
            }

            if (_mObj != null)
            {
                UIUtils.SetActive(_mObj, false);
                _mObj.transform.SetParent(_gameManager.recyclePoolTrs);
                _mObj.transform.localPosition=Vector3.zero;
            }
        }

        //实际打开
        private void DoOpen()
        {
            _isShowing = true;

            HandleUiResourceOk(Path, UnityEngine.Resources.Load(Path));
        }

        private void HandleUiResourceOk(string path, Object obj)
        {
            if (!_isShowing) return;

            if (obj != null)
            {
                var parentTrs = GetParentTrs();
                _mDialog = UiManager.Instance.GetUiDialog(UiId);

                if (_mDialog != null)
                {
                    _mObj = _mDialog.gameObject;
                    UIUtils.SetActive(_mObj, true);
                    _mObj.transform.SetParent(parentTrs);
                    _mObj.transform.localPosition = Vector3.zero;
                    _mObj.transform.SetAsLastSibling();
                }
                else
                {
                    _mObj = Object.Instantiate(obj, parentTrs) as GameObject;
                    if (_mObj == null)
                    {
                        //加载窗口失败，返回初始化失败
                        Debug.LogError("加载窗口失败！path = " + path);
                        return;
                    }

                    _mDialog = _mObj.GetComponent<UiDialogBase>();

                    if (_mDialog == null)
                    {
                        Debug.LogError("cant find designer component : " + obj.name);
                        return;
                    }

                    UiManager.Instance.AddUiDialog(UiId, _mDialog);
                }

                InitLogic();

                _mDialog.SetLogic(this);

                _mDialog.Init();

                //延迟一帧，当ui真正绘制出来以后，在调用ShowFinished 这样一些坐标转换，和一些UI操作才不会出错
                //UI的显示操作都应该放在ShowFinished中去做，而不应该在Init中去做 
                TimerEventManager.Instance.DelayFrames(1, () => { _mDialog.ShowFinished(); });

                UiManager.Instance.PushUi(this);
            }
        }

        private Transform GetParentTrs()
        {
            Transform parentTrs = null;
            switch (UiLayer)
            {
                case EUiLayer.Low_2D:
                    parentTrs = _gameManager.ui2DTrsLow;
                    break;
                case EUiLayer.High_2D:
                    parentTrs = _gameManager.ui2DTrsHigh;
                    break;
            }
            
            return parentTrs;
        }
        
        //注册游戏逻辑的委托事件
        protected virtual void InitLogic()
        {

        }
    }
}
