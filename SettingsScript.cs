using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SettingsScript : MonoBehaviour {
    #region PublicVariables

    public Button DeleteButton;
    public bool InDeleteMode = false;
    public List<GameObject> removeSlots = new List<GameObject>();

    public GameObject SettingsMenu;

    #endregion
    #region PrivateVariables

    private bool isSortAcending = true;
    private string sortedByLast;

    #endregion
    #region Singleton

    public static SettingsScript Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion
    #region LifecycleMethods

    void Start() => DeleteButton = GameObject.Find("DeleteButton").GetComponent<Button>();

    #endregion
    #region PublicMethods

    public void DeleteItemsFromInventory() {
        SoundManager.Instance.PlaySound("Button_Press");

        if (!InDeleteMode) {
            InDeleteMode = true;

            ColorBlock colorButton = DeleteButton.colors;
            colorButton.normalColor = new Color(0.85f, 0f, 0f);
            colorButton.selectedColor = new Color(0.85f, 0f, 0f);
            DeleteButton.colors = colorButton;
        } else {
            foreach (GameObject slot in removeSlots) {
                Item item = slot.GetComponent<InventorySlot>().SlotItem;
                Inventory.Instance.Remove(item);
            }

            removeSlots.Clear();
            InDeleteMode = false;

            ColorBlock colorButton = DeleteButton.colors;
            colorButton.normalColor = new Color(1f, 1f, 1f);
            colorButton.selectedColor = new Color(1f, 1f, 1f);
            DeleteButton.colors = colorButton;
        }
    }

    public void SortInventoryBy(string sortBy) {
        SoundManager.Instance.PlaySound("Button_Press");

        if (sortedByLast == sortBy) {
            isSortAcending = !isSortAcending;
        }

        if (!isSortAcending) {
            sortedByLast = sortBy;
            Inventory.Instance.SortInventory("Ascending " + sortBy);
        } else {
            sortedByLast = sortBy;
            Inventory.Instance.SortInventory("Descending " + sortBy);
        }
    }

    public void OpenSettings() {
        SoundManager.Instance.PlaySound("Button_Press");
        SettingsMenu.SetActive(true);
    }

    public void CloseSettings() {
        SoundManager.Instance.PlaySound("Button_Press");
        SettingsMenu.SetActive(false);
    }

    #endregion
}
