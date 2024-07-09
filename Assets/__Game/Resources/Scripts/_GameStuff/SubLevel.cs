using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.SOs;
using UnityEngine;
using UnityEngine.UI;
using static __Game.Resources.Scripts.EventBus.EventStructs;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class SubLevel : MonoBehaviour
  {
    [SerializeField] private Button _voiceButton;
    [SerializeField] private AudioClip _voiceClip;
    [Header("")]
    [SerializeField] private CorrectValuesContainerSo _correctValuesContainerSo;
    [Header("Tutorial")]
    [SerializeField] private bool _tutorial;
    [SerializeField] private GameObject _tutorialFinger1;
    [SerializeField] private GameObject _tutorialFinger2;
    [Space]
    [SerializeField] private Button _tutorialVariant;

    public CorrectValuesContainerSo GetCorrectValuesContainerSo() => _correctValuesContainerSo;

    private void Start()
    {
      if (_tutorial == true)
      {
        _tutorialFinger1.SetActive(true);
        _tutorialFinger2.SetActive(false);
      }
    }

    private void OnEnable()
    {
      _voiceButton.onClick.AddListener(() =>
      {
        EventBus<VoiceButtonAudioEvent>.Raise(new VoiceButtonAudioEvent { AudioClip = _voiceClip });

        if (_tutorial == true)
        {
          _tutorialFinger1.SetActive(false);
          _tutorialFinger2.SetActive(true);
        }
      });

      if (_tutorial == true)
      {
        _tutorialVariant.onClick.AddListener(() =>
        {
          _tutorialFinger2.SetActive(false);
        });
      }
    }
  }
}