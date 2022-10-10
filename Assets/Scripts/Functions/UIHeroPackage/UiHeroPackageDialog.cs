using System.Collections.Generic;
using _GameBase.UIBase;
using Common;
using Helpers;
using Managers;
using Managers.Model;
using UIExtension.ScrollRectExt;
using UnityEngine;

namespace Functions.UIHeroPackage
{
    public class UiHeroPackageLogic : UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UIHeroPackage/UiHeroPackageDialog";
        protected override EUiID UiId => EUiID.HeroPackage;
        protected override EUiLayer UiLayer => EUiLayer.High_2D;

        private readonly CModelPlay _model;

        public UiHeroPackageLogic(CModelPlay model)
        {
            _model = model;
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
                    battleUnitExcelItem = ExcelManager.Instance.GetExcelItem<BattleUnitExcelData, BattleUnitExcelItem>(unitId)
                };

                infos.Add(heroItemInfo);
            }

            return infos;
        }
    }

    public class HeroItemInfo : CellInfo
    {
        public BattleUnitExcelItem battleUnitExcelItem;
    }
    
    public class UiHeroPackageDialog : UiDialogBase
    {
        [SerializeField] private UiCircularScrollView scrollView;

        private UiHeroPackageLogic _uiLogic;

        #region override

        public override void Init()
        {
            _uiLogic = (UiHeroPackageLogic)UiLogic;
        }

        public override void ShowFinished()
        {
            var data = _uiLogic.GenerateHeroCellInfo();

            scrollView.Init();
            scrollView.SetData(data);
        }

        public override void Release()
        {
            base.Release();

            scrollView.CycleAllItem();
        }

        #endregion
    }
}