using Data.Account;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Functions.PersonDetailInfo
{
    public class PersonInfoPanel : MonoBehaviour
    {
        [SerializeField] private Image headIconImg;
        [SerializeField] private TextMeshProUGUI nameTxt;

        public void SetData(PersonInfo personInfo)
        {
            headIconImg.sprite = Utils.GetSprite(personInfo.iconExcelId);
            nameTxt.text = personInfo.name;
        }
    }
}
