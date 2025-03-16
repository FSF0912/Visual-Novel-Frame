using FSF.Collection;
using UnityEngine;
using System.Linq;
using System.Collections;
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
        [SerializeField] private List<Character> _characterDisplays =new();


        private int _currentIndex;
        private bool processBranch = false;
        private Stack<int> BranchReturnPoints = new();
        [HideInInspector] public bool AllowInput = true;
        private bool InputReceived
        {
            get
            {
                return AllowInput && (Input.GetMouseButtonDown(0) ||_activationKeys.Any(Input.GetKeyDown));
            }
        }

        private void Start()
        {
            ShowNextDialogue();
        }

        private void Update()
        {
            if (InputReceived)
            {
                ShowNextDialogue();
            }
        }

        public async UniTask ShowNextDialogue()
        {
            if (_profile == null || _currentIndex >= _profile.actions.Length)
            {
                _currentIndex = 0;
            }

            var currentAction = _profile.actions[_currentIndex];
            if (currentAction.isBranch)
            {
                AllowInput = false;
                BranchReturnPoints.Push(currentAction.branchEndIndex);
               
            }
            
            if (TypeWriter.Instance.OutputText(currentAction.name, currentAction.dialogue))
            {
                UpdateCharacter(currentAction);
                _background.Output(currentAction.backGround);
                AudioManager.Instance.PlayAudio(currentAction.audio, currentAction.bg_Music);
                _currentIndex++;
            }
            else
            {
                InterruptCharacterActions();
            }


        }

        private void UpdateCharacter(SingleAction action)
        {
            int order = 0;
            foreach (var option in action.characterOptions)
            {
                
                var character = _characterDisplays.FirstOrDefault(
                    x => x.characterDefindID == option.characterDefindID
                );
                if(character == default)
                {
                    character = InstantiateCharacter(option);
                }

                character.Output(option.characterImage, false, option);
                character.transform.SetSiblingIndex(option.ArrangeByListOrder ? order : option.CustomOrder);
                order++;
            }
        }

        private Character InstantiateCharacter(CharacterOption option)
        {
            var temp = Instantiate(_imageSwitcherPrefab, _charactersHolder);
            temp.name = $"Character_{option.characterDefindID}";
            var rt = temp.transform as RectTransform;
            rt.anchoredPosition = new(-2000, -1500);
            rt.sizeDelta = new(0, 1100);

            var component = temp.GetComponent<Character>();
            component.characterDefindID = option.characterDefindID;
            _characterDisplays.Add(component);
            return component;
        }

        private void InterruptCharacterActions()
        {
            foreach (var display in _characterDisplays)
            {
                display.Interrupt();
            }
            _background.Interrupt();
        }

        private void OnDestroy()
        {
            Debug.Log($"Killed {DOTween.KillAll()} Tweens.");
        }
    }
}