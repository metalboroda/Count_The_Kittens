using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class Answer : MonoBehaviour
  {
    public event Action NumberButtonClicked;
    public event Action<int> NumberButtonClickedInt;

    [SerializeField] private Image _catImage;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectionColor;
    [Space]
    [SerializeField] private Color _numberButtonDefaultColor;
    [SerializeField] private Color _numberButtonSelectionColor;
    [Header("Tutorial")]
    [SerializeField] private bool _allowTutorial;
    [SerializeField] private GameObject _tutorialFingerCat;
    [SerializeField] private GameObject _tutorialFingerNumber;
    [field: Header("References")]
    [field: SerializeField] public Button AnswerButton;
    [SerializeField] private Button _numberButton;

    public bool Completed { get; private set; }

    private int _answerNumber;

    private GameBootstrapper _gameBootstrapper;
    private AnswersContainer _answersContainer;

    private void Awake() {
      _gameBootstrapper = GameBootstrapper.Instance;
      _answersContainer = GetComponentInParent<AnswersContainer>();

      GetAllCatsSpriteRenderers();
    }

    private void OnEnable() {
      AnswerButton.onClick.AddListener(() => {
        EnableNumberObject();

        AnswerButton.interactable = false;

        ChangeCatsColors();

        EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent());
      });

      _numberButton.onClick.AddListener(() => {
        Completed = true;

        _numberButton.interactable = false;

        if (_allowTutorial == true)
          _tutorialFingerNumber.SetActive(false);

        ChangeNumberButtonColors();

        NumberButtonClicked?.Invoke();
        NumberButtonClickedInt?.Invoke(_answerNumber);

        EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent());

        if (_answerNumber == _answersContainer.CurrentNumber - 1)
          _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameWinState(_gameBootstrapper), 1f, this);
        else
          _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameLoseState(_gameBootstrapper), 1f, this);
      });
    }

    private void Start() {
      _numberButton.transform.localScale = Vector3.zero;
      _numberButton.gameObject.SetActive(false);

      if (_allowTutorial == true) {
        _tutorialFingerCat.SetActive(true);
      }
      else {
        _tutorialFingerCat.SetActive(false);
      }

      _tutorialFingerNumber.SetActive(false);
    }

    private void EnableNumberObject() {
      _numberButton.gameObject.SetActive(true);

      Vector3 scale = new Vector3(1, 1, 1);

      _numberButton.transform.DOScale(scale, 0.5f);

      if (_allowTutorial == true) {
        _tutorialFingerCat.SetActive(false);
        _tutorialFingerNumber.SetActive(true);
      }
    }

    public void SetAnswerNumber(int number) {
      _answerNumber = number;

      _numberButton.GetComponentInChildren<TextMeshProUGUI>().text = _answerNumber.ToString();
    }

    private void GetAllCatsSpriteRenderers() {
      _catImage.color = _defaultColor;
    }

    private void ChangeCatsColors() {
      Sequence sequence = DOTween.Sequence();

      sequence.Append(_catImage.DOColor(_selectionColor, 0.2f))
              .Append(_catImage.DOColor(_defaultColor, 0.2f));
      sequence.Play();
    }

    private void ChangeNumberButtonColors() {
      Sequence sequence = DOTween.Sequence();

      sequence.Append(_numberButton.GetComponentInChildren<TextMeshProUGUI>().DOColor(_numberButtonSelectionColor, 0.2f))
              .Append(_numberButton.GetComponentInChildren<TextMeshProUGUI>().DOColor(_numberButtonDefaultColor, 0.2f));
      sequence.Play();
    }
  }
}