using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
using System.Collections;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class AnswersContainer : MonoBehaviour
  {
    [SerializeField] private string _correctValue;
    [Header("")]
    [SerializeField] private float _valueClipDelay = 0.5f;
    [Space]
    [SerializeField] private AudioClip[] _valuesClips;

    private Answer[] _answers;
    private AnswerButton[] _answersButton;
    private int _currentTextNumber = 1;

    private GameBootstrapper _gameBootstrapper;

    private void Awake() {
      _gameBootstrapper = GameBootstrapper.Instance;

      _answers = GetComponentsInChildren<Answer>();
      _answersButton = GetComponentsInChildren<AnswerButton>();
    }

    private void Start() {
      EventBus<EventStructs.VariantsAssignedEvent>.Raise(new EventStructs.VariantsAssignedEvent());

      AnswersSubscription();
    }

    private void AnswersSubscription() {
      foreach (var answer in _answers) {
        answer.CatButtonPressed += SetAnswerNumberText;
      }

      foreach (var button in _answersButton) {
        button.AnswerButtonPressed += OnAnswerButton;
      }
    }

    private void SetAnswerNumberText(Answer answer) {
      answer.SetNumberText(_currentTextNumber.ToString());

      PlayTextNumberClip(_currentTextNumber -1);

      _currentTextNumber++;
    }

    private void OnAnswerButton(string buttonValue) {
      if (buttonValue == _correctValue) {
        _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameWinState(_gameBootstrapper), 1.5f, this);
      }
      else {
        _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameLoseState(_gameBootstrapper), 1.5f, this);
      }
    }

    private void PlayTextNumberClip(int index) {
      StartCoroutine(DoPlayTextNumberClip(index));
    }

    private IEnumerator DoPlayTextNumberClip(int index) {
      yield return new WaitForSeconds(_valueClipDelay);

      EventBus<EventStructs.VariantAudioClickedEvent>.Raise(new EventStructs.VariantAudioClickedEvent { AudioClip = _valuesClips[index] });
    }
  }
}