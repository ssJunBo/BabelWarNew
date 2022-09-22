using System;
using System.Collections;
using DG.Tweening;
using HotFix.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HotFix.UIExtension
{
  public class ExpandButton : Selectable, IPointerClickHandler, ISubmitHandler
  {
    [Space(10), SerializeField] private bool hasAnim;
    [SerializeField] private string clickAudioClipName;

    private Sequence _mSequence;

    private void BtnTweenAnim()
    {
      _mSequence = DOTween.Sequence();
      _mSequence.Append(transform.DOScale(0.8f, 3 / 30f));
      _mSequence.Append(transform.DOScale(1.1f, 3 / 30f));
      _mSequence.Append(transform.DOScale(1f, 3 / 30f));
      _mSequence.AppendCallback(() =>
      {
        m_OnClick?.Invoke();

        _mSequence.SetAutoKill(true);
        // _mSequence.Pause();
      });
    }

    [FormerlySerializedAs("onClick")] [SerializeField]
    private Button.ButtonClickedEvent m_OnClick = new Button.ButtonClickedEvent();

    protected ExpandButton()
    {
    }

    public Button.ButtonClickedEvent onClick
    {
      get => this.m_OnClick;
      set => this.m_OnClick = value;
    }

    private void Press()
    {
      if (!this.IsActive() || !this.IsInteractable())
        return;
      UISystemProfilerApi.AddMarker("Button.onClick", (UnityEngine.Object)this);
      if (!hasAnim) this.m_OnClick?.Invoke();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.button != 0)
        return;

      if (hasAnim)
      {
        BtnTweenAnim();
      }

      AudioManager.Instance.PlayUI(!string.IsNullOrEmpty(clickAudioClipName) ? clickAudioClipName : "click_common");

      this.Press();
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
      this.Press();
      if (!this.IsActive() || !this.IsInteractable())
        return;
      this.DoStateTransition(Selectable.SelectionState.Pressed, false);
      this.StartCoroutine(this.OnFinishSubmit());
    }

    private IEnumerator OnFinishSubmit()
    {
      float fadeTime = this.colors.fadeDuration;
      float elapsedTime = 0.0f;
      while ((double)elapsedTime < (double)fadeTime)
      {
        elapsedTime += Time.unscaledDeltaTime;
        yield return (object)null;
      }

      this.DoStateTransition(this.currentSelectionState, false);
    }

    [Serializable]
    public class ButtonClickedEvent : UnityEvent
    {
    }
  }
}