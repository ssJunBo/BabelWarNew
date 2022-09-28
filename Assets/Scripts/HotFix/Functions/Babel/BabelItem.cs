using Main.Game.Base;
using TMPro;
using UnityEngine;

namespace HotFix.Functions.Babel
{
  public class BabelItem : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI nameTxt;

    private UiBabelLogic logic;

    public void Init(UiBabelLogic logic)
    {
      this.logic = logic;
    }

    public void SetData(int name)
    {
      nameTxt.text = name.ToString();
    }

    public void OnClickItem()
    {
      // 打开战斗选择界面
      logic.modelPlay.UiLoadingLogic.Open(ConStr.Fighting, () => { logic.modelPlay.UiFightingLogic.Open(); });
    }
  }
}
