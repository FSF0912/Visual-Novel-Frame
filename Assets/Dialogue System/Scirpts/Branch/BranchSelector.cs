using FSF.Collection;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace FSF.VNG
{
    public class BranchSelector : MonoSingleton<BranchSelector>
    {
        public GameObject baranchOptionPrefab;
        public async UniTask<int> Branch(params BranchOption[] options)
        {
            UniTaskCompletionSource<int> selectionSource = new();
            for (int i = 0; i < options.Length; i++)
            {
                var temp = Instantiate(baranchOptionPrefab, this.transform);
                temp.transform.SetParent(this.transform);
                temp.name = $"BranchOption_{i}";
                var branch = temp.GetComponent<SingleBranch>();
                branch.SetText(options[i].BranchText);
                int index = i;
                branch.button.onClick.AddListener(() =>
                {
                    selectionSource.TrySetResult(index);
                });
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
            int result = await selectionSource.Task;
            DestroyButtons();
            return result;
        }

        private void DestroyButtons()
        {
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
        }

    }
}