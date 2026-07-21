#if UNITY_EDITOR

using System.Collections.Generic;

namespace Words
{
    public class HashSetGlossaryBuilder : AbstractGlossaryBuilder
    {
        private readonly HashSet<string> _set = new();
        
        protected override string FolderName => "HashSet";
        
        protected override void Init()
        {
            _set.Clear();
        }

        protected override object Object => _set;
        
        protected override void AddWord(string word)
        {
            _set.Add(word);
        }
    }
}

#endif