using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace FSF.VNG
{
    public class Char : MonoBehaviour
    {
        public RawImage target;
        public float alpha
        {
            get{
                return material.GetFloat(FadeID);
            }
            set{
                material.SetFloat(FadeID, Mathf.Clamp01(value));
            }
        }
        
        Material material;
        bool isFirstImage;
        int FirstTexID, SecondTexID, WeightID, FadeID;
        Tween fadeTween, WeightTween;

        float time = 0.3f;

        private void Start()
        {
            material = new Material(Shader.Find("Custom/BlendFade"));
            target.material = material;
            isFirstImage = true;
            FirstTexID = Shader.PropertyToID("_FirstTex");
            SecondTexID = Shader.PropertyToID("_SecondTex");
            WeightID = Shader.PropertyToID("_Weight");
            FadeID = Shader.PropertyToID("_TotalFade");
        }

        public void OutputImage(Texture targetTexture)
        {
            if (targetTexture == null) return;
            WeightTween?.Kill();
            if (isFirstImage) 
            {  
                material.SetTexture(SecondTexID, targetTexture);
                WeightTween= material.DOFloat(1, WeightID, time);
                isFirstImage = false;
            }
            else
            {
                material.SetTexture(FirstTexID, targetTexture);
                WeightTween = material.DOFloat(0, WeightID, time);
                isFirstImage = true;
            }
        }

        public void OutputImageDirect(Texture targetTexture)
        {
            if (targetTexture == null) return;
            WeightTween?.Complete();
            material.SetTexture(material.GetFloat(WeightID) == 0 ?FirstTexID : SecondTexID, 
                                targetTexture);
        }
    }
}