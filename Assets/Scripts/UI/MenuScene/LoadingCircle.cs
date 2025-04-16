using DG.Tweening;
using UnityEngine;

namespace JigsawPuzzles.UI.MenuScene
{
    public class LoadingCircle : MonoBehaviour
    {
        public float rotationDuration = 1f; 

        void Start()
        {
            RotateCircle();
        }

        void RotateCircle()
        {
            gameObject.transform.DORotate(new Vector3(0, 0, -360), rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }
}
