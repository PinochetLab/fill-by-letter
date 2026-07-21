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
        
        public async UniTask Load(AssetReference assetReference)
        {
            try
            {
                var textAsset = await Addressables.LoadAssetAsync<TextAsset>(assetReference);
                _t = JsonConvert.DeserializeObject<T>(textAsset.text);
                _semaphore.Release();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Ошибка загрузки словаря: {ex.Message}");
                _t = new T();
                _semaphore.Release();
            }
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