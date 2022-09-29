using System.Collections.Generic;
using UnityEngine;

namespace UIExtension.ScrollRectExt
{
    public class ExpandTipsCircularScrollView : UiCircularScrollView
    {
        public GameObject expandTips;
        public GameObject mArrow;
        public float tipSpacing;

        private float _expandTipsHeight;
        private bool _isExpand;

        public override void Init()
        {
            base.Init();
            
            expandTips.SetActive(false);
            var rectTrans = expandTips.GetComponent<RectTransform>();
            rectTrans.pivot = new Vector2(0, 1);
            rectTrans.anchorMin = new Vector2(0, 1);
            rectTrans.anchorMax = new Vector2(0, 1);
            rectTrans.anchoredPosition=Vector2.zero;

            _expandTipsHeight = rectTrans.rect.height;
        }

        public override void SetData(List<CellInfo> cellInfos)
        {
            base.SetData(cellInfos);

            _isExpand = false;
        }

        private string _lastClickCellName;
        public void OnClickCell(CellInfo cell)
        {
            _lastClickCellName = cell.LoopItem.gameObject.name;

            int index = int.Parse(_lastClickCellName);
            expandTips.SetActive(true);
            _isExpand = true;

              //-> Tips框 显示
            //m_ExpandTips.SetActive(m_IsExpand);
            expandTips.transform.localPosition = new Vector3(0, -((columSpacing + CellItemHeight) * rowOrColum + tipSpacing), 0);

            //-> Content尺寸 计算
            float contentHeight = _isExpand ? ContentHeight + _expandTipsHeight + tipSpacing : ContentHeight;
            
            contentHeight = contentHeight < ViewHeight ? ViewHeight : contentHeight;
            content.sizeDelta = new Vector2(ContentWidth, contentHeight);

            MinIndex = -1;

            for(int i = 0, length = CellInfos.Count ; i < length; i++)
            {
                CellInfo cellInfo = CellInfos[i];

                float pos = 0;  // Y 坐标
                float rowPos = 0; //计算每排里面的cell 坐标

                pos = CellItemHeight * Mathf.FloorToInt(i / rowOrColum) + columSpacing * (Mathf.FloorToInt(i / rowOrColum) + 1);
                rowPos = CellItemWidth * (i % rowOrColum) + columSpacing * (i % rowOrColum);

                pos += (i/rowOrColum >= rowOrColum && _isExpand) ? _expandTipsHeight + tipSpacing*2 - columSpacing : 0; //往下移 Tips框高 和 距离

                cellInfo.Pos = new Vector3(rowPos, -pos, 0);

                if(IsOutRange(-pos))
                {
                    if(cellInfo.LoopItem != null)
                    {
                        ItemPool.Cycle(cellInfo.LoopItem);
                        cellInfo.LoopItem = null;
                    }
                }
                else
                {
                    //-> 记录显示范围中的 首位index 和 末尾index
                    MinIndex = MinIndex == -1 ? i : MinIndex;// 首位 Index
                    MaxIndex = i; // 末尾 Index

                    LoopItem cellObj = cellInfo.LoopItem == null ? ItemPool.Spawn() : cellInfo.LoopItem;
                    cellObj.GetComponent<RectTransform>().anchoredPosition = cellInfo.Pos;
                    cellInfo.LoopItem = cellObj;
                }

                CellInfos[i] = cellInfo;
            }
        }
        
        
        
   
        
    }
}