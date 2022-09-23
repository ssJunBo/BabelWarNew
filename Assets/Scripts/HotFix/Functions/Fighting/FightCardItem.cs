using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotFix.Functions.Fighting
{
    public class FightCardItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI descTxt;
        [SerializeField] private Image iconImg;
        
        private Transform _originTrs;
        private RectTransform _dragParentRectTrs;

        public void Init(RectTransform dragParentRectTrs)
        {
            _originTrs = transform.parent;
            _dragParentRectTrs = dragParentRectTrs;
        }


        public void SetData(FightCardExcelItem fightCardExcelItem)
        {
            nameTxt.text = fightCardExcelItem.Name;
            descTxt.text = fightCardExcelItem.Desc;
            // iconImg.sprite=
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(_dragParentRectTrs);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_dragParentRectTrs, eventData.position,
                eventData.enterEventCamera, out var uiPosition);
            transform.position = uiPosition; //将当前时间摄像机的拖拽事件的位置赋值给当前UI
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(_originTrs);
        }
    }
}
