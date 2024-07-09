using __Game.Resources.Scripts.EventBus;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  public class WordButton : MonoBehaviour
  {
    [Header("Visual")]
    [SerializeField] private Sprite _defaultColor;
    [SerializeField] private Sprite _selectedColor;
    [Header("Animation")]
    [Header("Audio")]
    [SerializeField] private AudioClip _wordAudioClip;

    public bool IsClicked { get; private set; }

    private Image _image;
    private Button _button;
    private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
      _image = GetComponent<Image>();
      _button = GetComponent<Button>();
      _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
      _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
      _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
      IsClicked = !IsClicked;
      _button.image.sprite = IsClicked ? _selectedColor : _defaultColor;

      transform.DOPunchScale(new Vector3(0.1f, 0.1f, 1), 0.25f, 0, 0);

      EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent());
    }

    public string GetWordText() => _textMeshPro.text;

    public void PlayWordAudioCLip()
    {
      EventBus<EventStructs.VariantAudioClickedEvent>.Raise(new EventStructs.VariantAudioClickedEvent { AudioClip = _wordAudioClip });
    }
  }
}