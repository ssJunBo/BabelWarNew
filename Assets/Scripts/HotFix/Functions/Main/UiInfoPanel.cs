using HotFix.Managers;
using HotFix.Managers.Model;
using Main.Game.ResourceFrame;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace HotFix.Functions.Main
{
    public class UiInfoPanel : MonoBehaviour,IDragHandler
    {
        [SerializeField] private Image personHeadImg;
        [SerializeField] private TextMeshProUGUI personNameTxt;
        [SerializeField] private Toggle musicTog;

        private CModelPlay modelPlay;

        public void SetData(CModelPlay modelPlay)
        {
            this.modelPlay = modelPlay;

            musicTog.onValueChanged.AddListener((b) =>
            {
                AudioManager.Instance.PlayUI("click_common");
                AudioManager.Instance.SetBgMusicState(b);
            });
            
            // 默认静音
            AudioManager.Instance.SetBgMusicState(false);
            
            GeneratePerson(101);
        }

        private GameObject personObj;

        private void GeneratePerson(int heroId)
        {
            var pathId = ExcelManager.Instance.GetExcelItem<BattleUnitExcelData, BattleUnitExcelItem>(heroId).PathId;
            string pathStr = ExcelManager.Instance.GetExcelData<PathExcelData>().GetDetailPath(pathId);

            GameObject go = ResManager.Instance.LoadResource(pathStr);

            personObj = Instantiate(go, GameManager.Instance.personTrs);
            personObj.transform.localPosition = Vector3.zero;
            personObj.transform.localScale = Vector3.one;
            personObj.transform.localRotation = Quaternion.identity;
        }

        public void OnClickHeadIcon()
        {
            modelPlay.UiPersonDetailInfoLogic.Open();
        }

        public void OnClickStartBattle()
        {
            modelPlay.UiBabelLogic.Open();
        }

        public void Clear()
        {
            Destroy(personObj);
        }

        public void OnDrag(PointerEventData eventData)
        {
            personObj.transform.localEulerAngles += new Vector3(0, -eventData.delta.x, 0);
        }
    }
}