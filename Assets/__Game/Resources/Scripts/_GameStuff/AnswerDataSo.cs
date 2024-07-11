using UnityEngine;

namespace Assets.__Game.Resources.Scripts._GameStuff
{
  [CreateAssetMenu(menuName = "SOs/AnswerData", fileName = "AnswerName")]
  public class AnswerDataSo : ScriptableObject
  {
    public string AnswerValue;
    [Space]
    public Sprite CatSprite;
    [Space]
    public AudioClip AnswerClip;
  }
}