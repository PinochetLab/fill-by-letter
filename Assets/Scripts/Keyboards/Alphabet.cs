using UnityEngine;

namespace Keyboards
{
    [CreateAssetMenu(fileName = "Alphabet", menuName = "Alphabet", order = 0)]
    public class Alphabet : ScriptableObject
    {
        [SerializeField] private string allLetters;
        [SerializeField] private string wovels;
        
        public string AllLetters => allLetters;
        public string Wovels => wovels;
    }
}