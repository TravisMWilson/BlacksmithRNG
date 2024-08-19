using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    #region PublicVariables

    public GameObject DraggablePrefab;
    public Transform DraggableContainer;
    public Item SlotItem;

    #endregion
    #region PrivateVariables

    private GameObject draggable;
    private RectTransform rectTransform;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private PointerEventData savedEventData;

    private bool destroy = true;

    #endregion
    #region LifecycleMethods

    void Start() {
        if (GameObject.Find("Canvas") != null) {
            DraggableContainer = GameObject.Find("Canvas").transform;
            raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        }
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            destroy = false;
            OnEndDrag(savedEventData);
            destroy = true;
        }
    }

    #endregion
    #region PublicMethods

    public void OnBeginDrag(PointerEventData eventData) {
        if (SceneManager.GetActiveScene().name == "BlacksmithScene") return;

        draggable = Instantiate(DraggablePrefab, DraggableContainer);
        draggable.GetComponent<Image>().sprite = transform.Find("ItemButton").Find("ItemIcon").GetComponent<Image>().sprite;
        draggable.transform.Find("ItemFrame").GetComponent<Image>().sprite = transform.Find("ItemButton").GetComponent<Image>().sprite;
        rectTransform = draggable.GetComponent<RectTransform>();

        GameObject partMadeText = GameObject.Find("PartMadeText");

        if (partMadeText != null) {
            partMadeText.GetComponent<TextMeshProUGUI>().text = string.Empty;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (rectTransform == null) return;
        
        rectTransform.anchoredPosition = eventData.position - new Vector2(512, 288);
        savedEventData = new PointerEventData(EventSystem.current) {
            position = eventData.position
        };
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (eventData == null || draggable == null) return;
        pointerEventData = new PointerEventData(EventSystem.current) {
            position = eventData.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        if (results.Count > 0) HandleDragDropResults(results);
        if (destroy) Destroy(draggable);
    }

    public void EquipSword() {
        if (!Inventory.Instance.SwordsParent.gameObject.activeSelf) return;

        SoundManager.Instance.PlaySound("Button_Press");
        Inventory.Instance.EquippedSword = SlotItem;
        DisplaySword.Instance.UpdateSword(SlotItem);
    }

    public void ToggleDelete() {
        SoundManager.Instance.PlaySound("Button_Press");
        Color buttonColor = transform.Find("ItemButton").Find("ItemIcon").GetComponent<Image>().color;

        if (SettingsScript.Instance.InDeleteMode) {
            if (buttonColor == new Color(0.45f, 0f, 0f)) {
                RemoveSlotFromRemovalList();
            } else {
                AddSlotToRemove();
            }
        }
    }

    #endregion
    #region PrivateMethods

    private void HandleDragDropResults(List<RaycastResult> results) {
        foreach (RaycastResult result in results) {
            if (result.gameObject.name.Contains("Slot")) {
                HandleSlotDrop(result.gameObject);
            }
        }
    }

    private void HandleSlotDrop(GameObject slot) {
        switch (SceneManager.GetActiveScene().name) {
            case "FurnaceScene":
                SoundManager.Instance.PlaySound("Place_Material_In_Forge");
                SwapSlotImages(slot);
                FurnaceScript.Instance.UpdatePartsRarityTotal();
                break;
            case "AnvilScene":
                string slotMaterial = Inventory.GetMaterialFromIcon(slot.transform.Find("ItemIcon").gameObject);

                if (slotMaterial.Contains(SlotItem.Name[..5])) {
                    SoundManager.Instance.PlaySound("Place_Part_In_Anvil");
                    SwapSlotImages(slot);
                    HandleAnvilScene();
                }

                break;
            case "AltarScene":
                SoundManager.Instance.PlaySound("Altar_Place");
                SwapSlotImages(slot);
                HandleAltarScene();
                break;
            default:
                break;
        }
    }

    private void SwapSlotImages(GameObject slot) {
        GameObject draggableFrame = draggable.transform.Find("ItemFrame").gameObject;

        slot.GetComponent<SlotItemScript>().selectedItem = SlotItem;
        slot.transform.Find("ItemIcon").GetComponent<Image>().sprite = draggable.GetComponent<Image>().sprite;
        slot.transform.Find("ItemFrame").GetComponent<Image>().sprite = draggableFrame.GetComponent<Image>().sprite;
    }

    private void HandleAnvilScene() {
        string[] part = SlotItem.Name.Split('_');

        GameObject displayPart = GameObject.Find(part[0] + "Image");
        displayPart.GetComponent<Image>().sprite = draggable.GetComponent<Image>().sprite;

        GameObject displayPartShadow = GameObject.Find(part[0] + "Shadow");
        displayPartShadow.GetComponent<Image>().sprite = draggable.GetComponent<Image>().sprite;

        GameObject sword = GameObject.Find("DisplaySword");
        DisplaySword.Instance.CacheDisplayObjects(sword);
        DisplaySword.Instance.RepositionDisplaySword();

        string swordRarity = UtilityScript.FormatNumber(CalculateSwordPower());
        GameObject.Find("PowerDisplayText").GetComponentInChildren<TextMeshProUGUI>().text = "Power - " + swordRarity;
    }

    private void HandleAltarScene() {
        float costReduction = 1 - (UpgradeManager.CostReductionLevel / 100.0f);
        int combineCost = Mathf.CeilToInt(100.0f / ((int)SlotItem.Quality + 1) * costReduction);

        GameObject.Find("QuantityNeededText").GetComponentInChildren<TextMeshProUGUI>().text = SlotItem.Quantity + "/" + combineCost;
    }

    private double CalculateSwordPower() {
        double swordRarity = 0;

        foreach (GameObject obj in FindObjectsOfType<GameObject>()) {
            if (obj.name == "SlotItem") {
                Item slottedItem = obj.GetComponent<SlotItemScript>().selectedItem;

                if (slottedItem != null) {
                    swordRarity += slottedItem.Rarity;
                }
            }
        }

        return swordRarity;
    }

    private void RemoveSlotFromRemovalList() {
        GameObject itemButton = transform.Find("ItemButton").gameObject;
        Button slotButton = itemButton.GetComponent<Button>();
        Image slotIcon = itemButton.transform.Find("ItemIcon").GetComponent<Image>();

        ColorBlock colorButton = slotButton.colors;

        colorButton.normalColor = new Color(1f, 1f, 1f);
        colorButton.selectedColor = new Color(1f, 1f, 1f);
        slotIcon.color = new Color(1f, 1f, 1f);

        slotButton.colors = colorButton;

        if (gameObject.name.Contains("Sword")) {
            foreach (Transform part in gameObject.transform) {
                part.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            }
        }

        _ = SettingsScript.Instance.removeSlots.Remove(gameObject);
    }

    private void AddSlotToRemove() {
        SettingsScript.Instance.removeSlots.Add(gameObject);

        GameObject itemButton = transform.Find("ItemButton").gameObject;
        Button slotButton = itemButton.GetComponent<Button>();
        Image slotIcon = itemButton.transform.Find("ItemIcon").GetComponent<Image>();

        ColorBlock colorButton = slotButton.colors;

        colorButton.normalColor = new Color(0.45f, 0f, 0f);
        colorButton.selectedColor = new Color(0.45f, 0f, 0f);
        slotIcon.color = new Color(0.45f, 0f, 0f);

        slotButton.colors = colorButton;

        if (gameObject.name.Contains("Sword")) {
            foreach (Transform part in gameObject.transform) {
                part.gameObject.GetComponent<Image>().color = new Color(0.45f, 0f, 0f);
            }
        }
    }

    #endregion
}
