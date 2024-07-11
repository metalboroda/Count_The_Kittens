using __Game.Resources.Scripts.EventBus;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class AnswerButton : MonoBehaviour
  {
    public event Action<string> AnswerButtonPressed;

    [SerializeField] private AnswerDataSo _answerData;
    [Header("")]
    [SerializeField] private bool _showAnswerButtonAtOnce;
    [SerializeField] private Button _answerButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _answerButtonText;
    [Header("Tutorial")]
    [SerializeField] private GameObject _tutorialFinger;

    private void Start() {
      SubscribeButtons();
      SetupAnswerButtonText();
      HandleButtons();
    }

    private void SubscribeButtons() {
      _answerButton.onClick.AddListener(() => {
        AnswerButtonPressed?.Invoke(_answerButtonText.text);

        _answerButton.interactable = false;

        EventBus<EventStructs.VariantAudioClickedEvent>.Raise(new EventStructs.VariantAudioClickedEvent { AudioClip = _answerData.AnswerClip });

        _tutorialFinger?.SetActive(false);
      });
    }

    private void SetupAnswerButtonText() {
      _answerButtonText.text = _answerData.AnswerValue;
    }

    private void HandleButtons() {
      if (_showAnswerButtonAtOnce == true) {
        _answerButton.gameObject.SetActive(true);
        _answerButton.GetComponent<DOTweenAnimation>().DOPlay();
      }
      else {
        _answerButton.gameObject.SetActive(false);
      }
    }
  }
}