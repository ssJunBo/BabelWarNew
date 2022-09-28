using System.Collections.Generic;
using DG.Tweening;
using HotFix.Managers;
using Main.Game.Base;
using TMPro;
using UnityEngine;
using HotFix.Common;
using HotFix.Data.Account;
using HotFix.Managers.Model;
using HotFix.Pool;
using HotFix.UIBase;

namespace HotFix.Functions.Fighting
{
    public class UiFightingLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UIFight/UiFightingDialog";
        public override EUiID UiId => EUiID.Fighting;

        protected override EUiLayer UiLayer => EUiLayer.High_2D;

        private readonly List<CardExcelItem> _cardExcelItems = new();

        public readonly CModelPlay modelPlay;

        public UiFightingLogic(CModelPlay modelPlay)
        {
            this.modelPlay = modelPlay;
        }

        public List<CardExcelItem> GetCardExcelItems(List<CardInfo> cardInfos)
        {
            _cardExcelItems.Clear();

            foreach (var cardInfo in cardInfos)
            {
                _cardExcelItems.Add(CardManager.Instance.GetCardExcelItem(cardInfo));
            }

            return _cardExcelItems;
        }
    }
    
    public class UiFightingDialog : UiDialogBase
    {
        #region 挂点

        [SerializeField] private TextMeshProUGUI quickFightTxt;
        [SerializeField] private RectTransform cardMoveTrs;
        [SerializeField] private GameObject fightTipsObj;
        [SerializeField] private GameObject fightMaskObj;
        [SerializeField] private RectTransform fightRect;
        
        [Header("自己Card UI")]
        [SerializeField] private GameObject ownCardParentObj;
        [SerializeField] private RectTransform ownCardContentTrs;
        [SerializeField] private OwnCardItem ownCardItemPre;
        [SerializeField] private RectTransform ownCardBornPos;
        [SerializeField] private RectTransform ownCardCenterPos;
        [SerializeField] private GameObject roundInfoObj;
        [SerializeField] private TextMeshProUGUI roundTimeDownTxt;

        [Header("敌人Card UI")]
        [SerializeField] private GameObject enemyCardParentObj;
        [SerializeField] private RectTransform enemyCardContentTrs;
        [SerializeField] private EnemyCardItem enemyCardItemPre;
        [SerializeField] private RectTransform enemyCardItemRect;
        [SerializeField] private RectTransform enemyCardBornPos;
        [SerializeField] private Transform enemyCardPos;// 移动到场景上的位置

        // ---- finish ui -------
        [Header("完成界面UI")]
        [SerializeField] private GameObject finishObjPanel;
        [SerializeField] private TextMeshProUGUI finishDesc;

        #endregion

        #region logic

        private UiFightingLogic _uiLogic;

        #endregion

        #region Override

        public override void Init()
        {
            _uiLogic = (UiFightingLogic)UiLogic;

            #region 订阅事件

            EventManager.Subscribe<int>(EventMessageType.FightResult, FightResultRefresh);
            EventManager.Subscribe<List<CardInfo>>(EventMessageType.IssueCard,RefreshCard);

            #endregion
        }

        public override void ShowFinished()
        {
            _ownCardPool = new ObjectPool<OwnCardItem>(ownCardItemPre, FightManager.Instance.objPoolTrs);
            _enemyCardPool = new ObjectPool<EnemyCardItem>(enemyCardItemPre, FightManager.Instance.objPoolTrs);
            
            SetUI();
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

            _timer = 0;

            base.Release();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A)) CardManager.Instance.ChangeRound(Round.Own);

            if (Input.GetKeyDown(KeyCode.B)) CardManager.Instance.ChangeRound(Round.Enemy);

            if (CardManager.Instance.Round == Round.Own && !_fightOver) CardTimeDown();
        }

        #endregion

        #region 初始化UI

        private Color _normalColor,_redColor;
        private void SetUI()
        {
            ownCardParentObj.SetActive(false);
            roundInfoObj.SetActive(false);
            enemyCardParentObj.SetActive(false);
            
            fightMaskObj.SetActive(true);
            finishObjPanel.SetActive(false);
            quickFightTxt.text = "x1";
            
            ColorUtility.TryParseHtmlString("#FF5B5B", out _redColor);
            ColorUtility.TryParseHtmlString("#FFFFFF", out _normalColor);
        }


        #endregion
       
        #region --------------------- 自己卡牌逻辑 ----------------------------

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

        private void InArea(bool isInArea)
        {
            if (fightTipsObj.activeSelf==isInArea)
                return;

            fightTipsObj.SetActive(isInArea);
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

        private const float TimeLimit = 30;
        private float _timer;
        private void CardTimeDown()
        {
            _timer += Time.deltaTime;
            if (_timer >= TimeLimit)
            {
                CardManager.Instance.ChangeRound(Round.Enemy);
                _timer = 0;
            }

            int remainTime = (int)(TimeLimit - _timer);
            roundTimeDownTxt.text = $"剩余时间：{remainTime}";

            roundTimeDownTxt.color = remainTime <= 10 ? _redColor : _normalColor;
        }

        #endregion

        #region  ---------------------------- 敌人卡牌逻辑 ----------------------------
      
        private readonly List<EnemyCardItem> _enemyAllGenerateCards = new();
        private readonly List<float> _enemyXPos = new();
        private ObjectPool<EnemyCardItem> _enemyCardPool;

        private void UpdateEnemyCard(List<CardInfo> cardInfos)
        {
            if (_moving) return;
            
            GeneratedCard(cardInfos);

            GeneratedPos();

            SetCardAnim(cardInfos.Count);
        }

        private void GeneratedCard(List<CardInfo> cardInfos)
        {
            foreach (var cardInfo in cardInfos)
            {
                var obj = _enemyCardPool.Spawn();
                obj.transform.SetParent(enemyCardContentTrs);
                obj.transform.localScale = Vector3.one;
                obj.transform.position = enemyCardBornPos.position;

                _enemyAllGenerateCards.Add(obj);
            }
        }

        private void GeneratedPos()
        {
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
        }

        private Sequence _enemyCardSeq;
        private bool _moving;
        private void SetCardAnim(int newCardCount)
        {
            if (_enemyCardSeq != null)
            {
                _enemyCardSeq.Kill();
            }

            _enemyCardSeq = DOTween.Sequence();

            
            for (int i = 0; i < _enemyAllGenerateCards.Count; i++)
            {
                var enemyItem = _enemyAllGenerateCards[i];

                var endPos = _enemyXPos[i];

                if (i > _enemyAllGenerateCards.Count - newCardCount)
                {
                    _enemyCardSeq.AppendInterval(10 / 60f);
                    _enemyCardSeq.Append( enemyItem.transform.DOLocalMoveX(endPos, 0.5f));
                }
                else
                {
                    enemyItem.transform.DOLocalMoveX(endPos, 0.5f);
                }
            }
            
            // 延迟 1s 开始发卡
            _enemyCardSeq.AppendInterval(1f);
            
            for (int i = _enemyAllGenerateCards.Count-1; i >=0; i--)
            {
                var enemyCardItem = _enemyAllGenerateCards[i];
                _enemyCardSeq.AppendCallback(() =>
                {
                    _enemyAllGenerateCards.Remove(enemyCardItem);
                    _moving = true;
                });
                _enemyCardSeq.Append(enemyCardItem.transform.DOMove(enemyCardPos.position, 0.5f).OnComplete(() =>
                {
                    _moving = false;
                }));
                _enemyCardSeq.Join(enemyCardItem.transform.DOScale(Vector3.one * 3, 0.5f));
                _enemyCardSeq.AppendCallback(() =>
                {
                    var cardInfo = CardManager.Instance.GetOneCardPlay();
                    enemyCardItem.SetData(CardManager.Instance.GetCardExcelItem(cardInfo));
                    enemyCardItem.StartBack(() =>
                    {
                        _enemyCardPool.Cycle(enemyCardItem);
                    });
                });
                _enemyCardSeq.AppendInterval(2.5f);
            }

            _enemyCardSeq.AppendCallback(() =>
            {
                CardManager.Instance.ChangeRound(Round.Own);
            });
        }

        #endregion
        
        #region ---------------------------- message ----------------------------

        private bool _fightOver;
        private void FightResultRefresh(int result)
        {
            _fightOver = true;
            TimerEventManager.Instance.DelaySeconds(1f, () =>
            {
                finishDesc.text = result == 1 ? "战斗胜利" : "战斗失败";
                finishObjPanel.SetActive(true);
            });
            
            _enemyCardSeq.Kill();
        }

        // 新增卡
        private void RefreshCard(List<CardInfo> cardInfos)
        {
            var curRound = CardManager.Instance.Round;

            roundInfoObj.SetActive(curRound == Round.Enemy);
            ownCardParentObj.SetActive(curRound == Round.Own);
            enemyCardParentObj.SetActive(curRound == Round.Enemy);
            
            switch (curRound)
            {
                case Round.Own:
                    GenerateCards(cardInfos);
                    break;
                case Round.Enemy:
                    UpdateEnemyCard(cardInfos);
                    break;
            }
        }
        
        #endregion
        
        #region ---------------------------- BtnEvent ----------------------------
        public void OnClickQuickFight()
        {
            FightManager.Instance.OpenQuickFight = !FightManager.Instance.OpenQuickFight;

            EventManager.DispatchEvent(EventMessageType.ChangeTimeScale, FightManager.Instance.OpenQuickFight ? 2 : 1);
            quickFightTxt.text = FightManager.Instance.OpenQuickFight ? "x2" : "x1";
        }

        public void OnClickQuitFightScene()
        {
            FightManager.Instance.PauseFighting();

            _uiLogic.modelPlay.UiLoadingLogic.Open(ConStr.Main, () =>
            {
                EventManager.DispatchEvent(EventMessageType.ChangeTimeScale, 1);
                GameManager.Instance.ModelPlay.UiMainLogic.Open();
            });
        }

        public void OnClickStartFight()
        {
            FightManager.Instance.StartFighting();
            fightMaskObj.SetActive(false);
            ownCardParentObj.SetActive(true);
            // 开始抽卡
            CardManager.Instance.ChangeRound(Round.Own);
        }

        public void OnClickRoundOver()
        {
            CardManager.Instance.ChangeRound(Round.Enemy);
        }
        
        public void OnClickCardPackage()
        {
            GameManager.Instance.ModelPlay.UICardPackageLogic.Open();
        }
        
        #endregion
    }
}