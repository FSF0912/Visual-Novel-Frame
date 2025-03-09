using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FSF.VNG
{
    public class Character : MonoBehaviour
    {
        public int characterDefindID = 0;
        public bool UseExpression = false;
        public CanvasGroup totalFade;
        public Image char_Image1, char_Image2;
        public CanvasRenderer char_Renderer1, char_Renderer2;

        public Image expression_Image1, expression_Image2;
        public CanvasRenderer expression_Renderer1, expression_Renderer2;

        RectTransform selfRTransform;
        Sequence _sequence;

        bool char_isFirstImage;
        private float _char_mixWeight;
        private float _cachedClampValue;
        private float _expressionFactor1, _expressionFactor2;

        public float char_mixWeight
        {
            get => _char_mixWeight;
            set
            {
                _char_mixWeight = value;
                _cachedClampValue = Mathf.Clamp01(value);
                float inverseValue = 1 - _cachedClampValue;

                char_Renderer1.SetAlpha(_cachedClampValue);
                char_Renderer2.SetAlpha(inverseValue);
                
                if (UseExpression)
                {
                    expression_Renderer1.SetAlpha(_expressionFactor1 * _cachedClampValue);
                    expression_Renderer2.SetAlpha(_expressionFactor2 * inverseValue);
                }
            }
        }

        bool expression_isFirstImage;
        private float _expression_mixWeight;
        public float expression_mixWeight
        {
            get => _expression_mixWeight;
            set
            {
                _expression_mixWeight = value;
                float clampedValue = Mathf.Clamp01(value);
                
                _expressionFactor1 = char_Renderer1.GetAlpha();
                _expressionFactor2 = char_Renderer2.GetAlpha();
                
                expression_Renderer1.SetAlpha(_expressionFactor1 * clampedValue);
                expression_Renderer2.SetAlpha(_expressionFactor2 * (1 - clampedValue));
            }
        }

        private void Awake()
        {
            char_mixWeight = 1;
            char_isFirstImage = true;
            totalFade.alpha = 1;
            selfRTransform = (RectTransform)this.transform;
        }

        public void Output(Sprite target = null, bool image_direct = false, CharacterOption option = null)
        {
            _sequence?.Kill(true);
            _sequence = DOTween.Sequence();

            if (target != null || target != char_isFirstImage ? char_Image2.sprite : char_Image1.sprite)
            {
                Image targetImage = char_isFirstImage ? char_Image2 : char_Image1;
                if (!image_direct)
                {
                    targetImage.sprite = target;
                    _sequence.Join(DOTween.To(() => char_mixWeight, x => char_mixWeight = x,
                        char_isFirstImage ? 0 : 1, Dialogue_Configs.characterTranslationTime));

                    char_isFirstImage = !char_isFirstImage;
                }
                else
                {
                    (char_isFirstImage ? char_Image1 : char_Image2).sprite = target;
                }
            }

            if (option == null) return;
            Vector2 leftPos = new(-1920, 0);
            Vector2 rightPos = new(1920, 0);
            Vector2 bottomPos = new(0, -1080);

            switch (option.motionMode)
            {
                case MotionPresents.None: break;

                case MotionPresents.LeftEnterToCenter:
                    selfRTransform.anchoredPosition = leftPos;
                    MoveToZero();
                    break;

                case MotionPresents.RightEnterToCenter:
                    selfRTransform.anchoredPosition = rightPos;
                    MoveToZero();
                    break;

                case MotionPresents.BottomEnterToCenter:
                    selfRTransform.anchoredPosition = bottomPos;
                    MoveToZero();
                    break;

                case MotionPresents.LeftEscape:
                    MoveToAppointedPosition(leftPos);
                    break;

                case MotionPresents.RightEscape:
                    MoveToAppointedPosition(rightPos);
                    break;

                case MotionPresents.ToCenter:
                    MoveToZero();
                    break;

                case MotionPresents.ToLeft:
                    MoveToAppointedPosition(new(-500, 0));
                    break;

                case MotionPresents.ToRight:
                    MoveToAppointedPosition(new(500, 0));
                    break;

                case MotionPresents.Custom when option.useOrigin:
                    selfRTransform.anchoredPosition = option.origin;
                    goto default;

                case MotionPresents.Custom:
                    _sequence.Join(selfRTransform.DOAnchorPos(option.appointedPosition, 
                        option.action_Duration).SetEase(option.action_Ease));
                    break;

                case MotionPresents.ShakeHeavily:
                    _sequence.Join(selfRTransform.DOShakeAnchorPos(
                        option.action_Duration, 
                        strength: 50).SetEase(option.action_Ease));
                    break;

                case MotionPresents.ShackSlightly:
                    _sequence.Join(selfRTransform.DOShakeAnchorPos(
                        option.action_Duration, 
                        strength: 10).SetEase(option.action_Ease));
                    break;
                
                case MotionPresents.HorizontalMove:
                    _sequence.Join(selfRTransform.DOShakeAnchorPos(
                        option.action_Duration, 
                        new Vector2(45f, 0),
                        2,
                        20f,
                        false,
                        true,
                        ShakeRandomnessMode.Harmonic).SetEase(option.action_Ease));
                    break;

                case MotionPresents.VerticalMove:
                    _sequence.Join(selfRTransform.DOShakeAnchorPos(
                        option.action_Duration,
                        new Vector2(0, -45f),
                        2,
                        20f,
                        false,
                        true,
                        ShakeRandomnessMode.Harmonic).SetEase(option.action_Ease));
                    break;

                default: break;
            }

            void MoveToZero() => MoveToAppointedPosition(Vector2.zero);
            
            void MoveToAppointedPosition(Vector2 pos)
            {
                _sequence.Join(selfRTransform.DOAnchorPos(pos, option.action_Duration)
                    .SetEase(option.action_Ease));
            }
        }

        public void Interrupt()
        {
            _sequence.Kill(true);
        }
    }
}