using _GameBase;
using Common;
using Helpers;
using Managers;
using Managers.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace Functions.Main
{
    public class UiInfoPanel : MonoBehaviour,IDragHandler
    {
        [SerializeField] private Image personHeadImg;
        [SerializeField] private TextMeshProUGUI personNameTxt;

        private UiMainLogic uiMainLogic;

        public void SetData(UiMainLogic uiMainLogic)
        {
            this.uiMainLogic = uiMainLogic;

            int iconExcelId = DataManager.Instance.PersonInfo.IconExcelId;
            personHeadImg.sprite = Utils.GetSprite(iconExcelId);
            personNameTxt.text = DataManager.Instance.PersonInfo.Name;

            GeneratePerson(101);
        }

        private GameObject _personObj;

        private void GeneratePerson(int heroId)
        {
            var pathId = ExcelManager.Instance.GetExcelItem<BattleUnitExcelData, BattleUnitExcelItem>(heroId).PathId;
            string pathStr = ExcelManager.Instance.GetExcelData<PathExcelData>().GetDetailPath(pathId);


            return;
            
            // GameObject go = (GameObject)ResourcesComponent.Instance.GetAssetWithPath(uiMainLogic.UiId.ToString().ToLower() + "dialog.unity3d", UiId + "Dialog");
            // GameObject go = resourloa<GameObject>(pathStr);

            // _personObj = Instantiate(go, GameManager.Instance.personTrs);
            // _personObj.transform.localPosition = Vector3.zero;
            // _personObj.transform.localScale = Vector3.one;
            // _personObj.transform.localRotation = Quaternion.identity;
        }

        public void OnClickHeadIcon()
        {
            UiManager.Instance.OpenUi(EUiID.UiPersonDetailInfo);
        }

        public void OnClickStartBattle()
        {
            UiManager.Instance.OpenUi(EUiID.UiBabel);
        }

        public void Clear()
        {
            Destroy(_personObj);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _personObj.transform.localEulerAngles += new Vector3(0, -eventData.delta.x, 0);
        }
    }
}