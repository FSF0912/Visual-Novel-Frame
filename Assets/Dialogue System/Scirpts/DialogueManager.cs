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
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        [Header("UI References")]
        [SerializeField] private GameObject _imageSwitcherPrefab;
        [Space(5.0f)]
        [SerializeField] private RectTransform _charactersHolder;
        [SerializeField] private Character _background;
        [Header("Game Configuration")]
        public GameDefinitionAsset GameDefinition;
        [SerializeField] private KeyCode[] _activationKeys = 
        { KeyCode.Space, KeyCode.Return, KeyCode.F };

        private DialogueProfile _currentProfile;
        private Dictionary<int, Character> _characterDisplays = new();
        private int _currentIndex;
        [HideInInspector] public bool processingBranch = false;
        private BranchOption? currentBranchOption = null;
        private int returnIndex;

        [HideInInspector] public bool AllowInput = true;

        public bool InputReceived => 
            AllowInput && 
            (Input.GetMouseButtonDown(0) || _activationKeys.Any(a => Input.GetKeyDown(a)));

        private void Start()
        {
            _currentProfile = ProfileManager.LoadProfile(new EpisodeSymbol(1, 1));
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
            await _semaphore.WaitAsync();
            try
            {
                if (processingBranch) return; 
                if (_currentProfile == null || _currentIndex >= _currentProfile.actions.Length)
                {
                    Debug.Log("对话结束");
                    return;
                }

                if (currentBranchOption.HasValue && _currentIndex > currentBranchOption.Value.endIndex)
                {
                    _currentIndex = returnIndex;
                    currentBranchOption = null;
                    _semaphore.Release();
                    await ShowNextDialogue();
                    return;
                }

                var currentAction = _currentProfile.actions[_currentIndex];

                if (currentAction.isBranch)
                {
                    processingBranch = true;
                    var branchResult = currentAction.branchOptions[await BranchSelector.Instance.Branch(currentAction.branchOptions)];
                    currentBranchOption = branchResult;
                    returnIndex = branchResult.returnIndex;
                    _currentIndex = branchResult.jumpIndex;
                    processingBranch = false;
                    _semaphore.Release();
                    await ShowNextDialogue();
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
                            //var rt = character.transform as RectTransform;
                            //rt.anchoredPosition = new Vector2(-2000, -1500);
                            //rt.sizeDelta = new Vector2(0, 1100);
                            _characterDisplays.Add(option.characterDefindID, character);
                        }
                        print(GameDefinition == null);
                        var charparams = GameDefinition.GetPortraitAndExpression(
                            option.characterDefindID, option.characterImage, option.characterExpression);
                        character.Output(charparams.Item1, charparams.Item2, false, option);
                        character.transform.SetSiblingIndex(option.SortByListOrder ? order : option.CustomOrder);
                        order++;
                    }

                    _background.Output(GameDefinition.GetBackGround(currentAction.backGround));
                    AudioManager.Instance.PlayAudio(null, GameDefinition.GetAudio(currentAction.Music));
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
            finally
            {
                _semaphore.Release();
            }
        }

        private void OnDestroy()
        {
            Debug.Log($"Killed {DOTween.KillAll()} tweens.");
        }
    }
}