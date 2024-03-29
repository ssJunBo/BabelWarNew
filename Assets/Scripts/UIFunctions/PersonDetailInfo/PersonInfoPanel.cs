using Data.Account;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFunctions
{
    public class PersonInfoPanel : MonoBehaviour
    {
        [SerializeField] private Image headIconImg;
        [SerializeField] private TextMeshProUGUI nameTxt;

        public void SetData(PersonInfo personInfo)
        {
            headIconImg.sprite = Utils.GetSprite(personInfo.IconExcelId);
            nameTxt.text = personInfo.Name;
        }
    }
}
