using System;
using HotFix.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotFix.Functions.Fighting
{
    public class FightCardItem : MonoBehaviour,  IDragHandler,IPointerDownHandler,IPointerUpHandler
    {
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI descTxt;
        [SerializeField] private Image iconImg;
        [SerializeField] private GameObject effectObj;

        private Transform _originTrs;
        private RectTransform _dragParentRectTrs;

        private RectTransform _fightTrs;
        private int _index;
        private bool _inFightArea;

        public Action<FightCardItem> RemoveCardAct;
        public Action DragEndAct;
        public Action<bool> InAreaAct;
        public void Init(RectTransform dragParentRectTrs)
        {
            _originTrs = transform.parent;
            _dragParentRectTrs = dragParentRectTrs;
        }

        public void SetData(CardExcelItem fightCardExcelItem,int index,RectTransform fightTrs)
        {
            _fightTrs = fightTrs;
            _index = index;
            nameTxt.text = fightCardExcelItem.Name;
            descTxt.text = fightCardExcelItem.Desc;
            iconImg.sprite = AtlasManager.Instance.GetSprite("FightCard", fightCardExcelItem.Icon.ToString());
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragParentRectTrs, eventData.position,
                eventData.enterEventCamera, out var uiPosition);
            transform.position = uiPosition; //将当前时间摄像机的拖拽事件的位置赋值给当前UI

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_fightTrs, screenPos, Camera.main,
                out Vector2 localPoint);

            var width = _fightTrs.rect.width / 2;
            var height = _fightTrs.rect.height / 2;

            if (localPoint.x > -width 
                && localPoint.x < width
                && localPoint.y > -height 
                && localPoint.y < height)
            {
                _inFightArea = true;
                InAreaAct?.Invoke(true);
            }
            else
            {
                _inFightArea = false;
                InAreaAct?.Invoke(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.SetParent(_dragParentRectTrs);
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragParentRectTrs, eventData.position,
                eventData.enterEventCamera, out var uiPosition);
            
            transform.position = uiPosition; //将当前时间摄像机的拖拽事件的位置赋值给当前UI
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 1.2f;

            effectObj.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InAreaAct?.Invoke(false);
            
            if (_inFightArea)
            {
                RemoveCardAct?.Invoke(this);
                Destroy(gameObject);
                return;
            }
            
            transform.SetParent(_originTrs);
            transform.transform.localScale = Vector3.one;
            transform.SetSiblingIndex(_index);
            DragEndAct?.Invoke();
            effectObj.SetActive(false);
        }
    }
}
