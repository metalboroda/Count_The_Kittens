using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
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

    [SerializeField] private bool _instantLose;
    [Header("References")]
    [SerializeField] private AnswerDataSo _answerData;
    [Header("Image")]
    [SerializeField] private Image _catImage;
    [Header("Buttons")]
    [SerializeField] private Button _catButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _numberText;

    [Header("Tutorial")]
    [SerializeField] private bool _tutorial;
    [SerializeField] private GameObject _catFinger;
    [SerializeField] private GameObject _answerFinger;
    [Header("Audio")]
    [SerializeField] private AudioClip _meowClip;

    private GameBootstrapper _gameBootstrapper;

    private void Awake() {
      _gameBootstrapper = GameBootstrapper.Instance;
    }

    private void Start() {
      SetupCatImage();
      SubscribeButtons();
      HandleTexts();
      TutorialSwitch(1);
    }

    public void SetNumberText(string text) {
      _numberText.text = text;

      _numberText.gameObject.SetActive(true);
      _numberText.GetComponent<DOTweenAnimation>().DOPlay();
    }

    private void SetupCatImage() {
      _catImage.sprite = _answerData.CatSprite;
    }

    private void SubscribeButtons() {
      _catButton.onClick.AddListener(() => {
        if (_instantLose == true) {
          _gameBootstrapper.StateMachine.ChangeState(new GameLoseState(_gameBootstrapper));

          return;
        }

        CatButtonPressed?.Invoke(this);

        _catButton.interactable = false;

        EventBus<EventStructs.VariantAudioClickedEvent>.Raise(new EventStructs.VariantAudioClickedEvent { AudioClip = _meowClip });

        TutorialSwitch(2);
      });


    }

    private void HandleTexts() {
      _numberText.gameObject.SetActive(false);
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