using System.Collections.Generic;
using _GameBase.UIBase;
using Common;
using Managers;
using Managers.Model;
using UIExtension.ScrollRectExt;
using UnityEngine;

namespace Functions.Babel
{
    public class UiBabelLogic: UiLogicBase
    {
        protected override string Path => "Prefabs/Functions/UIBabel/UiBabelDialog";
        protected override EUiID UiId  => EUiID.Babel;
        protected override EUiLayer UiLayer => EUiLayer.High_2D;

        public CModelPlay modelPlay;
        public UiBabelLogic(CModelPlay modelPlay)
        {
            this.modelPlay = modelPlay;
        }

        public List<CellInfo> GenerateData()
        {
            LevelExcelData allLevelData= ExcelManager.Instance.GetExcelData<LevelExcelData>();
            
            List<CellInfo> infoList = new List<CellInfo>();

            foreach (var levelItem in allLevelData.items)
            {
                BabelInfo info = new BabelInfo
                {
                    LevelExcelItem = levelItem
                };

                infoList.Add(info);
            }

            return infoList;
        }
    }

    public class BabelInfo : CellInfo
    {
        public LevelExcelItem LevelExcelItem;
    }

    public class UiBabelDialog : UiDialogBase
    {
        [SerializeField] private UiCircularScrollView uiCircularScrollView;

        private UiBabelLogic _uiLogic;

        public override void Init()
        {
            _uiLogic = (UiBabelLogic)UiLogic;
        }

        public override void ShowFinished()
        {
            var levelData=_uiLogic.GenerateData();
            
            uiCircularScrollView.Init();
            uiCircularScrollView.SetData(levelData);
        }
        
    }
}
