using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Functions.Currency
{
    public class CurrencyItem : MonoBehaviour
    {
        [SerializeField] private Image iconImg;
        [SerializeField] private TextMeshProUGUI countTxt;

        public void SetData(CurrencyInfo info)
        {
            // iconImg.sprite = null;
            countTxt.text =info.Num.ToString();
        }
    }
}