using HotFix.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Test03
{
	public class PinchScrollRect : ScrollRect
	{
		private readonly float maxScale = 3f;
		private readonly float minScale = 1f;
		private readonly float zoomSpeed = 0.0075f;
		
		private void Update()
		{
			if (Input.touchCount==1)
			{
				vertical = true;
				horizontal = true;
			}
			
			if (Input.touchCount > 1)
			{
			    vertical = false;
			    horizontal = false;
				
				Touch touch1 = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);

				if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
				{
					Vector2 pivot = CalcTouchPivot(touch1, touch2);

					content.SetPivot(pivot);
					
					// 放大缩小
					UpdateScale(touch1, touch2);
				}
			}
		}

		private void UpdateScale(Touch touch1, Touch touch2)
		{
			Vector2 touchPrevPos1 = touch1.position - touch1.deltaPosition;
			Vector2 touchPrevPos2 = touch2.position - touch2.deltaPosition;

			float prevTouchDeltaMag = (touchPrevPos1 - touchPrevPos2).magnitude;
			float touchDeltaMag = (touch1.position - touch2.position).magnitude;
			float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

			Vector3 localScale = content.localScale;
			float scaleVal = Mathf.Clamp(localScale.x + deltaMagnitudeDiff * zoomSpeed, minScale, maxScale);

			content.localScale = Vector3.one * scaleVal;
			content.anchoredPosition =
				RectTransformExt.SafeAnchorPos(content, scaleVal, content.anchoredPosition, GameExpand.CanvasSize);
		}

		private Vector2 CalcTouchPivot(Touch touch1, Touch touch2)
		{
			Vector2 tmpTouchCenterPos = (touch1.position + touch2.position) / 2;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(content, tmpTouchCenterPos, Camera.main, out Vector2 pos);

			Vector2 pivot = content.pivot;
			Rect contentRect = content.rect;

			float pivotX = (pos.x + contentRect.width * pivot.x) / contentRect.width;
			float pivotY = (pos.y + contentRect.height * pivot.y) / contentRect.height;

			return new Vector2(pivotX, pivotY);
		}
	}
}
