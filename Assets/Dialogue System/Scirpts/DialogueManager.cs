using FSF.Collection;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

namespace FSF.DialogueSystem
{
    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        [Header("Scene Variables")]
        [SerializeField] private GameObject _imageSwitcherPrefab;
        [SerializeField] private AudioSource _characterVoiceSource;
        [SerializeField] private RectTransform _charactersHolder;
        [SerializeField] private Character _background;
        [Header("Settings")]
        [SerializeField] private DialogueProfile _profile;
        [SerializeField] private KeyCode[] _activationKeys = 
        { KeyCode.Space, KeyCode.Return, KeyCode.F };
        private List<Character> _characterDisplays =new();


        private int _currentIndex;
        [HideInInspector] public bool AllowInput = true;
        private bool InputReceived
        {
            get
            {
                return AllowInput && (
            Input.GetMouseButtonDown(0) ||
            _activationKeys.Any(Input.GetKeyDown) //||
            //Input.GetAxis("Mouse ScrollWheel") < 0
            );
            }
        }

        protected override void OnAwake()
        {
            
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

        public void ShowNextDialogue()
        {
            if (_profile == null || _currentIndex >= _profile.actions.Length)
            {
                ResetDialogue();
                //return;
            }

            var currentAction = _profile.actions[_currentIndex];
            
            if (TypeWriter.Instance.OutputText(currentAction.name, currentAction.dialogue))
            {
                UpdateCharacter(currentAction);
                UpdateBackground(currentAction);
                PlayAudio(currentAction);
            }
            else
            {
                InterruptCharacterActions();
            }

            _currentIndex++;
        }

        private void UpdateCharacter(SingleAction action)
        {
            int order = 0;
            foreach (var option in action.imageOptions)
            {
                
                var character = _characterDisplays.FirstOrDefault(
                    x => x.characterDefindID == option.characterDefindID
                );
                if(character == default)
                {
                    character = InstantiateCharacter(option);
                }

                character.OutputImage(option.characterImage);
                character.Animate(option);
                character.transform.SetSiblingIndex(option.ArrangeByListOrder ? order : option.CustomOrder);
                order++;
            }
            if (action.TillingCharacters) 
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_charactersHolder);
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

        private void UpdateBackground(SingleAction action)
        {
            if (action.backGround != null)
            {
                _background.OutputImage(action.backGround);
            }
        }

        private void PlayAudio(SingleAction action)
        {
            if(Dialogue_Configs.InterruptVoicePlayback)
            {
                _characterVoiceSource.Stop();
            }

            _characterVoiceSource.clip = action.audio;
            _characterVoiceSource.Play();
        }

        private void InterruptCharacterActions()
        {
            foreach (var display in _characterDisplays)
            {
                display.Interrupt();
            }
            _background.Interrupt();
        }

        public void ResetDialogue()
        {
            _currentIndex = 0;
        }
    }
}