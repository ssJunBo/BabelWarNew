using System.Collections.Generic;
using DG.Tweening;
using HotFix.FuncLogic;
using HotFix.Managers;
using Main.Game.Base;
using TMPro;
using UnityEngine;
using HotFix.Common;
using HotFix.Data.Account;
using HotFix.Pool;
using HotFix.UIBase;

namespace HotFix.Functions.Fighting
{
    public class UiFightingDialog : UiDialogBase
    {
        [SerializeField] private TextMeshProUGUI quickFightTxt;
        [SerializeField] private RectTransform cardMoveTrs;
        [SerializeField] private GameObject fightTipsObj;
        [SerializeField] private GameObject fightMaskObj;
        [SerializeField] private RectTransform fightRect;
        
        [Header("自己Card UI")]
        [SerializeField] private RectTransform ownCardContentTrs;
        [SerializeField] private OwnCardItem ownCardItemPre;
        [SerializeField] private RectTransform ownCardBornPos;
        [SerializeField] private RectTransform ownCardCenterPos;
     
        [Header("敌人Card UI")]
        [SerializeField] private RectTransform enemyCardContentTrs;
        [SerializeField] private GameObject enemyCardItemPre;
        [SerializeField] private RectTransform enemyCardItemRect;
        [SerializeField] private RectTransform enemyCardBornPos;

        // ---- finish ui -------
        [Header("完成界面UI")]
        [SerializeField] private GameObject finishObjPanel;
        [SerializeField] private TextMeshProUGUI finishDesc;

        private UiFightingLogic _uiLogic;

        #region Override

        public override void Init()
        {
            _uiLogic = (UiFightingLogic)UiLogic;

            #region 添加事件

            EventManager.Subscribe<int>(EventMessageType.FightResult, FightResultRefresh);
            EventManager.Subscribe<List<CardInfo>>(EventMessageType.IssueCard,RefreshCard);

            #endregion
        }

        public override void ShowFinished()
        {
            _ownCardPool = new ObjectPool<OwnCardItem>(ownCardItemPre, FightManager.Instance.objPoolTrs);
            _enemyCardPool = new ObjectPool(enemyCardItemPre, FightManager.Instance.objPoolTrs);
            InitUI();
        }

        public override void Release()
        {
            EventManager.UnSubscribe<int>(EventMessageType.FightResult, FightResultRefresh);
            EventManager.UnSubscribe<List<CardInfo>>(EventMessageType.IssueCard,RefreshCard);

            foreach (var item in _ownAllGenerateCards)
            {
                _ownCardPool.Cycle(item);
            }

            _ownAllGenerateCards.Clear();

            foreach (var item in _enemyAllGenerateCards)
            {
                _enemyCardPool.Cycle(item);
            }
            
            _enemyAllGenerateCards.Clear();

            base.Release();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                CardManager.Instance.ChangeTurn(Turn.Own);
            }
            
            if (Input.GetKeyDown(KeyCode.B))
            {
                CardManager.Instance.ChangeTurn(Turn.Enemy);
            }
        }

        #endregion

        private void InitUI()
        {
            fightMaskObj.SetActive(true);
            finishObjPanel.SetActive(false);
            quickFightTxt.text = "x1";
        }
        
        // --------------------- 自己卡牌逻辑 ----------------------------
        private readonly List<OwnCardItem> _ownAllGenerateCards = new();
        private readonly List<float> _xPos = new();
        private ObjectPool<OwnCardItem> _ownCardPool;

        private void GenerateCards(List<CardInfo> cardInfos)
        {
            List<CardExcelItem> cardExcelItems = _uiLogic.GetCardExcelItems(cardInfos);
            for (var index = 0; index < cardExcelItems.Count; index++)
            {
                var fightCardExcelItem = cardExcelItems[index];
                OwnCardItem ownCardItem = _ownCardPool.Spawn();
                ownCardItem.transform.SetParent(ownCardContentTrs);
                ownCardItem.transform.localScale = Vector3.one * 0.5f;
                ownCardItem.transform.position = ownCardBornPos.position;
                ownCardItem.transform.SetAsLastSibling();

                ownCardItem.DragEndAct = RefreshCardPos;
                ownCardItem.RemoveCardAct = RemoveCard;
                ownCardItem.InAreaAct = InArea;

                ownCardItem.Init(cardMoveTrs, fightRect, index);
                ownCardItem.SetData(fightCardExcelItem);
                _ownAllGenerateCards.Add(ownCardItem);
            }

            RefreshCardPos();
        }

