using System.Collections.Generic;
using _GameBase;
using Common;
using Managers;
using UIExtension.ScrollRectExt;
using UnityEngine;

namespace UIFunctions
{
    public class UiBabelLogic: UiLogicBase
    {
        public override EUiID UiId  => EUiID.UIBabel;
        protected override EUiLayer UiLayer => EUiLayer.High_2D;

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
