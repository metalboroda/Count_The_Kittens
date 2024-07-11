using __Game.Resources.Scripts.EventBus;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class Answer : MonoBehaviour
  {
    public event Action<Answer> CatButtonPressed;
    public event Action<string> AnswerButtonPressed;

    [Header("References")]
    [SerializeField] private AnswerDataSo _answerData;
    [Header("Image")]
    [SerializeField] private Image _catImage;
    [Header("Buttons")]
    [SerializeField] private Button _catButton;
    [Space]
    [SerializeField] private bool _showAnswerButtonAtOnce;
    [SerializeField] private Button _answerButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _numberText;
    [SerializeField] private TextMeshProUGUI _answerButtonText;
    [Header("Tutorial")]
    [SerializeField] private bool _tutorial;
    [SerializeField] private GameObject _catFinger;
    [SerializeField] private GameObject _answerFinger;
    [Header("Audio")]
    [SerializeField] private AudioClip _meowClip;

    private void Start() {
      SetupAnswerButtonText();
      SetupCatImage();
      SubscribeButtons();
      HandleTexts();
      HandleButtons();
      TutorialSwitch(1);
    }

    public void SetNumberText(string text) {
      _numberText.text = text;

      _numberText.gameObject.SetActive(true);
      _numberText.GetComponent<DOTweenAnimation>().DOPlay();
    }

    private void SetupAnswerButtonText() {
      _answerButtonText.text = _answerData.AnswerValue;
    }

    private void SetupCatImage() {
      _catImage.sprite = _answerData.CatSprite;
    }

    private void SubscribeButtons() {
      _catButton.onClick.AddListener(() => {
        CatButtonPressed?.Invoke(this);

        _catButton.interactable = false;

        EventBus<EventStructs.VariantAudioClickedEvent>.Raise(new EventStructs.VariantAudioClickedEvent { AudioClip = _meowClip });

        _answerButton.gameObject.SetActive(true);
        _answerButton.GetComponent<DOTweenAnimation>().DOPlay();

        TutorialSwitch(2);
      });

      _answerButton.onClick.AddListener(() => {
        AnswerButtonPressed?.Invoke(_answerButtonText.text);

        _answerButton.interactable = false;

        EventBus<EventStructs.VariantAudioClickedEvent>.Raise(new EventStructs.VariantAudioClickedEvent { AudioClip = _answerData.AnswerClip });

        TutorialSwitch(3);
      });
    }

    private void HandleTexts() {
      _numberText.gameObject.SetActive(false);
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

    private void TutorialSwitch(int stage) {
      _catFinger.SetActive(false);
      _answerFinger.SetActive(false);

      if (_tutorial == false) return;

      switch (stage) {
        case 1:
          _catFinger.SetActive(true);
          break;
        case 2:
          _catFinger.SetActive(false);
          _answerFinger.SetActive(true);
          break;
        case 3:
          _catFinger.SetActive(false);
          _answerFinger.SetActive(false);
          break;
      }
    }
  }
}