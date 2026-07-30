using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Animations
{
    public class SaleRotator : MonoBehaviour
    {
        [SerializeField] private Transform image;
        [SerializeField] private Transform shadow;
        [SerializeField] private float rotationSpeed = 100;
        
        private void Awake()
        {
            
            var duration = 360f / rotationSpeed;

            image
                .DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true);

            shadow
                .DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true);
        }
    }
}