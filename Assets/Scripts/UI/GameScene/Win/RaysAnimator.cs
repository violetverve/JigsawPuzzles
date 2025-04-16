using UnityEngine;
using DG.Tweening;

namespace UI.GameScene.Win
{
    public class RaysAnimator : MonoBehaviour
    {
        private float _duration = 40f;
        private Vector3 _rotationAmount = new Vector3(0, 0, -360);

        private void Start()
        {
            RotateContinuously();
        }

        void RotateContinuously()
        {
            transform.DOLocalRotate(_rotationAmount, _duration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental);
        }

    }
}

