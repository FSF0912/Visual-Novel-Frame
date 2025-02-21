using FSF.Collection;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace FSF.VNG
{
    [System.Serializable]
    public class CharacterPoolSingle
    {
        public Character Character;
        public Transform transform => Character.transform;
        public bool IsBusy;

        public CharacterPoolSingle(Character character)
        {
            Character = character;
            IsBusy = false;
        }

        public void OutputImage(Sprite target)
        {
            Character.OutputImage(target);
        }

        public void Animate(CharacterOption option)
        {
            Character.Animate(option);
        }
    }
    
    [System.Serializable]
    public class CharacterPool : IEnumerable<CharacterPoolSingle>
    {
        public List<CharacterPoolSingle> singles = new List<CharacterPoolSingle>();
        public CharacterPoolSingle GetFree()
        {
            return singles.FirstOrDefault(single => !single.IsBusy);
        }

        public void Add(Character character)
        {
            singles.Add(new CharacterPoolSingle(character));
        }

        public IEnumerator<CharacterPoolSingle> GetEnumerator()
        {
            foreach (var item in singles)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public CharacterPoolSingle this[int index]
        {
            get{
                return singles[index];
            }

            set{
                singles[index] = value;
            }
        }
    }


    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        [Header("Variables")]
        [SerializeField] private GameObject _imageSwitcherPrefab;
        [SerializeField] private RectTransform _charactersHolder;
        [SerializeField] private Character _background;
        [Header("Settings")]
        [SerializeField] private DialogueProfile _profile;
        [SerializeField] private KeyCode[] _activationKeys = 
        { KeyCode.Space, KeyCode.Return, KeyCode.F };
        [SerializeField] private CharacterPool _characterDisplays =new();


        private int _currentIndex;
        [HideInInspector] public bool AllowInput = true;
        private bool InputReceived
        {
            get
            {
                return AllowInput && (Input.GetMouseButtonDown(0) ||_activationKeys.Any(Input.GetKeyDown));
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
                //return;
                _currentIndex = 0;
            }

            var currentAction = _profile.actions[_currentIndex];
            
            if (TypeWriter.Instance.OutputText(currentAction.name, currentAction.dialogue))
            {
                UpdateCharacter(currentAction);
                _background.OutputImage(currentAction.backGround);
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
                    x => x.Character.characterDefindID == option.characterDefindID
                );
                if(character == default)
                {
                    character = new CharacterPoolSingle(InstantiateCharacter(option));
                }

                character.OutputImage(option.characterImage);
                character.Animate(option);
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

        private async UniTask<Character> InstantiateCharacterAsync(CharacterOption option)
        {
            var Sync = InstantiateAsync(_imageSwitcherPrefab, _charactersHolder);
            await Sync;
            var temp = Sync.Result[0];
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
                display.Character.Interrupt();
            }
            _background.Interrupt();
        }

        private void OnDestroy()
        {
            //Debug.Log($"Killed {DOTween.KillAll()} Tweens.");
            DOTween.Clear();
        }
    }
}