using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public GameObject PrefabsMenu;
    public GameObject ScenesMenu;
    public GameObject panelAide;

    private void Start()
    {
        ShowPrefabsMenu();
    }
    public void ShowPrefabsMenu()
    {
        PrefabsMenu.SetActive(true);
        ScenesMenu.SetActive(false);
        panelAide.SetActive(false);
    }

    public void ShowScenesMenu()
    {
        PrefabsMenu.SetActive(false);
        ScenesMenu.SetActive(true);
        panelAide.SetActive(false);
    }

    public void ShowAide()
    {
        PrefabsMenu.SetActive(false);
        ScenesMenu.SetActive(false);
        panelAide.SetActive(true);
    }
}
