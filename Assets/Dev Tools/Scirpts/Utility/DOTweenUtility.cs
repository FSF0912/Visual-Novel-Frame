using TMPro;

namespace DG.Tweening{
    public static class DOTweenUtility
    {
        public static Core.TweenerCore<string, string, Plugins.Options.StringOptions> 
        DOText(this TextMeshProUGUI tmp, string endValue, float duration, bool ignoreOldValue = true){
            string oldValue = ignoreOldValue ? string.Empty : tmp.text;
            var t = DOTween.To(()=> oldValue, a => tmp.text = a, endValue, duration);
            t.SetOptions(true);
            return t;
        }
    }
}
