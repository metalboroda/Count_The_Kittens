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

    [Header("Settings")]
    [SerializeField] private string _answerButtonValue;
    [Header("")]
    [Header("References")]
    [Header("Buttons")]
    [SerializeField] private Button _catButton;
    [SerializeField] private Button _answerButton;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _numberText;
    [SerializeField] private TextMeshProUGUI _answerButtonText;
    [Header("Tutorial")]
    [SerializeField] private bool _tutorial;
    [SerializeField] private GameObject _catFinger;
    [SerializeField] private GameObject _answerFinger;
    [Header("Audio")]
    [SerializeField] private float _minRandomPitch = 0.95f;
    [SerializeField] private float _maxRandomPitch = 1.05f;
    [Header("")]
    [SerializeField] private AudioClip _meowClip;

    private AudioSource _audioSource;

    private void Awake() {
      _audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
      SetAnswerButtonText();
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

    private void SetAnswerButtonText() {
      _answerButtonText.text = _answerButtonValue;
    }

    private void SubscribeButtons() {
      _catButton.onClick.AddListener(() => {
        CatButtonPressed?.Invoke(this);

        _catButton.interactable = false;

        PlayAudioClipWithRandomPitch(_meowClip);

        _answerButton.gameObject.SetActive(true);
        _answerButton.GetComponent<DOTweenAnimation>().DOPlay();

        TutorialSwitch(2);
      });

      _answerButton.onClick.AddListener(() => {
        AnswerButtonPressed?.Invoke(_answerButtonText.text);

        TutorialSwitch(3);
      });
    }

    private void HandleTexts() {
      _numberText.gameObject.SetActive(false);
    }

    private void HandleButtons() {
      _answerButton.gameObject.SetActive(false);
    }

    private void PlayAudioClipWithRandomPitch(AudioClip audioClip) {
      if (_audioSource == null) return;

      float randomPitch = Random.Range(_minRandomPitch, _maxRandomPitch);

      _audioSource.pitch = randomPitch;

      _audioSource.PlayOneShot(audioClip);
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