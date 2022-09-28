using System.Collections.Generic;
using HotFix.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.UIExtension.ScrollRectExt
{
    public class UiCircularScrollView : ScrollRect
    {
        public LoopItem cellItemPre;
        public MoveType moveType;
        public int rowOrColum;

        [SerializeField, Header("间距设置")] protected float rowSpacing;
        [SerializeField] protected float columSpacing;

        // 偏移信息
        [SerializeField, Header("偏移设置")] private float leftOffset;
        [SerializeField] public float topOffset;

        private RectTransform _cellItemRect;

        // cell 信息
        protected float CellItemWidth;
        protected float CellItemHeight;

        // 记录显示范围框信息
        protected float ViewWidth;
        protected float ViewHeight;

        // content大小信息
        protected float ContentWidth;
        protected float ContentHeight;

        // 所有 cell 信息
        protected List<CellInfo> CellInfos;

        // 首尾Index
        protected int MinIndex = -1;
        protected int MaxIndex = -1;

        protected ObjectPool<LoopItem> ItemPool;

        protected override void Awake()
        {
            base.Awake();

            ItemPool = new ObjectPool<LoopItem>(cellItemPre, content);

            _cellItemRect = cellItemPre.GetComponent<RectTransform>();
        }

        public virtual void Init()
        {
            vertical = moveType == MoveType.Vertical;
            horizontal = moveType == MoveType.Horizontal;

            // 记录cell基本信息
            var cellRectSize = _cellItemRect.rect.size;
            CellItemWidth = cellRectSize.x;
            CellItemHeight = cellRectSize.y;

            // 记录view 框信息
            var rectSize = viewport.rect.size;
            ViewWidth = rectSize.x;
            ViewHeight = rectSize.y;

            // 初始化content信息
            var rect2 = content.rect;
            ContentWidth = rect2.width;
            ContentHeight = rect2.height;

            SetAnchor(content);
            content.pivot = new Vector2(0, 1);

            onValueChanged.RemoveAllListeners();
            onValueChanged.AddListener(ScrollRectListener);
        }

        public virtual void SetData(List<CellInfo> cellInfos)
        {
            CellInfos = cellInfos;

            int count = CellInfos.Count;

            // 计算content尺寸
            if (moveType == MoveType.Vertical)
            {
                ContentWidth = content.rect.width;

                var calHeight = (columSpacing + CellItemHeight) * Mathf.CeilToInt((float)count / rowOrColum) +
                                topOffset;
                ContentHeight = calHeight < ViewHeight ? ViewHeight : calHeight;

                content.sizeDelta = new Vector2(ContentWidth, ContentHeight);
                content.anchoredPosition = new Vector2(0, 0);
            }
            else
            {
                float calWidth = (rowSpacing + CellItemWidth) * Mathf.CeilToInt((float)count / rowOrColum) +
                                 leftOffset;
                ContentWidth = calWidth < ViewWidth ? ViewWidth : calWidth;

                ContentHeight = content.rect.height;

                content.sizeDelta = new Vector2(ContentWidth, ContentHeight);
                content.anchoredPosition = new Vector2(0, content.rect.size.y);
            }

            // 1 计算每个cell坐标并存储 2 显示范围内 cell 
            for (int i = 0; i < CellInfos.Count; i++)
            {
                CellInfo cellInfo = CellInfos[i];

                float pos; // isVertical ? 记录 Y : X
                float rowPos; // 计算每排里的 cell 坐标

                // * -> 计算每个Cell坐标
                // ReSharper disable once PossibleLossOfFraction
                var tmpVal = Mathf.FloorToInt(i / rowOrColum);
                var tmpVal1 = i % rowOrColum;

                if (moveType == MoveType.Vertical)
                {
                    rowPos = CellItemWidth * tmpVal1 + rowSpacing * tmpVal1 + leftOffset;
                    pos = CellItemHeight * tmpVal + columSpacing * tmpVal + topOffset;
                    cellInfo.Pos = new Vector3(rowPos, -pos, 0);
                }
                else
                {
                    pos = CellItemWidth * tmpVal + rowSpacing * tmpVal + leftOffset;
                    rowPos = CellItemHeight * tmpVal1 + columSpacing * tmpVal1 + topOffset;
                    cellInfo.Pos = new Vector3(pos, -rowPos, 0);
                }

                // 计算是否超过范围
                float cellPos = moveType == MoveType.Vertical ? cellInfo.Pos.y : cellInfo.Pos.x;
                if (IsOutRange(cellPos))
                {
                    cellInfo.LoopItem = null;
                    continue;
                }

                // -> 记录显示范围内 首位 index 和 末位 index
                MinIndex = MinIndex == -1 ? i : MinIndex; // 首位index
                MaxIndex = i; // 末位index

                //-> 取或创建 Cell
                LoopItem cell = ItemPool.Spawn();
                cell.transform.GetComponent<RectTransform>().anchoredPosition = cellInfo.Pos;
                cell.gameObject.name = i.ToString();

                //-> 存数据
                cellInfo.LoopItem = cell;

                cell.SetUi(cellInfo);
            }
        }

        private void SetAnchor(RectTransform rectTransform)
        {
            if (moveType == MoveType.Vertical)
            {
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
            }
        }

        //判断是否超出显示范围
        protected bool IsOutRange(float pos)
        {
            Vector3 listP = content.anchoredPosition;
            if (moveType == MoveType.Vertical)
            {
                if (pos + listP.y > CellItemHeight || pos + listP.y < -ViewHeight)
                {
                    return true;
                }
            }
            else
            {
                if (pos + listP.x < -CellItemWidth || pos + listP.x > ViewHeight)
                {
                    return true;
                }
            }

            return false;
        }

        #region 事件相关

        //滑动事件
        protected virtual void ScrollRectListener(Vector2 value)
        {
           UpdateCheck();
        }

        void UpdateCheck()
        {
            if (CellInfos == null)
                return;

            //检查超出范围
            for (int i = 0, length = CellInfos.Count; i < length; i++)
            {
                CellInfo cellInfo = CellInfos[i];
                LoopItem itemObj = cellInfo.LoopItem;
                Vector3 pos = cellInfo.Pos;

                float rangePos = moveType == MoveType.Vertical ? pos.y : pos.x;

                //判断是否超出显示范围
                if (IsOutRange(rangePos))
                {
                    //把超出范围的cell 扔进 poolsObj里
                    if (itemObj != null)
                    {
                        ItemPool.Cycle(itemObj);
                        CellInfos[i].LoopItem = null;
                    }
                }
                else
                {
                    if (itemObj == null)
                    {
                        //优先从 poolsObj中 取出 （poolsObj为空则返回 实例化的cell）
                        LoopItem cell = ItemPool.Spawn();
                        cell.transform.localPosition = pos;
                        cell.gameObject.name = i.ToString();
                        CellInfos[i].LoopItem = cell;

                        cell.SetUi(CellInfos[i]);
                    }
                }
            }
        }

        #endregion

        public void CycleAllItem()
        {
            for (int i = 0; i < CellInfos.Count; i++)
            {
                if (CellInfos[i].LoopItem != null)
                {
                    ItemPool.Cycle(CellInfos[i].LoopItem);
                    CellInfos[i].LoopItem = null;
                }
            }
        }
    }


    // 数据基类
    public class CellInfo 
    {
        // 记录 物体的坐标 和 物体 scroll 内部使用数据 scroll rect 自己内部初始化
        public Vector3 Pos;
        public LoopItem LoopItem;

        // ui 测试 展示使用
        public string Name;
    }

    public enum MoveType
    {
        Horizontal,
        Vertical
    }
}
