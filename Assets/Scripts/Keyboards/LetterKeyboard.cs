using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Keyboards
{
    public class LetterKeyboard : MonoBehaviour
    {
        [SerializeField] private List<LetterKey> allKeys;
        [SerializeField] private GameObject grid;

        private const string Alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        private List<LetterKey> _keys;

        private void Start()
        {
            SetAlphabet(Alphabet);
            SetInteractable(false);
        }

        public void SetInteractable(bool interactable)
        {
            grid.SetActive(interactable);
            _keys.ForEach(k => k.SetInteractable(interactable));
        }

        private void SetAlphabet(string alphabet)
        {
            _keys = allKeys.Take(alphabet.Length).ToList();
            
            for (var i = 0; i < allKeys.Count; i++)
            {
                if (i < alphabet.Length)
                {
                    allKeys[i].SetLetter(alphabet[i]);
                    allKeys[i].gameObject.SetActive(true);
                }
                else
                {
                    allKeys[i].gameObject.SetActive(false);
                }
            }
        }
    }
}