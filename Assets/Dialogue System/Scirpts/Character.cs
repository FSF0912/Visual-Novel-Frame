using FSF.Collection.Utilities;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace FSF.VNG
{
    public class Character : MonoBehaviour
    {
        public int characterDefindID = 0;
        public Image targetImage1, targetImage2;
        public CanvasGroup targetCanvasGroup1, targetCanvasGroup2;

        /// <summary>
        /// 为true时，image1在上层(层级中最下面)，
        // image2在下层(层级中最上面)，反之...
        /// </summary>
        bool switcher;

        RectTransform selfRTransform;
        Vector2 tempPosition;
        Tween image1_Tween, image2_Tween;
        Tween movementTween, distanceTween;

        private void Awake()
        {
            targetImage1.transform.SetAsLastSibling();
            switcher = true;
            targetCanvasGroup1.alpha = 1;
            targetCanvasGroup2.alpha = 0;
            selfRTransform = this.transform as RectTransform;
        }

        public void OutputImage(Sprite target = null)
        {
            if(target == null) {return;}
            image1_Tween?.Kill();
            image2_Tween?.Kill();
            float switchTime = 0.3f;
            if (switcher)
            {//切换为image2,image1渐隐,image2渐显。？
                targetImage2.sprite = target;
                targetImage2.transform.SetAsLastSibling();
                //DOFade。。。
                targetCanvasGroup1.DOFade(0, switchTime);
                targetCanvasGroup2.DOFade(1, switchTime);
                //
                switcher = false;
            }
            else
            {//切换为image1,image2渐隐,image1渐显？。
                targetImage1.sprite = target;
                targetImage1.transform.SetAsLastSibling();
                //
                targetCanvasGroup1.DOFade(1, switchTime);
                targetCanvasGroup2.DOFade(0, switchTime);
                //
                switcher = true;
            }
        }

        public void OutputImageDirect(Sprite target)
        {   
            if (target == null) {return;}
            if (switcher)
            {
                targetImage1.sprite = target;
            }
            else
            {
                targetImage2.sprite = target;
            }
        }

        public void Interrupt()
        {
            image1_Tween?.Kill();
            image2_Tween?.Kill();
            movementTween?.Kill();
            distanceTween?.Kill();
            targetCanvasGroup1.alpha = switcher ? 1 : 0;
            targetCanvasGroup2.alpha = switcher ? 0 : 1;
            selfRTransform.anchoredPosition = tempPosition;
        }

        public void Animate(CharacterOption option)
        {
            movementTween?.Kill();
            distanceTween?.Kill();
            switch (option.motionMode)
            {
                case MotionPresents.None:
                    switch (option.behaviourMode)
                    {
                        case CharacterBehaviourMode.None:
                            break;

                        case CharacterBehaviourMode.ShakeHeavily:
                            movementTween = selfRTransform.DOShakeAnchorPos(
                                option.action_Duration, 
                                strength : 50
                                );
                                movementTween.SetEase(option.action_Ease);
                            break;

                        case CharacterBehaviourMode.ShackSlightly:
                            movementTween = selfRTransform.DOShakeAnchorPos(
                                option.action_Duration, 
                                strength : 10
                                );
                                movementTween.SetEase(option.action_Ease);
                            break;
                        
                        case CharacterBehaviourMode.HorizontalMove:
                            movementTween = selfRTransform.DOShakeAnchorPos(
                                option.action_Duration, 
                                new Vector2(45f, 0),
                                2,
                                20f,
                                false,
                                true,
                                ShakeRandomnessMode.Harmonic
                                );
                                movementTween.SetEase(option.action_Ease);
                            break;

                        case CharacterBehaviourMode.VerticalMove:
                            movementTween = selfRTransform.DOShakeAnchorPos(
                                option.action_Duration,
                                new Vector2(0, -45f),
                                2,
                                20f,
                                false,
                                true,
                                ShakeRandomnessMode.Harmonic
                                );
                                movementTween.SetEase(option.action_Ease);
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
                    movementTween = selfRTransform.DOAnchorPos(option.appointedPosition, option.action_Duration);
                    movementTween.SetEase(option.action_Ease);
                    break;

                default: break;
            }

            void MoveToZero()
            {
                MoveToAppointedPosition(Vector2.zero);
            }

            void MoveToAppointedPosition(Vector2 pos)
            {
                movementTween = selfRTransform.DOAnchorPos(pos, option.action_Duration);
                movementTween.SetEase(option.action_Ease);
                tempPosition = pos;
            }
        }
    }
}
