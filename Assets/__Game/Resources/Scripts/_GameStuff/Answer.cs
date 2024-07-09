using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class Answer : MonoBehaviour
  {
    [SerializeField] private string _answerNumber;
    [Header("References")]
    [SerializeField] private Button _answerButton;
    [SerializeField] private TextMeshProUGUI _numberText;
    [Header("Cats")]
    [SerializeField] private GameObject _catsContainer;
    [Space]
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectionColor;
    [Header("Stupor settings")]
    [SerializeField] private float _stuporTimeoutSeconds = 30f;
    [Header("Tutorial")]
    [SerializeField] private bool _allowTutorial;
    [SerializeField] private GameObject _tutorialFinger;

    private Image[] _catsImage;
    private Coroutine _stuporTimeoutRoutine;

    private GameBootstrapper _gameBootstrapper;

    private void Awake()
    {
      _gameBootstrapper = GameBootstrapper.Instance;

      GetAllCatsSpriteRenderers();
    }

    private void OnEnable()
    {
      _answerButton.onClick.AddListener(() =>
      {
        EnableNumberObject();

        _answerButton.interactable = false;

        ChangeCatsColors();
      });
    }

    private void Start()
    {
      _numberText.transform.localScale = Vector3.zero;
      _numberText.gameObject.SetActive(false);

      _numberText.text = _answerNumber;

      if (_allowTutorial == true)
        _tutorialFinger.SetActive(true);
      else
        _tutorialFinger.SetActive(false);
    }

    private void EnableNumberObject()
    {
      _numberText.gameObject.SetActive(true);

      Vector3 scale = new Vector3(1, 1, 1);

      _numberText.transform.DOScale(scale, 0.5f);

      if (_gameBootstrapper != null)
        _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameWinState(_gameBootstrapper), 1f, this);

      if (_allowTutorial == true)
        _tutorialFinger.SetActive(false);
    }

    private void ResetAndStartStuporTimer()
    {
      if (_stuporTimeoutRoutine != null)
        StopCoroutine(_stuporTimeoutRoutine);

      _stuporTimeoutRoutine = StartCoroutine(DoStuporTimerCoroutine());
    }

    private IEnumerator DoStuporTimerCoroutine()
    {
      yield return new WaitForSeconds(_stuporTimeoutSeconds);

      EventBus<EventStructs.StuporEvent>.Raise(new EventStructs.StuporEvent());

      ResetAndStartStuporTimer();
    }

    private void GetAllCatsSpriteRenderers()
    {
      _catsImage = _catsContainer.GetComponentsInChildren<Image>();

      foreach (var c in _catsImage)
      {
        c.color = _defaultColor;
      }
    }

    private void ChangeCatsColors()
    {
      Sequence sequence = DOTween.Sequence();

      foreach (var spriteRenderer in _catsImage)
      {
        sequence.Append(spriteRenderer.DOColor(_selectionColor, 0.2f))
                .Append(spriteRenderer.DOColor(_defaultColor, 0.2f));
      }

      sequence.Play();
    }
  }
}