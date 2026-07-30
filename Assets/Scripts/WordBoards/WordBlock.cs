using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordBoards
{
    public class WordBlock : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRt;
        [SerializeField] private TMP_Text text;
        [SerializeField] private RectTransform textRt;

        public void SetWord(string word)
        {
            text.text = word;
            //LayoutRebuilder.ForceRebuildLayoutImmediate(textRt);
            var sizeDelta =  textRt.rect.size;
            sizeDelta.x += 30;
            panelRt.sizeDelta = sizeDelta;
        }

        public void ResetTransform()
        {
            panelRt.anchoredPosition = Vector3.zero;
            panelRt.localScale = Vector3.one;
        }
    }
}