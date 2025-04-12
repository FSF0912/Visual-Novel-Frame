using FSF.Collection;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.IO;

namespace FSF.VNG
{
    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        [Header("Variables")]
        [SerializeField] private GameObject _imageSwitcherPrefab;
        [Space(5.0f)]
        [SerializeField] private RectTransform _charactersHolder;
        [SerializeField] private Character _background;
        [Header("Settings")]
        [SerializeField] private DialogueProfile _profile;
        [SerializeField] private KeyCode[] _activationKeys = 
        { KeyCode.Space, KeyCode.Return, KeyCode.F };

        private Dictionary<int, Character> _characterDisplays = new();
        private int _currentIndex;
        public bool processingBranch = false;
        private BranchOption? currentBranchOption = null;
        private int returnIndex;

        [HideInInspector] public bool AllowInput = true;

        public bool InputReceived => 
            AllowInput && 
            (Input.GetMouseButtonDown(0) || _activationKeys.Any(Input.GetKeyDown));

        private void Start()
        {
            ShowNextDialogue().Forget();
        }

        private void Update()
        {
            if (InputReceived && !processingBranch)
            {
                ShowNextDialogue().Forget();
            }
        }

        public async UniTask ShowNextDialogue()
        {
            if (processingBranch) return; 
            if (_profile == null || _currentIndex >= _profile.actions.Length)
            {
                Debug.Log("对话结束");
                return;
            }

            if (currentBranchOption.HasValue && _currentIndex > currentBranchOption.Value.endIndex)
            {
                _currentIndex = returnIndex;
                currentBranchOption = null;
                ShowNextDialogue().Forget();
                return;
            }

            var currentAction = _profile.actions[_currentIndex];

            if (currentAction.isBranch)
            {
                processingBranch = true;
                var branchResult = currentAction.branchOptions[await BranchSelector.Instance.Branch(currentAction.branchOptions)];
                currentBranchOption = branchResult;
                returnIndex = branchResult.returnIndex;
                _currentIndex = branchResult.jumpIndex;
                processingBranch = false;
                ShowNextDialogue().Forget();
                return;
            }

            if (TypeWriter.Instance.OutputText(currentAction.name, currentAction.dialogue))
            {
                int order = 0;
                foreach (var option in currentAction.characterOptions)
                {
                    if (!_characterDisplays.TryGetValue(option.characterDefindID, out var character))
                    {
                        character = Instantiate(_imageSwitcherPrefab, _charactersHolder).GetComponent<Character>();
                        character.characterDefindID = option.characterDefindID;
                        character.name = $"Character_{option.characterDefindID}";
                        var rt = character.transform as RectTransform;
                        rt.anchoredPosition = new Vector2(-2000, -1500);
                        rt.sizeDelta = new Vector2(0, 1100);
                        _characterDisplays.Add(option.characterDefindID, character);
                    }

        #if VNG_EXPRESSION
                    character.Output(option.characterImage, option.characterExpression, false, option);
        #else
                    character.Output(option.characterImage, null, false, option);
        #endif
                    character.transform.SetSiblingIndex(option.SortByListOrder ? order : option.CustomOrder);
                    order++;
                }

                _background.Output(currentAction.backGround);
                AudioManager.Instance.PlayAudio(currentAction.audio, currentAction.bg_Music);
                _currentIndex++;
            }
            else
            {
                foreach (var display in _characterDisplays) display.Value.Interrupt();
                _background.Interrupt();
                if (Dialogue_Configs.InterruptVoicePlayback)
                {
                    AudioManager.Instance.InterruptAudio();
                }
            }
        }

        private void OnDestroy()
        {
            Debug.Log($"Killed {DOTween.KillAll()} tweens.");
        }
    }
}