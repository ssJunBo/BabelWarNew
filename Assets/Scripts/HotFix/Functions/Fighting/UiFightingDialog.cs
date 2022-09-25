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
        [SerializeField] private RectTransform contentTrs;
        [SerializeField] private FightCardItem fightCardItemPre;
        [SerializeField] private RectTransform directPos;
        [SerializeField] private RectTransform cardCenterPos;
        [SerializeField] private RectTransform fightTrs;
        [SerializeField] private GameObject fightTipsObj;
        [SerializeField] private GameObject fightMaskObj;
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
            _cardPool = new ObjectPool<FightCardItem>(fightCardItemPre, FightManager.Instance.objPoolTrs);
            
            InitUI();
        }

        public override void Release()
        {
            EventManager.UnSubscribe<int>(EventMessageType.FightResult, FightResultRefresh);
            EventManager.UnSubscribe<List<CardInfo>>(EventMessageType.IssueCard,RefreshCard);

            foreach (var item in allGenerateCards)
            {
                _cardPool.Cycle(item);
            }

            allGenerateCards = null;
            
            base.Release();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                CardManager.Instance.ChangeTurn(Turn.Own);
            }
        }

        #endregion

        private List<FightCardItem> allGenerateCards = new();
        private readonly List<float> _xPos = new();

        private ObjectPool<FightCardItem> _cardPool;

        private void InitUI()
        {
            fightMaskObj.SetActive(true);
            finishObjPanel.SetActive(false);
            quickFightTxt.text = "x1";
        }

        private void GenerateCards(List<CardInfo> cardInfos)
        {
            List<CardExcelItem> cardExcelItems = _uiLogic.GetCardExcelItems(cardInfos);
            foreach (var fightCardExcelItem in cardExcelItems)
            {
                FightCardItem fightCardItem = _cardPool.Spawn();
                fightCardItem.transform.SetParent(contentTrs);
                fightCardItem.transform.localScale = Vector3.one * 0.5f;
                fightCardItem.transform.position = directPos.position;
                
                fightCardItem.DragEndAct = RefreshCardPos;
                fightCardItem.RemoveCardAct = RemoveCard;
                fightCardItem.InAreaAct = InArea;

                fightCardItem.Init(cardMoveTrs, fightTrs);
                fightCardItem.SetData(fightCardExcelItem);
                allGenerateCards.Add(fightCardItem);
            }

            RefreshCardPos();
        }

        private void RefreshCardPos()
        {
            GeneratePos();
            //
            // for (int i = 0; i < allGenerateCards.Count; i++)
            // {
            //     var cardItem = allGenerateCards[i];
            //     var transform1 = cardItem.transform;
            //     
            //     cardItem.Index = i;
            //     
            //     transform1.localPosition = new Vector3(_xPos[i], 0, 0);
            //     
            //     var to = transform1.position - directPos.position;
            //     var angle = Vector3.Angle(Vector3.up, to);
            //     angle = cardItem.transform.localPosition.x > 0 ? -angle : angle;
            //
            //     cardItem.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //
            //     // cardItem.transform.DORotateQuaternion(Quaternion.AngleAxis(angle, Vector3.forward), 0.5f);
            // }
            
            var radiusVal = Vector2.Distance(directPos.position, cardCenterPos.position);

            for (var index = 0; index < allGenerateCards.Count; index++)
            {
                var cardItem = allGenerateCards[index];
                var to = cardItem.transform.position - directPos.localPosition;
                var angle = Vector3.Angle(Vector3.up, to);
                
                angle = cardItem.transform.localPosition.x > 0 ? angle : -angle;

                var position = directPos.position;

                var x1 = position.x + radiusVal * Mathf.Sin(angle * Mathf.Deg2Rad);
                var y1 = position.y + radiusVal * Mathf.Cos(angle * Mathf.Deg2Rad);

                var transform1 = cardItem.transform;
                Vector3 resPos = new Vector3(x1, y1, transform1.position.z);

                // transform1.position = resPos;

                cardItem.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                transform1.DOScale(Vector3.one, 0.5f);
                transform1.DOMove(resPos, 0.5f);
            }
        }

        private void GeneratePos()
        {
            var cardWidth = Mathf.Clamp((contentTrs.rect.width - 70 * 2) / allGenerateCards.Count, 50, 200);

            _xPos.Clear();
            if (allGenerateCards.Count % 2 != 0)
            {
                _xPos.Add(0);

                for (int i = 0; i < allGenerateCards.Count / 2; i++)
                {
                    _xPos.Add((i + 1) * cardWidth);
                    _xPos.Add(-(i + 1) * cardWidth);
                }
            }
            else
            {
                for (int i = 0; i < allGenerateCards.Count / 2; i++)
                {
                    _xPos.Add((i + 1) * cardWidth - cardWidth/2);
                    _xPos.Add(-(i + 1) * cardWidth + cardWidth/2);
                }
            }

            _xPos.Sort();
        }
        
        private void RemoveCard(FightCardItem cardItem)
        {
            allGenerateCards.Remove(cardItem);

            RefreshCardPos();
        }

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