        private void RefreshCardPos()
        {
            GeneratePos();
            
            var radiusVal = Vector2.Distance(ownCardBornPos.position, ownCardCenterPos.position);

            for (var index = 0; index < _ownAllGenerateCards.Count; index++)
            {
                var cardItem = _ownAllGenerateCards[index];

                cardItem.Index = index;

                Vector3 targetPos = new Vector3(_xPos[index], 0, 0);

                var to = targetPos - ownCardBornPos.localPosition;

                var angle = Vector3.Angle(Vector3.up, to);

                angle = index < _ownAllGenerateCards.Count / 2 ? -angle : angle;

                var position = ownCardBornPos.position;

                var x1 = position.x + radiusVal * Mathf.Sin(angle * Mathf.Deg2Rad);
                var y1 = position.y + radiusVal * Mathf.Cos(angle * Mathf.Deg2Rad);

                var transform1 = cardItem.transform;
                Vector3 resPos = new Vector3(x1, y1, transform1.position.z);

                // transform1.position = resPos;

                cardItem.transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);

                transform1.DOScale(Vector3.one, 0.5f);
                transform1.DOMove(resPos, 0.5f);
            }
        }

        private void GeneratePos()
        {
            var cardWidth = Mathf.Clamp((ownCardContentTrs.rect.width - 70 * 2) / _ownAllGenerateCards.Count, 50, 200);

            _xPos.Clear();
            if (_ownAllGenerateCards.Count % 2 != 0)
            {
                _xPos.Add(0);

                for (int i = 0; i < _ownAllGenerateCards.Count / 2; i++)
                {
                    _xPos.Add((i + 1) * cardWidth);
                    _xPos.Add(-(i + 1) * cardWidth);
                }
            }
            else
            {
                for (int i = 0; i < _ownAllGenerateCards.Count / 2; i++)
                {
                    _xPos.Add((i + 1) * cardWidth - cardWidth/2);
                    _xPos.Add(-(i + 1) * cardWidth + cardWidth/2);
                }
            }

            _xPos.Sort((x, y) => x.CompareTo(y));
        }
        
        private void RemoveCard(OwnCardItem cardItem)
        {
            _ownAllGenerateCards.Remove(cardItem);

            _ownCardPool.Cycle(cardItem);
            
            RefreshCardPos();
        }
        // --------------------- 自己卡牌逻辑 ----------------------------

        
        // ----------------------- 敌人卡牌逻辑 --------------------------
        private readonly List<GameObject> _enemyAllGenerateCards = new();
        private readonly List<float> _enemyXPos = new();
        private ObjectPool _enemyCardPool;

        private void GenerateEnemyCard(List<CardInfo> cardInfos)
        {
            foreach (var cardInfo in cardInfos)
            {
                var obj = _enemyCardPool.Spawn();
                obj.transform.SetParent(enemyCardContentTrs);
                obj.transform.localScale = Vector3.one;
                obj.transform.position = enemyCardBornPos.position;

                _enemyAllGenerateCards.Add(obj);
            }

            _enemyXPos.Clear();

            var rect = enemyCardContentTrs.rect;

            // 大于边界时的item间距
            var spacing = (rect.width - 10) / _enemyAllGenerateCards.Count;


            for (int i = 0; i < _enemyAllGenerateCards.Count; i++)
            {
                float xVal = default;
                if (enemyCardItemRect.rect.width * _enemyAllGenerateCards.Count < rect.width - 10)
                {
                    xVal = enemyCardItemRect.rect.width * i + enemyCardItemRect.rect.width / 2 + 5 * (i + 1);
                }
                else
                {
                    xVal = spacing * i + enemyCardItemRect.rect.width / 2 + 5 * (i + 1);
                }

                _enemyXPos.Add(xVal);
            }


            int newCardCount = cardInfos.Count;
            
            Sequence _cardSeq = DOTween.Sequence();

            for (int i = 0; i < _enemyAllGenerateCards.Count; i++)
            {
                var enemyItem = _enemyAllGenerateCards[i];

                var endPos = _enemyXPos[i];

                if (i > _enemyAllGenerateCards.Count - newCardCount)
                {
                    _cardSeq.AppendInterval(10 / 60f);
                    _cardSeq.Append( enemyItem.transform.DOLocalMoveX(endPos, 0.5f));
                }
                else
                {
                    enemyItem.transform.DOLocalMoveX(endPos, 0.5f);
                }
            }
        }


        // ----------------------- 敌人卡牌逻辑 --------------------------
        private void InArea(bool isInArea)
        {
            if (fightTipsObj.activeSelf==isInArea)
                return;

            fightTipsObj.SetActive(isInArea);
        }

        #region message

        private void FightResultRefresh(int result)
        {
            TimerEventManager.Instance.DelaySeconds(1f, () =>
            {
                finishDesc.text = result == 1 ? "战斗胜利" : "战斗失败";
                finishObjPanel.SetActive(true);
            });
        }

        // 新增卡
        private void RefreshCard(List<CardInfo> cardInfos)
        {
            switch (CardManager.Instance.Turn)
            {
                case Turn.Own:
                    GenerateCards(cardInfos);
                    break;
                case Turn.Enemy:
                    GenerateEnemyCard(cardInfos);
                    break;
            }
        }
        
        #endregion
        
        #region BtnEvent
        public void QuickFight()
        {
            FightManager.Instance.OpenQuickFight = !FightManager.Instance.OpenQuickFight;

            EventManager.DispatchEvent(EventMessageType.ChangeTimeScale, FightManager.Instance.OpenQuickFight ? 2 : 1);
            quickFightTxt.text = FightManager.Instance.OpenQuickFight ? "x2" : "x1";
        }

        public void QuitFightScene()
        {
            _uiLogic.modelPlay.UiLoadingLogic.Open(ConStr.Main, () =>
            {
                EventManager.DispatchEvent(EventMessageType.ChangeTimeScale, 1);
                GameManager.Instance.ModelPlay.UiMainLogic.Open();
            });
        }

        public void StartFight()
        {
            fightMaskObj.SetActive(false);
            FightManager.Instance.StartFighting();
            
            // 开始抽卡
            CardManager.Instance.ChangeTurn(Turn.Own);
        }
        #endregion
    }
}