using System;
using DG.Tweening;
using HotFix.Managers;
using HotFix.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix.Functions.Fighting
{
    public class EnemyCardItem : PoolItemBase
    {
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI descTxt;
        [SerializeField] private Image iconImg;
        
        public GameObject mFront; //卡牌正面
        public GameObject mBack; //卡牌的背面
        public CardState mCardState = CardState.Front; //卡牌当前的状态，是正面还是背面
        public float mTime = 0.3f;
        public bool isActive; //true代表正在执行翻转，不许被打断

        public override void OnSpawned()
        {
            Init();
            gameObject.SetActive(true);
        }

        public override void OnCycle()
        {
            gameObject.SetActive(false);

            transform.localScale = Vector3.one;
        }

        public void SetData(CardExcelItem fightCardExcelItem)
        {
            nameTxt.text = fightCardExcelItem.Name;
            descTxt.text = fightCardExcelItem.Desc;
            iconImg.sprite = AtlasManager.Instance.GetSprite("FightCard", fightCardExcelItem.Icon.ToString());
        }
        
        /// <summary>
        /// 初始化卡牌角度，根据mCardState
        /// </summary>
        private void Init()
        {
            if (mCardState == CardState.Front)
            {
                mFront.transform.eulerAngles = Vector3.zero;
                mBack.transform.eulerAngles = new Vector3(0, 90, 0);
            }
            else
            {
                mBack.transform.eulerAngles = Vector3.zero;
                mFront.transform.eulerAngles = new Vector3(0, 90, 0);
            }
        }

        //开始向后转
        public void StartBack(Action cb)
        {
            if (isActive)
            {
                return;
            }

            ToBack(cb);
        }

        //开始前转
        public void StartFront()
        {
            if (isActive)
            {
                return;
            }

            ToFront();
        }

        /// <summary>
        /// 翻转到背面
        /// </summary>
        /// <returns></returns>
        private void ToBack(Action cb)
        {
            isActive = true;

            Sequence cardSeq = DOTween.Sequence();
            cardSeq.Append(mFront.transform.DORotate(new Vector3(0, -90, 0), mTime));
            cardSeq.AppendInterval(mTime);
            cardSeq.Append(mBack.transform.DORotate(new Vector3(0, 0, 0), mTime));
            cardSeq.AppendInterval(1.5f);
            cardSeq.AppendCallback(() =>
            {
                isActive = false;
                cb?.Invoke();
            });
        }

        /// <summary>
        /// 翻转到正面
        /// </summary>
        /// <returns></returns>
        private void ToFront()
        {
            isActive = true;

            Sequence cardSeq = DOTween.Sequence();
            cardSeq.Append(mBack.transform.DORotate(new Vector3(0, -90, 0), mTime));
            cardSeq.AppendInterval(mTime);
            cardSeq.Append(mFront.transform.DORotate(new Vector3(0, 0, 0), mTime));
            cardSeq.AppendCallback(() => { isActive = false; });
        }
    }

    public enum CardState
    {
        Front,
        Back
    }
}