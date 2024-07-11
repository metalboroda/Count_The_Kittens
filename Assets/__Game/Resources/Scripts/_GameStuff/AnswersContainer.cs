using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
using System.Collections;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class AnswersContainer : MonoBehaviour
  {
    [SerializeField] private float _valueClipDelay = 0.5f;
    [Space]
    [SerializeField] private AudioClip[] _valuesClips;

    private Answer[] _answers;
    private int _currentTextNumber = 1;

    private AudioSource _audioSource;

    private GameBootstrapper _gameBootstrapper;

    private void Awake() {
      _gameBootstrapper = GameBootstrapper.Instance;

      _audioSource = GetComponent<AudioSource>();

      _answers = GetComponentsInChildren<Answer>();
    }

    private void Start() {
      AnswersSubscription();
    }

    private void AnswersSubscription() {
      foreach (var answer in _answers) {
        answer.CatButtonPressed += SetAnswerNumberText;
        answer.AnswerButtonPressed += OnAnswerButton;
      }
    }

    private void SetAnswerNumberText(Answer answer) {
      answer.SetNumberText(_currentTextNumber.ToString());

      PlayTextNumberClip(_currentTextNumber -1);

      _currentTextNumber++;
    }

    private void OnAnswerButton(string buttonValue) {
      if (buttonValue == _answers.Length.ToString()) {
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

      _audioSource.PlayOneShot(_valuesClips[index]);
    }
  }
}