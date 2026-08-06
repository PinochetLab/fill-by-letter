using System;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Words
{
    public class GlossaryLoader<T> where T : new()
    {
        private T _t;
        private bool _loaded;
        private readonly SemaphoreSlim _semaphore = new(0, 1);

        public async UniTask Load(TextAsset textAsset)
        {
            if (textAsset == null)
            {
                Debug.LogError("TextAsset is null!");
                _t = new T();
                _loaded = true;
                _semaphore?.Release();
                return;
            }

            // Просто берем текст
            string json = textAsset.text;
    
            // Убираем BOM
            json = json.TrimStart('\uFEFF');
    
            // Десериализуем
            try
            {
                _t = await UniTask.RunOnThreadPool(() => JsonConvert.DeserializeObject<T>(json));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка: {ex.Message}");
                _t = new T();
            }
    
            _loaded = true;
            _semaphore?.Release();
        }

        public T GetGlossary()
        {
            if (_loaded)
            {
                return _t;
            }
            
            _semaphore.Wait();
            
            _loaded = true;
            return _t;
        }
    }
}