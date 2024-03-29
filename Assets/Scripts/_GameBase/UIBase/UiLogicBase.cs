﻿using System.Collections.Generic;
using Common;
using ET;
using Managers;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _GameBase
{
    public abstract class UiLogicBase
    {
        // 具体ui
        public abstract EUiID UiId { get; }
        protected virtual EUiLayer UiLayer => EUiLayer.Low_2D;

        private bool _isShowing;
        private GameObject _mObj;
        private UiDialogBase _mDialog;

        public int layerDepth;
        
        // 打开界面传入参数
        public object[] data;

        //注册游戏逻辑的委托事件
        protected virtual void InitLogic()
        {

        }

        public virtual void Open(params object[] data)
        {
            this.data = data;
            
            if (_isShowing) return;

            DoOpen();
        }

        public virtual void Close()
        {
            if (_mDialog != null)
            {
                _isShowing = false;
                _mDialog.Release();
            }

            if (_mObj != null)
            {
                Object.Destroy(_mObj);
            }

            data = null;

            UIManager.Instance.RemoveUi(this);

            UnLoadAsync().Coroutine();
        }

        // 已加载的ab资源
        private List<string> LoadedResource = new();
        
        private async ETTask UnLoadAsync()
        {
            using (ListComponent<string> list = ListComponent<string>.Create())
            {
                list.AddRange(LoadedResource);
                LoadedResource = null;

                if (TimerComponent.Instance == null)
                {
                    return;
                }

                Log.Info("关闭界面延时5秒卸载ab资源");

                // 延迟5秒卸载包，因为包卸载是引用计数，5秒之内假如重新有逻辑加载了这个包，那么可以避免一次卸载跟加载
                await TimerComponent.Instance.WaitAsync(5000);

                Log.Info("关闭界面开始卸载ab资源");

                foreach (string abName in list)
                {
                    using CoroutineLock coroutineLock = 
                        await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ResourcesLoader, abName.GetHashCode(), 0);
                    {
                        if (ResourcesComponent.Instance == null)
                        {
                            return;
                        }

                        await ResourcesComponent.Instance.UnloadBundleAsync(abName);
                    }
                }
            }
        }
        
        //实际打开
        private async void DoOpen()
        {
            _isShowing = true;

            Log.Info("生成预支体前时间 " + Time.realtimeSinceStartup);

            await ResourcesComponent.Instance.LoadBundleAsync(UiId.ToString().ToLower());

            Log.Info("生成预支体后时间 " + Time.realtimeSinceStartup);

            Object obj =
                ResourcesComponent.Instance.GetAsset(UiId.ToString().ToLower(), UiId + "Dialog");

            HandleUiResourceOk(obj);
        }

        private void HandleUiResourceOk(Object obj)
        {
            if (!_isShowing) return;

            if (obj != null)
            {
                var parentTrs = GetParentTrs();
                _mDialog = UIManager.Instance.GetUiDialog(UiId);

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
                        Debug.LogError("加载窗口失败！");
                        return;
                    }

                    _mDialog = _mObj.GetComponent<UiDialogBase>();

                    if (_mDialog == null)
                    {
                        Debug.LogError("cant find designer component : " + obj.name);
                        return;
                    }

                    UIManager.Instance.AddUiDialog(UiId, _mDialog);
                }

                InitLogic();

                _mDialog.SetLogic(this);

                _mDialog.Init();

                //延迟一帧，当ui真正绘制出来以后，在调用ShowFinished 这样一些坐标转换，和一些UI操作才不会出错
                //UI的显示操作都应该放在ShowFinished中去做，而不应该在Init中去做 
                TimerEventManager.Instance.DelayFrames(1, () => { _mDialog.ShowFinished(); });

                UIManager.Instance.AddUi(this);
                
                var canvas =  _mDialog.GetComponent<Canvas>();

                int layerDepth = 0;
                var uiLogicBase = UIManager.Instance.GetTopUiLogicBase();
                if (uiLogicBase != null)
                {
                    layerDepth = uiLogicBase.layerDepth;
                }
                
                canvas.overrideSorting = true;
                canvas.sortingOrder = ++layerDepth;
            }
        }

        private Transform GetParentTrs()
        {
            Transform parentTrs = null;
            switch (UiLayer)
            {
                case EUiLayer.Low_2D:
                    parentTrs = GameManager.Instance.ui2DTrsLow;
                    break;
                case EUiLayer.High_2D:
                    parentTrs = GameManager.Instance.ui2DTrsHigh;
                    break;
            }
            
            return parentTrs;
        }
        
    }
}
