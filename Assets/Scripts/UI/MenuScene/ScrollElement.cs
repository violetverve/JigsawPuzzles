using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameManagement.Difficulty;

namespace UI.MenuScene
{
    public class ScrollElement : MonoBehaviour
    {
        [Header("Element Colors")]
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _basicColor;

        [Header("Element Components")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _piecesText;
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private Button _button;
        private int _index;


        [Header("Element Parameters")]
        [SerializeField] private float _size;
        [SerializeField] private float _padding;
        [SerializeField] private float _basicScale;
        [SerializeField] private float _activeScale;
        private float _totalPaddingAndSize;

        [SerializeField, HideInInspector] private AnimationCurve _animationCurveSize;
        [SerializeField, HideInInspector] private AnimationCurve _animationCurveRed;
        [SerializeField, HideInInspector] private AnimationCurve _animationCurveBlue;
        [SerializeField, HideInInspector] private AnimationCurve _animationCurveGreen;
        [SerializeField, HideInInspector] private AnimationCurve _animationCurveOpacity;

        private void Awake()
        {
            _totalPaddingAndSize = _padding + _size;

            AdjustAnimationCurveSize();
            AdjustAnimationCurveColor();
            AdjustOpacityCurve();
        }

        private void OnEnable()
        {
            PuzzlePrepareUI.ItemChanging += SetBasicParameters;
            _button.onClick.AddListener(ElementClick);
        }

        private void OnDisable()
        {
            PuzzlePrepareUI.ItemChanging -= SetBasicParameters;
            _button.onClick.RemoveListener(ElementClick);
        }

        private void AdjustAnimationCurveSize()
        {
            _animationCurveSize.ClearKeys();

            _animationCurveSize.AddKey(_totalPaddingAndSize, _basicScale);
            _animationCurveSize.AddKey(-1 * _totalPaddingAndSize, _basicScale);
            _animationCurveSize.AddKey(0, _activeScale);
        }

        public void SetBasicParameters(float scrollPosition)
        {
            float relativePosition = CalculateRelativePosition(scrollPosition);
            SetScale(relativePosition);
            SetColor(relativePosition);
            SetOpacity(relativePosition);
        }

        private float CalculateRelativePosition(float scrollPosition)
        {
            return scrollPosition + _index * (_padding + _size);
        }

        private void SetScale(float relativePosition)
        {
            float scale = _animationCurveSize.Evaluate(relativePosition);
            _rectTransform.localScale = new Vector3(scale, scale);
        }

        private void SetColor(float relativePosition)
        {
            Color color = new Color(
                _animationCurveRed.Evaluate(relativePosition),
                _animationCurveGreen.Evaluate(relativePosition),
                _animationCurveBlue.Evaluate(relativePosition));
            _image.color = color;
        }

        private void SetOpacity(float relativePosition)
        {
            float opacity = _animationCurveOpacity.Evaluate(relativePosition);
            var color = _rewardText.color;
            color.a = opacity;

            _rewardText.color = color;
        }

        private void AdjustAnimationCurveColor()
        {
            AdjustColorCurve(_animationCurveRed, _basicColor.r, _activeColor.r);
            AdjustColorCurve(_animationCurveGreen, _basicColor.g, _activeColor.g);
            AdjustColorCurve(_animationCurveBlue, _basicColor.b, _activeColor.b);
        }

        private void AdjustColorCurve(AnimationCurve curve, float basicColorValue, float activeColorValue)
        {
            curve.ClearKeys();
            float keyPosition = _padding + _size;

            curve.AddKey(keyPosition, basicColorValue);
            curve.AddKey(-keyPosition, basicColorValue);
            curve.AddKey(0, activeColorValue);
        }

        private void AdjustOpacityCurve()
        {
            int inactiveOpacity = 0;
            int activeOpacity = 1;

            _animationCurveOpacity.ClearKeys();
            float keyPosition = _padding + _size;

            _animationCurveOpacity.AddKey(keyPosition, inactiveOpacity);
            _animationCurveOpacity.AddKey(-keyPosition, inactiveOpacity);
            _animationCurveOpacity.AddKey(0, activeOpacity);
        }

        public void Load(DifficultySO difficulty, int index)
        {
            _piecesText.text = difficulty.Grid.Area.ToString();
            _coinsText.text = difficulty.Reward.ToString();
            _index = index;

            RectTransform coinsRect = _coinsText.rectTransform;
            coinsRect.sizeDelta = new Vector2(_coinsText.preferredWidth, coinsRect.sizeDelta.y);
        }

        private void ElementClick()
        {
            PuzzlePrepareUI.OnElementClick?.Invoke(_index);
        }

    }

}
