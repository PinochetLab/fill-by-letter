using System.Collections.Generic;
using UnityEngine;

namespace Yandex
{
    [System.Serializable]
    public class LocalizedData
    {
        [SerializeField] private Dictionary<string, Data> data = new ();

        public Data GetData(string lang)
        {
            if (!data.ContainsKey(lang))
            {
                data.Add(lang, new Data());
            }

            return data[lang];
        }
    }
}