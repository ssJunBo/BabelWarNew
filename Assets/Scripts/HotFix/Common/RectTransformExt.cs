using UnityEngine;

namespace HotFix.Common
{
    public static class RectTransformExt
    {
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 vector2 = (rectTransform.pivot - pivot) * size * rectTransform.localScale.x;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition -= vector2;
        }
        
        public static Vector2 SafeAnchorPos(RectTransform rectTransform, float scale, Vector2 targetPos,Vector2 canvasSize)
        {
            Rect rect = rectTransform.rect;
            float scaleWidth = rect.width * scale;
            float scaleHeight = rect.height * scale;

            Vector2 pivot = rectTransform.pivot;
            float left = pivot.x * scaleWidth - targetPos.x;
            float right = (1 - pivot.x) * scaleWidth + targetPos.x;
            float top = (1 - pivot.y) * scaleHeight + targetPos.y;
            float bottom = pivot.y * scaleHeight - targetPos.y;

            Vector2 halfCanvasSize = canvasSize * 0.5f;
            if (left < halfCanvasSize.x)
            {
                targetPos.x -= halfCanvasSize.x - left;
            }
            else if (right < halfCanvasSize.x)
            {
                targetPos.x += halfCanvasSize.x - right;
            }

            if (bottom < halfCanvasSize.y)
            {
                targetPos.y -= halfCanvasSize.y - bottom;
            }
            else if (top < halfCanvasSize.y)
            {
                targetPos.y += halfCanvasSize.y - top;
            }

            return targetPos;
        }
    }
    
    public static class  GameExpand
    {
        private static RectTransform _canvasRect;
        public static Vector2 CanvasSize
        {
            get
            {
                if (_canvasRect==null)
                {
                    _canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
                }
                return _canvasRect.sizeDelta;
            }
        }
    }
}