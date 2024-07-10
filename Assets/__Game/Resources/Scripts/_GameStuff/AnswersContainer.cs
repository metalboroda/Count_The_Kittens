using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
using System.Collections;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class AnswersContainer : MonoBehaviour
  {
    [SerializeField] private Answer[] _answers;
    [Header("Stupor settings")]
    [SerializeField] private float _stuporTimeoutSeconds = 30f;

    private int _currentNumber;
    private Coroutine _stuporTimeoutRoutine;

    private GameBootstrapper _gameBootstrapper;

    private void Awake() {
      _gameBootstrapper = GameBootstrapper.Instance;
    }

    private void OnEnable() {
      foreach (var answer in _answers) {
        answer.AnswerButton.onClick.AddListener(() => AssignNumber(answer));
        answer.NumberButtonClicked += CheckForAllAnswersCompleted;
      }
    }

    private void OnDisable() {
      foreach (var answer in _answers) {
        answer.AnswerButton.onClick.RemoveListener(() => AssignNumber(answer));
        answer.NumberButtonClicked -= CheckForAllAnswersCompleted;
      }
    }

    private void Start() {
      _currentNumber = 1;

      ResetAndStartStuporTimer();
    }

    private void AssignNumber(Answer answer) {
      answer.SetAnswerNumber(_currentNumber);

      _currentNumber++;
    }

    private void CheckForAllAnswersCompleted() {
      foreach (var answer in _answers) {
        if (answer.Completed == false) {
          return;
        }
      }

      ResetAndStartStuporTimer();

      if (_gameBootstrapper != null) {
        _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameWinState(_gameBootstrapper), 1.5f, this);

        StopCoroutine(_stuporTimeoutRoutine);
      }
    }

    private void ResetAndStartStuporTimer() {
      if (_stuporTimeoutRoutine != null)
        StopCoroutine(_stuporTimeoutRoutine);

      _stuporTimeoutRoutine = StartCoroutine(DoStuporTimerCoroutine());
    }

    private IEnumerator DoStuporTimerCoroutine() {
      yield return new WaitForSeconds(_stuporTimeoutSeconds);

      EventBus<EventStructs.StuporEvent>.Raise(new EventStructs.StuporEvent());

      ResetAndStartStuporTimer();
    }
  }
}