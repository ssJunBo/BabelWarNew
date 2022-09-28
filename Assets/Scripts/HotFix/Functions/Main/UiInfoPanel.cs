﻿using HotFix.Helpers;
using HotFix.Managers;
using HotFix.Managers.Model;
using Main.Game.ResourceFrame;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace HotFix.Functions.Main
{
    public class UiInfoPanel : MonoBehaviour,IDragHandler
    {
        [SerializeField] private Image personHeadImg;
        [SerializeField] private TextMeshProUGUI personNameTxt;

        private CModelPlay _modelPlay;

        public void SetData(CModelPlay modelPlay)
        {
            _modelPlay = modelPlay;

            int iconExcelId = DataManager.Instance.PersonInfo.iconExcelId;
            personHeadImg.sprite = Utils.GetSprite(iconExcelId);
            personNameTxt.text = DataManager.Instance.PersonInfo.name;

            GeneratePerson(101);
        }

        private GameObject _personObj;

        private void GeneratePerson(int heroId)
        {
            var pathId = ExcelManager.Instance.GetExcelItem<BattleUnitExcelData, BattleUnitExcelItem>(heroId).PathId;
            string pathStr = ExcelManager.Instance.GetExcelData<PathExcelData>().GetDetailPath(pathId);

            GameObject go = ResManager.Instance.LoadResource(pathStr);

            _personObj = Instantiate(go, GameManager.Instance.personTrs);
            _personObj.transform.localPosition = Vector3.zero;
            _personObj.transform.localScale = Vector3.one;
            _personObj.transform.localRotation = Quaternion.identity;
        }

        public void OnClickHeadIcon()
        {
            _modelPlay.UiPersonDetailInfoLogic.Open();
        }

        public void OnClickStartBattle()
        {
            _modelPlay.UiBabelLogic.Open();
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