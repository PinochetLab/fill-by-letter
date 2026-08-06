using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordBoards
{
    public class WordBlock : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRt;
        [SerializeField] private TMP_Text text;

        public void SetWord(string word)
        {
            text.text = word;
            //LayoutRebuilder.ForceRebuildLayoutImmediate(textRt);
            var sizeDelta =  panelRt.sizeDelta;
            sizeDelta.x = text.preferredWidth + 60;
            panelRt.sizeDelta = sizeDelta;
        }

        public void ResetTransform()
        {
            panelRt.anchoredPosition = Vector3.zero;
            panelRt.localScale = Vector3.one;
        }
    }
}