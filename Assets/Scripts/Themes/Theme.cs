using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Themes
{
    [CreateAssetMenu(fileName = "Theme", menuName = "Theme", order = 0)]
    public class Theme : ScriptableObject
    {
        [SerializeField] private string themeName;
        [SerializeField] private int multiplier;
        [SerializeField] private Sprite sprite;
        [SerializeField] private AssetReference glossary;
        
        public string ThemeName => themeName;
        public int Multiplier => multiplier;
        public Sprite Sprite => sprite;
        public AssetReference Glossary => glossary;
    }
}