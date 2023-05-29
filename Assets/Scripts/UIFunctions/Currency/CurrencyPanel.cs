using System.Collections.Generic;
using UnityEngine;

namespace UIFunctions
{
    public class CurrencyPanel : MonoBehaviour
    {
       [SerializeField] private CurrencyItem currencyItemPrefab;

       public void SetData(List<CurrencyInfo> currencyInfos)
       {
           foreach (var currencyInfo in currencyInfos)
           {
               var currencyItem= Instantiate(currencyItemPrefab, transform);
               currencyItem.SetData(currencyInfo);
           }
       }
    }

    public class CurrencyInfo
    {
        public CurrencyType CurrencyType;
        public int Num;
    }

    public enum CurrencyType
    {
        Gold,
        Gem
    }
}
