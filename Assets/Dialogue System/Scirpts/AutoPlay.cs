using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;

namespace FSF.VNG
{
    public class AutoPlayController : MonoBehaviour
    {
        private float AutoPlayWaitTime;
        private bool _isAutoPlaying;
        private CancellationTokenSource _cts;
        private bool _wasProcessingLastFrame;
        private DialogueManager dialogueManager;

        private void Start()
        {
            AutoPlayWaitTime = Dialogue_Configs.AutoPlayWaitTime;
            _isAutoPlaying = false;
            dialogueManager = DialogueManager.Instance;
        }

        private void Update()
        {
            if (!_isAutoPlaying) return;

            if (dialogueManager.InputReceived && !dialogueManager.processingBranch)
            {
                ToggleAutoPlay(false);
            }
        }

        public void ToggleAutoPlay(bool enable)
        {
            if (_isAutoPlaying == enable) return;

            _isAutoPlaying = enable;
            
            if (enable)
            {
                _cts = new CancellationTokenSource();
                StartAutoPlay().Forget();
            }
            else
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async UniTaskVoid StartAutoPlay()
        {
            var token = _cts.Token;
            try
            {
                while (_isAutoPlaying)
                {
                    await HandleTiming(token);
                    if (!_isAutoPlaying) break;
                    
                    await DialogueManager.Instance.ShowNextDialogue();
                    await UniTask.Yield(token);
                }
            }
            catch (System.OperationCanceledException) { }
        }

        private async UniTask HandleTiming(CancellationToken token)
        {
            var voiceSource = AudioManager.Instance.voice_Source;
            
            if (voiceSource.isPlaying)
            {
                await UniTask.WaitUntil(() => !voiceSource.isPlaying, cancellationToken: token);
                await UniTask.WaitForSeconds(AutoPlayWaitTime, cancellationToken: token);
            }
            else
            {
                await UniTask.WaitForSeconds(AutoPlayWaitTime, cancellationToken: token);
            }
        }

        private void OnDestroy()
        {
            ToggleAutoPlay(false);
        }
    }
}