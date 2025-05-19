using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;



namespace PJT.CETIM
{
    public class PrefabSelectorManager : MonoBehaviour
    {
        public List<Button> prefabButtons;
        public List<GameObject> prefabsToPlace;
        public Color selectedColor = Color.green;
        public Color defaultColor = Color.white;

        public int selectedIndex = -1;
        public GameObject SelectedPrefab { get; private set; }
        private SpatialAnchorManager manager;
        public static PrefabSelectorManager Instance;

        private void Awake()
        {
            Instance = this;
            manager = GetComponent<SpatialAnchorManager>();

            
        }

        public void SelectPrefab(int index)
        {
            if (index < 0 || index >= prefabsToPlace.Count)
                return;

            SelectedPrefab = prefabsToPlace[index];
            selectedIndex = index;
        

            for (int i = 0; i < prefabButtons.Count; i++)
            {
                var button = prefabButtons[i];
                var colors = button.colors;

                if (i == selectedIndex)
                {
                    colors.normalColor = selectedColor;
                    colors.highlightedColor = selectedColor;
                    colors.pressedColor = selectedColor;
                    colors.selectedColor = selectedColor;
                }
                else
                {
                    colors.normalColor = defaultColor;
                    colors.highlightedColor = defaultColor;
                    colors.pressedColor = defaultColor;
                    colors.selectedColor = defaultColor;
                }

                button.colors = colors;
            }

            Debug.Log($"🎯 Prefab sélectionné: {SelectedPrefab.name}");
            Debug.Log($"🎯 selectedIndex: {selectedIndex}");
        }
    }
}
