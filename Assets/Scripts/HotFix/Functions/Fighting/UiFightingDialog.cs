using System.Collections.Generic;
using HotFix.FuncLogic;
using HotFix.Managers;
using HotFix.SystemTools.EventSys;
using Main.Game.Base;
using TMPro;
using UnityEngine;
using HotFix.Common;
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

            #endregion
        }

        public override void ShowFinished()
        {
            InitUI();
        }

        public override void Release()
        {
            EventManager.UnSubscribe<int>(EventMessageType.FightResult, FightResultRefresh);
            
            for (int i = _cardItems.Count - 1; i >= 0; i--)
            {
                Destroy(_cardItems[i].gameObject);
            }

            _cardItems = null;
            
            base.Release();
        }
        
        #endregion

        private List<FightCardItem> _cardItems;
        private readonly List<float> _xPos = new();

        private void InitUI()
        {
            fightMaskObj.SetActive(true);
            finishObjPanel.SetActive(false);
            
            _cardItems = new List<FightCardItem>(_uiLogic.CardExcelItems.Count);

            for (var index = 0; index < _uiLogic.CardExcelItems.Count; index++)
            {
                var fightCardExcelItem = _uiLogic.CardExcelItems[index];
                FightCardItem fightCardItem = Instantiate(fightCardItemPre, contentTrs);
                fightCardItem.DragEndAct = RefreshCardPos;
                fightCardItem.RemoveCardAct = RemoveCard;
                fightCardItem.InAreaAct = InArea;

                fightCardItem.Init(cardMoveTrs,fightTrs);
                fightCardItem.SetData(fightCardExcelItem);
                _cardItems.Add(fightCardItem);
            }

            RefreshCardPos();

            quickFightTxt.text = "x1";
        }

        private void GeneratePos()
        {
            var cardWidth = Mathf.Clamp((contentTrs.rect.width - 70 * 2) / _cardItems.Count, 50, 200);

            _xPos.Clear();
            if (_cardItems.Count % 2 != 0)
            {
                _xPos.Add(0);

                for (int i = 0; i < _cardItems.Count / 2; i++)
                {
                    _xPos.Add((i + 1) * cardWidth);
                    _xPos.Add(-(i + 1) * cardWidth);
                }
            }
            else
            {
                for (int i = 0; i < _cardItems.Count / 2; i++)
                {
                    _xPos.Add((i + 1) * cardWidth - cardWidth/2);
                    _xPos.Add(-(i + 1) * cardWidth + cardWidth/2);
                }
            }

            _xPos.Sort();
        }

        private void RefreshCardPos()
        {
            GeneratePos();

            for (int i = 0; i < _cardItems.Count; i++)
            {
                var cardItem = _cardItems[i];
                var transform1 = cardItem.transform;
                
                cardItem.Index = i;
                
                transform1.localPosition = new Vector3(_xPos[i], 0, 0);

                var to = transform1.position - directPos.position;
                var angle = Vector3.Angle(Vector3.up, to);
                angle = cardItem.transform.localPosition.x > 0 ? -angle : angle;

                cardItem.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            var radiusVal = Vector2.Distance(directPos.position, cardCenterPos.position);

            foreach (var cardItem in _cardItems)
            {
                var to = cardItem.transform.position - directPos.position;
                var angle = Vector3.Angle(Vector3.up, to);
                angle = cardItem.transform.localPosition.x > 0 ? angle : -angle;

                var position = directPos.position;
                
                var x1 = position.x + radiusVal * Mathf.Sin(angle * Mathf.Deg2Rad);
                var y1 = position.y + radiusVal * Mathf.Cos(angle * Mathf.Deg2Rad);
                
                var transform1 = cardItem.transform;
                Vector3 resPos = new Vector3(x1, y1, transform1.position.z);
                transform1.position = resPos;
            }
        }

        private void RemoveCard(FightCardItem cardItem)
        {
            _cardItems.Remove(cardItem);

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
            finishDesc.text = result == 1 ? "战斗胜利" : "战斗失败";
            finishObjPanel.SetActive(true);
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
        }
        #endregion
    }
}