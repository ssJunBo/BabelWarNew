using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIExtension
{
    public class RichTextMeshPro : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            TMP_Text tmp = GetComponent<TMP_Text>();
            int linkIndex =TMP_TextUtilities.FindIntersectingLink(tmp,eventData.position,null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = tmp.textInfo.linkInfo[linkIndex];
                Debug.Log(linkInfo.GetLinkID());
                Application.OpenURL(linkInfo.GetLinkID());
            }
        }
    }
}