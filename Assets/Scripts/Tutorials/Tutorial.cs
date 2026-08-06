using System.Collections.Generic;
using UnityEngine;

namespace Tutorials
{
    [CreateAssetMenu(fileName = "Tutorial", menuName = "Tutorial", order = 0)]
    public class Tutorial : ScriptableObject
    {
        [SerializeField] private List<TutorialStep> steps;
        
        [TextArea(3, 5)] [SerializeField] private string targetText;
        [TextArea(3, 5)] [SerializeField] private string selectCell1Text;
        [TextArea(3, 5)] [SerializeField] private string typeLetter1Text;
        [TextArea(3, 5)] [SerializeField] private string writeWordText;
        [TextArea(3, 5)] [SerializeField] private string selectCell2Text;
        [TextArea(3, 5)] [SerializeField] private string typeLetter2Text;
        [TextArea(3, 5)] [SerializeField] private string startWord1Text;
        [TextArea(3, 5)] [SerializeField] private string addLetter1Text;
        [TextArea(3, 5)] [SerializeField] private string cancelText;
        [TextArea(3, 5)] [SerializeField] private string startWord2Text;
        [TextArea(3, 5)] [SerializeField] private string addLetter2Text;
        [TextArea(3, 5)] [SerializeField] private string okText;
        
        public  List<TutorialStep> Steps => steps;
        
        public string TargetText => targetText;
        public string SelectCell1Text => selectCell1Text;
        public string TypeLetter1Text => typeLetter1Text;
        public string WriteWordText => writeWordText;
        public string SelectCell2Text => selectCell2Text;
        public string TypeLetter2Text => typeLetter2Text;
        public string StartWord1Text => startWord1Text;
        public string AddLetter1Text => addLetter1Text;
        public string CancelText => cancelText;
        public string StartWord2Text => startWord2Text;
        public string AddLetter2Text => addLetter2Text;
        public string OkText => okText;
    }
}