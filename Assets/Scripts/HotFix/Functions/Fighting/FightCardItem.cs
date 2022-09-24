using System;
using DG.Tweening;
using HotFix.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotFix.Functions.Fighting
{
    public class FightCardItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI descTxt;
        [SerializeField] private Image iconImg;
        [SerializeField] private GameObject effectObj;

        private RectTransform _ownRectTrs;
        private Transform _originTrs;
        private RectTransform _dragParentRectTrs;

        private RectTransform _fightTrs;
        private bool _inFightArea;

        [NonSerialized]public int Index;
        public Action<FightCardItem> RemoveCardAct;
        public Action DragEndAct;
        public Action<bool> InAreaAct;

        public void Init(RectTransform dragParentRectTrs,RectTransform fightTrs)
        {
            _originTrs = transform.parent;
            _fightTrs = fightTrs;
            _dragParentRectTrs = dragParentRectTrs;
            _ownRectTrs = transform.GetComponent<RectTransform>();
        }

        public void SetData(CardExcelItem fightCardExcelItem)
        {
            nameTxt.text = fightCardExcelItem.Name;
            descTxt.text = fightCardExcelItem.Desc;
            iconImg.sprite = AtlasManager.Instance.GetSprite("FightCard", fightCardExcelItem.Icon.ToString());
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_dragParentRectTrs, eventData.position, eventData.enterEventCamera,
                out Vector2 localPoint01);
            
            var ownRect = _ownRectTrs.rect.size;

            if (localPoint01.x < transform.localPosition.x - ownRect.x / 2
                || localPoint01.x > transform.localPosition.x + ownRect.x / 2
                || localPoint01.y < transform.localPosition.y - ownRect.y / 2
                || localPoint01.y > transform.localPosition.y + ownRect.y / 2)
            {
                return;
            }

            if (_isMoving)
            {
                _isMoving = false;
                _tweener.Kill();
            }
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragParentRectTrs, eventData.position, eventData.enterEventCamera, out var uiPosition);
            transform.position = uiPosition; //将当前时间摄像机的拖拽事件的位置赋值给当前UI
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_fightTrs, eventData.position, eventData.enterEventCamera, out Vector2 localPoint);

            var rect = _fightTrs.rect;
            
            var width = rect.width / 2;
            var height = rect.height / 2;

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

        private Tweener _tweener;
        private bool _isMoving = false;
        public void OnPointerDown(PointerEventData eventData)
        {
            var transform1 = transform;
            
            transform1.SetParent(_dragParentRectTrs);

            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragParentRectTrs, eventData.position,
                eventData.enterEventCamera, out var uiPosition);

            
            transform1.position = uiPosition; //将当前时间摄像机的拖拽事件的位置赋值给当前UI
            transform1.localRotation = Quaternion.identity;
            transform1.localScale = Vector3.one * 1.2f;

            _isMoving = true;
            _tweener = transform.DOMoveY(transform1.position.y + 2, 0.5f);
            _tweener.onComplete = () =>
            {
                _isMoving = true;
            };

            effectObj.SetActive(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _tweener.Kill();
            
            InAreaAct?.Invoke(false);

            if (_inFightArea)
            {
                RemoveCardAct?.Invoke(this);
                Destroy(gameObject);
                return;
            }

            transform.SetParent(_originTrs);
            transform.localScale = Vector3.one;
            transform.SetSiblingIndex(Index);
            
            DragEndAct?.Invoke();
            effectObj.SetActive(false);
        }
    }
}
