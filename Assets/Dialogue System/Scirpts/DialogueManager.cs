using FSF.Collection;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace FSF.VNG
{
    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        [Header("Variables")]
        [SerializeField] private GameObject _imageSwitcherPrefab;
        [SerializeField] private GameObject _branchOptionButtonPrefab;
        [Space(5.0f)]
        [SerializeField] private RectTransform _charactersHolder;
        [SerializeField] private RectTransform _branchOptionsHolder;
        [SerializeField] private Character _background;
        [Header("Settings")]
        [SerializeField] private DialogueProfile _profile;
        [SerializeField] private KeyCode[] _activationKeys = 
        { KeyCode.Space, KeyCode.Return, KeyCode.F };
        [SerializeField] private List<Character> _characterDisplays = new();

        private int _currentIndex;
        private bool processingBranch = false;
        private BranchOption? currentBranchOption = null;
        private Stack<int> returnIndexStack = new();
        [HideInInspector] public bool AllowInput = true;

        private bool InputReceived => 
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
            /*if (_profile == null || _currentIndex >= _profile.actions.Length)
            {
                _currentIndex = 0;
            }*/

            if (currentBranchOption.HasValue && _currentIndex >= currentBranchOption.Value.endIndex)
            {
                _currentIndex = returnIndexStack.Pop();
                currentBranchOption = null;
                return;
            }

            var currentAction = _profile.actions[_currentIndex];

            if (currentAction.isBranch)
            {
                processingBranch = true;
                var branchResult = currentAction.branchOptions[await BranchSelector.Instance.Branch(currentAction.branchOptions)];
                currentBranchOption = branchResult;
                returnIndexStack.Push(branchResult.returnIndex);
                _currentIndex = branchResult.jumpIndex;
                processingBranch = false;
                ShowNextDialogue().Forget();
                return;
            }

            if (TypeWriter.Instance.OutputText(currentAction.name, currentAction.dialogue))
            {
                int order = 0; // 恢复手动计数
                foreach (var option in currentAction.characterOptions)
                {
                    var character = _characterDisplays.FirstOrDefault(x => x.characterDefindID == option.characterDefindID);
                    if (character == null)
                    {
                        character = Instantiate(_imageSwitcherPrefab, _charactersHolder).GetComponent<Character>();
                        character.characterDefindID = option.characterDefindID;
                        character.name = $"Character_{option.characterDefindID}";
                        var rt = character.transform as RectTransform;
                        rt.anchoredPosition = new Vector2(-2000, -1500);
                        rt.sizeDelta = new Vector2(0, 1100);
                        _characterDisplays.Add(character);
                    }

        #if VNG_EXPRESSION
                    character.Output(option.characterImage, option.characterExpression, false, option);
        #else
                    character.Output(option.characterImage, null, false, option);
        #endif
                    // 恢复原始排序逻辑
                    character.transform.SetSiblingIndex(option.ArrangeByListOrder ? order : option.CustomOrder);
                    order++; // 手动递增顺序
                }

                _background.Output(currentAction.backGround);
                AudioManager.Instance.PlayAudio(currentAction.audio, currentAction.bg_Music);
                _currentIndex++;
            }
            else
            {
                foreach (var display in _characterDisplays) display.Interrupt();
                _background.Interrupt();
                if (Dialogue_Configs.InterruptVoicePlayback)
                {
                    AudioManager.Instance.InterruptAudio();
                }
            }
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
        }
    }
}