using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Scripts.Infrastructure;
using System.Collections;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class AnswersContainer : MonoBehaviour
  {
    [SerializeField] private Answer[] _answers;
    [Header("Audio")]
    [SerializeField] private AudioClip[] _numbersClips;
    [Header("Stupor settings")]
    [SerializeField] private float _stuporTimeoutSeconds = 30f;

    public int CurrentNumber { get; private set; }

    private Coroutine _stuporTimeoutRoutine;

    private GameBootstrapper _gameBootstrapper;

    private void Awake() {
      _gameBootstrapper = GameBootstrapper.Instance;
    }

    private void OnEnable() {
      foreach (var answer in _answers) {
        answer.AnswerButton.onClick.AddListener(() => AssignNumber(answer));
        answer.NumberButtonClicked += CheckForAllAnswersCompleted;
        answer.NumberButtonClickedInt += (number) => PlayNumberClip(number);
      }
    }

    private void OnDisable() {
      foreach (var answer in _answers) {
        answer.AnswerButton.onClick.RemoveListener(() => AssignNumber(answer));
        answer.NumberButtonClicked -= CheckForAllAnswersCompleted;
        answer.NumberButtonClickedInt -= (number) => PlayNumberClip(number);
      }
    }

    private void Start() {
      CurrentNumber = 1;

      ResetAndStartStuporTimer();

      EventBus<EventStructs.VariantsAssignedEvent>.Raise(new EventStructs.VariantsAssignedEvent());
    }

    private void AssignNumber(Answer answer) {
      answer.SetAnswerNumber(CurrentNumber);

      CurrentNumber++;
    }

    private void CheckForAllAnswersCompleted() {
      foreach (var answer in _answers) {
        if (answer.Completed == false) {
          return;
        }
      }

      ResetAndStartStuporTimer();
      StopCoroutine(_stuporTimeoutRoutine);
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

    private void PlayNumberClip(int number) {
      if (number > 0 && number <= _numbersClips.Length) {
        EventBus<EventStructs.VariantAudioClickedEvent>.Raise(new EventStructs.VariantAudioClickedEvent { AudioClip = _numbersClips[number - 1] });
      }
    }
  }
}