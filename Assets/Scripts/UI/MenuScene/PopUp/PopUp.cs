using UnityEngine;
using DG.Tweening;

public class PopUp : MonoBehaviour
{
    private void OnEnable()
    {
        AnimatePopUp();
    }

    private void AnimatePopUp()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }
}
