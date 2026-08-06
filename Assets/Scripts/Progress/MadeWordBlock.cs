using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Progress
{
    public class MadeWordBlock : MonoBehaviour
    {
        [SerializeField] private TMP_Text wordText;

        private const string ColorTag = "{0}<color=#{1}>{2}</color>{3}";

        public void SetWord(string word, int letterIndex, Color letterColor)
        {
            word = word.ToLower();
            word = word[0].ToString().ToUpper() + word.Substring(1);
            var begin = word[..letterIndex];
            
            var end = string.Empty;
            if (letterIndex < word.Length - 1)
            {
                end = word[(letterIndex + 1)..];
            }

            var hexString = letterColor.ToHexString();
            
            var letterString = word[letterIndex].ToString();

            wordText.text = string.Format(ColorTag, begin, hexString, letterString, end);
            
            //LayoutRebuilder.ForceRebuildLayoutImmediate(textRt);
            var sizeDelta =  wordText.rectTransform.sizeDelta;
            sizeDelta.x = wordText.preferredWidth;
            wordText.rectTransform.sizeDelta = sizeDelta;
        }
    }
}