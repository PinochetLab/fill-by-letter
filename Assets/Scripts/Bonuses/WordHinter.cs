using System;
using TMPro;
using UnityEngine;

namespace Bonuses
{
    public class WordHinter : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text wordText;

        private void Awake()
        {
            StopHint();
        }

        public void StartHint(string word)
        {
            wordText.text = word;
            panel.SetActive(true);
        }

        public void StopHint()
        {
            panel.SetActive(false);
        }
    }
}