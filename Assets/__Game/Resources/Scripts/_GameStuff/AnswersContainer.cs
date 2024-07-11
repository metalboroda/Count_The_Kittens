using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class AnswersContainer : MonoBehaviour
  {
    [SerializeField] private string _correctValue;
    [Header("")]
    [SerializeField] private Answer[] _answers;

    private int _currentTextNumber = 1;

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

      _currentTextNumber++;
    }

    private void OnAnswerButton(string buttonValue) {
      if (buttonValue == _correctValue) {
        Debug.Log("Win");
      }
      else {
        Debug.Log("Lose");
      }
    }
  }
}