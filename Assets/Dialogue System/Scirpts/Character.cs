using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FSF.VNG
{
    public class Character : MonoBehaviour
    {
        public int characterDefindID = 0;
        public Image firstImage, secondImage;
        CanvasRenderer firstRenderer, secondRenderer;
        public bool isBuzy = false;
        bool isFirstImage;
        RectTransform selfRTransform;
        Sequence _sequence;
        private void Awake()
        {
            firstImage.transform.SetAsLastSibling();
            isFirstImage = true;
            firstRenderer = firstImage.GetComponent<CanvasRenderer>();
            secondRenderer = secondImage.GetComponent<CanvasRenderer>();
            firstRenderer.SetAlpha(1);
            secondRenderer.SetAlpha(0);
            selfRTransform = this.transform as RectTransform;
            if (characterDefindID == -10000) isBuzy = true;
        }

        public void Output(Sprite target = null, bool image_direct = false, CharacterOption option = null)
        {
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            if (isFirstImage)
            {
                secondImage.sprite = target;
                secondImage.transform.SetAsLastSibling();
            }
            else
            {
                firstImage.sprite = target;
                firstImage.transform.SetAsLastSibling();
            }
            SetAlpha();

            if (option == null) return;
            switch (option.motionMode)
            {
                case MotionPresents.None:
                    switch (option.behaviourMode)
                    {
                        case CharacterBehaviourMode.None:
                            break;

                        case CharacterBehaviourMode.ShakeHeavily:
                            
                            _sequence.Join(
                                selfRTransform.DOShakeAnchorPos(
                                option.action_Duration, 
                                strength : 50
                                ).SetEase(option.action_Ease)
                            );
                            break;

                        case CharacterBehaviourMode.ShackSlightly:
                            _sequence.Join(
                                selfRTransform.DOShakeAnchorPos(
                                option.action_Duration, 
                                strength : 10
                                ).SetEase(option.action_Ease)
                            );
                            break;
                        
                        case CharacterBehaviourMode.HorizontalMove:
                            _sequence.Join(
                                selfRTransform.DOShakeAnchorPos(
                                option.action_Duration, 
                                new Vector2(45f, 0),
                                2,
                                20f,
                                false,
                                true,
                                ShakeRandomnessMode.Harmonic
                                ).SetEase(option.action_Ease)
                            );
                            break;

                        case CharacterBehaviourMode.VerticalMove:
                            _sequence.Join(
                                selfRTransform.DOShakeAnchorPos(
                                option.action_Duration,
                                new Vector2(0, -45f),
                                2,
                                20f,
                                false,
                                true,
                                ShakeRandomnessMode.Harmonic
                                ).SetEase(option.action_Ease)
                            );
                            break;

                        default: break;
                    }
                    break;

                case MotionPresents.LeftEnterToCenter:
                    selfRTransform.anchoredPosition = new(-1920, 0);
                    MoveToZero();
                    break;

                case MotionPresents.RightEnterToCenter:
                    selfRTransform.anchoredPosition = new(1920, 0);
                    MoveToZero();
                    break;

                case MotionPresents.BottomEnterToCenter:
                    selfRTransform.anchoredPosition = new(0, -1080);
                    MoveToZero();
                    break;

                case MotionPresents.LeftEscape:
                    MoveToAppointedPosition(new(-1920, 0));
                    break;

                case MotionPresents.RightEscape:
                    MoveToAppointedPosition(new(1920, 0));
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
                    if (option.useOrigin)
                    {
                        selfRTransform.anchoredPosition = option.origin;
                    }
                    _sequence.Join(
                        selfRTransform.DOAnchorPos(option.appointedPosition, option.action_Duration)
                        .SetEase(option.action_Ease)
                    );
                    break;

                default: break;
            }

            void MoveToZero() {
                MoveToAppointedPosition(Vector2.zero);
            }

            void MoveToAppointedPosition(Vector2 pos) {
                _sequence.Join(
                    selfRTransform.DOAnchorPos(pos, option.action_Duration)
                    .SetEase(option.action_Ease)
                );
            }

            void SetAlpha() {
                if (image_direct)
                {
                    return;
                }

                if (target == null)
                {
                    if (isFirstImage) DOFirst();
                    else DOSecond();
                    isFirstImage = !isFirstImage;
                    return;
                }

                DOFirst();
                DOSecond();
                isFirstImage = !isFirstImage;

                void DOFirst() {
                    _sequence.Join(
                        DOTween.To(() => isFirstImage ? 1.0f : 0.0f, x => firstRenderer.SetAlpha(x), isFirstImage ? 0.0f : 1.0f, 0.3f)
                    );
                }

                void DOSecond() {
                    _sequence.Join(
                        DOTween.To(() => isFirstImage ? 0.0f : 1.0f, x => secondRenderer.SetAlpha(x), isFirstImage ? 1.0f : 0.0f, 0.3f)
                    );
                }

            }
        }

        public void Interrupt()
        {
            _sequence?.Complete();
        }
    }
}
