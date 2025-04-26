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
        [HideInInspector] public bool isBuzy;

        RectTransform selfRTransform;
        Sequence _sequence;


        bool char_isFirstImage;
        private float _char_mixWeight;
        private float _cachedClampValue;

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
                    expression_Renderer1.SetAlpha(Mathf.Min(value, expression_Renderer1.GetAlpha()));
                    expression_Renderer2.SetAlpha(Mathf.Min(value, expression_Renderer2.GetAlpha()));
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
                expression_Renderer1.SetAlpha(Mathf.Min(value, char_Renderer1.GetAlpha()));
                expression_Renderer2.SetAlpha(Mathf.Min(value, char_Renderer2.GetAlpha()));
            }
        }

        private void Awake()
        {
            char_mixWeight = 1;
            char_isFirstImage = true;
            totalFade.alpha = 1;
            selfRTransform = (RectTransform)this.transform;
            isBuzy = false;
        }

        public void Output(Sprite target = null, Sprite expression = null, bool direct = false, CharacterOption option = null)
        {
            if (!direct)
            {
                _sequence?.Kill(true);
                _sequence = DOTween.Sequence();
                if (target == null)
                {
                    CharacterMovement();
                    return;
                }
                Sprite currentSprite = char_isFirstImage ? char_Image1.sprite : char_Image2.sprite;
                if (target != currentSprite)
                {
                    Image targetImage = char_isFirstImage ? char_Image2 : char_Image1;
                    targetImage.sprite = target;
                     _sequence.Join(DOTween.To(() => char_mixWeight, x => char_mixWeight = x,
                        char_isFirstImage ? 0 : 1, Dialogue_Configs.characterTranslationTime));
                    char_isFirstImage = !char_isFirstImage;
                }
                if (UseExpression)
                {
                    if (expression == null) return;
                    Sprite currentExpression = expression_isFirstImage ? expression_Image1.sprite : expression_Image2.sprite;
                    if (expression != currentExpression)
                    {
                        Image targetImage = expression_isFirstImage ? expression_Image2 : expression_Image1;
                        targetImage.sprite = expression;
                        _sequence.Join(DOTween.To(() => expression_isFirstImage ? 1 : 0, x => expression_mixWeight = x,
                            expression_isFirstImage ? 0 : 1, Dialogue_Configs.characterTranslationTime));
                        expression_isFirstImage = !expression_isFirstImage;
                    }
                }
                CharacterMovement();
            }
            else
            {
                if (target == null) return;
                (char_isFirstImage ? char_Image1 : char_Image2).sprite = target;
                return;
            }

            void CharacterMovement() {
                if (option == null) return;
                switch (option.presenceStatus)
                {
                    case CharacterPresenceStatus.None: break;

                    case CharacterPresenceStatus.Enter:
                        if (isBuzy) break;
                        isBuzy = true;
                        totalFade.alpha = 0;
                        _sequence.Join(
                            totalFade.DOFade(1, option.action_Duration)
                        );
                        break;

                    case CharacterPresenceStatus.Exit:
                        if (!isBuzy) break;
                        isBuzy = false;
                        totalFade.alpha = 1;
                        _sequence.Join(
                            totalFade.DOFade(0, option.action_Duration)
                        );
                        break;
                }

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
        }

        public void Interrupt()
        {
            _sequence.Kill(true);
        }
    }
}