using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts._GameStuff;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Resources.Scripts.SOs;
using Assets.__Game.Scripts.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static __Game.Resources.Scripts.EventBus.EventStructs;

public class WordsManager : MonoBehaviour
{
  [SerializeField] private string _correctText;
  [Header("")]
  [SerializeField] private SubLevel[] _subLevels;
  [Header("")]
  [SerializeField] private Button _submitButton;
  [Header("Effects")]
  [SerializeField] private ParticleSystem _winParticles;
  [SerializeField] private AudioSource _winAudioSource;
  [SerializeField] private ParticleSystem _loseParticles;
  [SerializeField] private AudioSource _loseAudioSource;

  private CorrectValuesContainerSo _currentCorrectValuesContainerSo;
  private List<WordButton> _wordButtons = new List<WordButton>();
  private HashSet<string> _correctValuesSet;
  private bool _canSubmit = true;
  private int _currentSubLevelIndex = 0;
  private Coroutine _stuporTimeoutRoutine;

  private Canvas _canvas;

  private GameBootstrapper _gameBootstrapper;

  private EventBinding<StateChanged> _stateChangedEvent;

  private void Awake()
  {
    _gameBootstrapper = GameBootstrapper.Instance;

    _canvas = GetComponent<Canvas>();

    _wordButtons.AddRange(GetComponentsInChildren<WordButton>());
  }

  private void OnEnable()
  {
    _submitButton.onClick.AddListener(OnSubmitButtonClick);

    _stateChangedEvent = new EventBinding<StateChanged>(StuporTimerDependsOnState);
  }

  private void OnDisable()
  {
    _stateChangedEvent.Remove(StuporTimerDependsOnState);
  }

  private void Start()
  {
    _canvas.worldCamera = Camera.main;

    EventBus<VariantsAssignedEvent>.Raise(new VariantsAssignedEvent());
    EventBus<QuestTextEvent>.Raise(new QuestTextEvent { QuestText = _correctText });

    ActivateSubLevel(_currentSubLevelIndex);
    ResetAndStartStuporTimer();
  }

  private void OnSubmitButtonClick()
  {
    if (_canSubmit == false) return;

    HashSet<string> selectedWords = new HashSet<string>();
    bool hasIncorrectSelection = false;

    foreach (var wordButton in _wordButtons)
    {
      string wordText = wordButton.GetWordText();

      if (wordButton.IsClicked)
      {
        selectedWords.Add(wordText);

        if (_correctValuesSet.Contains(wordText) == false)
        {
          hasIncorrectSelection = true;

          break;
        }
      }
      else
      {
        if (_correctValuesSet.Contains(wordText))
        {
          hasIncorrectSelection = true;

          break;
        }
      }
    }

    if (hasIncorrectSelection || selectedWords.Count != _correctValuesSet.Count)
    {
      _loseParticles.Play();
      _loseAudioSource.Play();

      _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameLoseState(_gameBootstrapper), 0.5f, this);
    }
    else
    {
      _currentSubLevelIndex++;

      PlayWinningAudioClip();

      _winParticles.Play();
      _winAudioSource.Play();

      if (_currentSubLevelIndex < _subLevels.Length)
        ActivateSubLevel(_currentSubLevelIndex);
      else
        _gameBootstrapper.StateMachine.ChangeStateWithDelay(new GameWinState(_gameBootstrapper), 1f, this);
    }

    EventBus<UiButtonEvent>.Raise(new UiButtonEvent());

    ResetAndStartStuporTimer();
  }

  private void ActivateSubLevel(int index)
  {
    for (int i = 0; i < _subLevels.Length; i++)
    {
      _subLevels[i].gameObject.SetActive(i == index);
    }

    _wordButtons.Clear();
    _wordButtons.AddRange(_subLevels[index].GetComponentsInChildren<WordButton>());

    _currentCorrectValuesContainerSo = _subLevels[index].GetCorrectValuesContainerSo();
    _correctValuesSet = new HashSet<string>(_currentCorrectValuesContainerSo.CorrectValues);

    _canSubmit = true;
  }

  private void PlayWinningAudioClip()
  {
    foreach (var wordButton in _wordButtons)
    {
      if (_correctValuesSet.Contains(wordButton.GetWordText()))
      {
        wordButton.PlayWordAudioCLip();

        break;
      }
    }
  }

  private void StuporTimerDependsOnState(StateChanged stateChanged)
  {
    if (stateChanged.State is GameplayState)
      ResetAndStartStuporTimer();
    else
    {
      if (_stuporTimeoutRoutine != null)
        StopCoroutine(_stuporTimeoutRoutine);
    }
  }

  private void ResetAndStartStuporTimer()
  {
    if (_stuporTimeoutRoutine != null)
      StopCoroutine(_stuporTimeoutRoutine);

    _stuporTimeoutRoutine = StartCoroutine(DoStuporTimerCoroutine());
  }

  private IEnumerator DoStuporTimerCoroutine()
  {
    yield return new WaitForSeconds(10);

    EventBus<StuporEvent>.Raise(new StuporEvent());

    ResetAndStartStuporTimer();
  }
}