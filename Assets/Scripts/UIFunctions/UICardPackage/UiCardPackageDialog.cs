using System.Collections.Generic;
using _GameBase;
using Common;
using Helpers;
using Managers;
using UIExtension.ScrollRectExt;
using UnityEngine;
using UnityEngine.UI;

namespace UIFunctions
{
    public class UiCardPackageLogic : UiLogicBase
    {
        public override EUiID UiId => EUiID.UICardPackage;

        protected override EUiLayer UiLayer => EUiLayer.High_2D;


        public List<CellInfo> GenerateCardCellInfo()
        {
            List<CellInfo> list = new List<CellInfo>();

            foreach (var cardInfo in DataManager.Instance.OwnCardsList)
            {
                CellInfo cardItemInfo = new CardItemInfo
                {
                    FightCardExcelItem = CardManager.Instance.GetCardExcelItem(cardInfo)
                };

                list.Add(cardItemInfo);
            }

            return list;
        }

        public List<CellInfo> GenerateHeroCellInfo()
        {
            List<CellInfo> infos = new List<CellInfo>();

            var heroInfo = DataManager.Instance.PersonInfo.HeroInfos;

            foreach (var combineId in heroInfo)
            {
                int unitId = IDParseHelp.GetBattleUnitId(combineId);

                HeroItemInfo heroItemInfo = new HeroItemInfo
                {
                    BattleUnitExcelItem =
                        ExcelManager.Instance.GetExcelItem<BattleUnitExcelData, BattleUnitExcelItem>(unitId)
                };

                infos.Add(heroItemInfo);
            }

            return infos;
        }
    }

    public class HeroItemInfo : CellInfo
    {
        public BattleUnitExcelItem BattleUnitExcelItem;
    }

    public class CardItemInfo : CellInfo
    {
        public int cardLev = 1;
        public CardExcelItem FightCardExcelItem;
    }

    public class UiCardPackageDialog : UiDialogBase
    {
        [SerializeField] private UiCircularScrollView skillCardSv;
        [SerializeField] private UiCircularScrollView heroCardSv;
        [SerializeField] private Toggle skillCardTog, heroCardTog;

        private readonly Vector3 _farV3 = new(-9999, -9999, -9999);
        private readonly Vector3 _oriPosV3 = new(0, -125, 0);

        private RectTransform _skillCardSvRect, _heroCardSvRect;
        
        private UiCardPackageLogic _uiLogic;

        #region override

        public override void Init()
        {
            _uiLogic = (UiCardPackageLogic)UiLogic;

            _skillCardSvRect = skillCardSv.GetComponent<RectTransform>();
            _heroCardSvRect = heroCardSv.GetComponent<RectTransform>();

            InitEvent();
        }

        public override void ShowFinished()
        {
            var data0 = _uiLogic.GenerateCardCellInfo();
            skillCardSv.Init();
            skillCardSv.SetData(data0);

            var data = _uiLogic.GenerateHeroCellInfo();
            heroCardSv.Init();
            heroCardSv.SetData(data);

            SetShowSv(true);
        }

        public override void Release()
        {
            base.Release();

            heroCardSv.CycleAllItem();
            skillCardSv.CycleAllItem();
        }

        #endregion

        private void InitEvent()
        {
            skillCardTog.onValueChanged.AddListener(SetShowSv);

            heroCardTog.onValueChanged.AddListener(b => { SetShowSv(!b); });
        }

        private void SetShowSv(bool isSkillCard)
        {
            _skillCardSvRect.localPosition = isSkillCard ? _oriPosV3 : _farV3;
            _heroCardSvRect.localPosition = isSkillCard ? _farV3 : _oriPosV3;
        }
    }
}
