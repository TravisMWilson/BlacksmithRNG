using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class FurnaceScript : MonoBehaviour {
    #region PublicVariables

    public GameObject ActiveBlueprint;

    #endregion
    #region PrivateVariables

    private string materialType;
    private string materialQuality;
    private int averageQuality;

    #endregion
    #region Singleton

    public static FurnaceScript Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion
    #region PublicMethods

    public void UpdatePartsRarityTotal() {
        double partRarity = CalculatePartRarity();
        UpdatePowerDisplay(partRarity);
        UpdatePartDisplay();
    }

    public void ShowBlueprint(GameObject blueprint) {
        SoundManager.Instance.PlaySound("Button_Press");
        ClearPartMadeText();

        if (ActiveBlueprint != null) {
            ActiveBlueprint.SetActive(false);
            ResetBlueprintSlots();
        }

        ActiveBlueprint = blueprint;
        ActiveBlueprint.SetActive(true);

        UpdatePartDisplay();
    }

    public void ForgePart() {
        if (ActiveBlueprint == null) {
            PopupManager.Instance.ShowPopup("First select a part to create from the top.", PopupType.OK);
            return;
        }
        
        if (!AreSlotsAllSameMaterial(mutePopup: false)) {
            PopupManager.Instance.ShowPopup("Not all materials are the same.", PopupType.OK);
            return;
        }

        // What type of part is being forged
        string partType = GetPartTypeFromBlueprint();
        string partName = partType + "_" + materialType;
        ItemType partItemType = GetItemType(partType);
        double partRarity = CalculatePartRarity();

        // Create part to add to inventory
        Sprite partSprite = Inventory.GetItemIconByName(partName);
        Item newPart = new Item(partName, partRarity, partSprite, partItemType, Enum.Parse<ItemQuality>(materialQuality));

        if (!TryForgePart(partType, newPart)) return;

        DisplayPartMadeMessage(partType);
        ResetBlueprintSlots();
        UpdatePartDisplay();
        ClearPowerDisplay();
        SoundManager.Instance.PlaySound("Forge_Part");
    }

    #endregion
    #region PrivateMethods

    private bool TryForgePart(string partType, Item newPart) {
        List<Item> materialsToRemove = new List<Item>();
        int requiredQuantity = partType == "Blade" ? 8 : 4;
        int currentQuantity = 0;
        averageQuality = 0;

        foreach (Transform slot in ActiveBlueprint.transform) {
            Item chosenMaterial = slot.GetComponent<SlotItemScript>().selectedItem;
            Item findItem = materialsToRemove.Find(material => material.Name == chosenMaterial.Name && material.Rarity == chosenMaterial.Rarity && material.Quality == chosenMaterial.Quality);

            if (findItem != null) {
                if (chosenMaterial.Quantity >= findItem.Quantity + 1) {
                    currentQuantity++;
                    averageQuality += (int)findItem.Quality;
                    findItem.Quantity++;
                } else {
                    PopupManager.Instance.ShowPopup("Not enough materials.", PopupType.OK);
                    return false;
                }
            } else {
                currentQuantity++;
                averageQuality += (int)chosenMaterial.Quality;

                Item removeMaterial = new Item(chosenMaterial.Name, chosenMaterial.Rarity, chosenMaterial.Icon, chosenMaterial.Type, chosenMaterial.Quality, 1);
                materialsToRemove.Add(removeMaterial);
            }
        }

        if (currentQuantity < requiredQuantity) {
            PopupManager.Instance.ShowPopup("Not enough materials.", PopupType.OK);
            return false;
        }

        newPart.Quality = (ItemQuality)(averageQuality / requiredQuantity);

        if (Inventory.Instance.Add(newPart)) {
            foreach (Item material in materialsToRemove) {
                Inventory.Instance.Remove(material);
            }

            return true;
        } else {
            PopupManager.Instance.ShowPopup("Not enough space in inventory.", PopupType.OK);
            return false;
        }
    }

    private void ResetBlueprintSlots() {
        foreach (Transform slot in ActiveBlueprint.transform) {
            slot.Find("ItemIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Materials/Blank Material");
            slot.Find("ItemFrame").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Frames/ItemFrame_Normal");
        }
    }

    private ItemType GetItemType(string partType) {
        return partType switch {
            "Blade" => ItemType.Blade,
            "Guard" => ItemType.Guard,
            "Handle" => ItemType.Handle,
            "Pommel" => ItemType.Pommel,
            _ => 0
        };
    }

    private double CalculatePartRarity() {
        double partRarity = 0;

        foreach (Transform slot in ActiveBlueprint.transform) {
            if (!slot.Find("ItemIcon").GetComponent<Image>().sprite.ToString().Contains("Blank")) {
                double slotRarity = slot.GetComponent<SlotItemScript>().selectedItem.Rarity;
                partRarity += slotRarity;
            }
        }

        return partRarity;
    }

    private void UpdatePowerDisplay(double partRarity)
        => GameObject.Find("PowerDisplayText").GetComponentInChildren<TextMeshProUGUI>().text = "Power - " + UtilityScript.FormatNumber(partRarity);

    private void ClearPartMadeText()
        => GameObject.Find("PartMadeText").GetComponent<TextMeshProUGUI>().text = string.Empty;

    private void DisplayPartMadeMessage(string partType)
        => GameObject.Find("PartMadeText").GetComponent<TextMeshProUGUI>().text = "You just made a " + materialType + " " + partType;

    private void ClearPowerDisplay()
        => GameObject.Find("PowerDisplayText").GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;

    private string GetPartTypeFromBlueprint()
        => ActiveBlueprint.name.Replace("Blueprint", "");

    private bool AreSlotsAllSameMaterial(bool mutePopup) {
        List<string> materialTypes = new List<string>();
        List<string> materialQualities = new List<string>();
        List<double> materialRarities = new List<double>();

        foreach (Transform slot in ActiveBlueprint.transform) {
            materialTypes.Add(slot.Find("ItemIcon").GetComponent<Image>().sprite.ToString());
            materialQualities.Add(slot.Find("ItemFrame").GetComponent<Image>().sprite.ToString());

            if (slot.GetComponent<SlotItemScript>().selectedItem == null) {
                if (!mutePopup) PopupManager.Instance.ShowPopup("Missing a material in a slot.", PopupType.OK);
                return false;
            }

            materialRarities.Add(slot.GetComponent<SlotItemScript>().selectedItem.Rarity);
        }

        if (materialTypes.Distinct().Count() > 1) {
            if (!mutePopup) PopupManager.Instance.ShowPopup("There are more than one type of material in the slots.", PopupType.OK);
            return false;
        }

        materialType = materialTypes[0].Replace(" (UnityEngine.Sprite)", "");
        materialQuality = materialQualities[0].Replace("ItemFrame_", "").Replace(" (UnityEngine.Sprite)", "");

        if (materialType == "Blank Material") {
            if (!mutePopup) PopupManager.Instance.ShowPopup("No materials added yet.", PopupType.OK);
            return false;
        }

        return true;
    }

    private void UpdatePartDisplay() {
        Image partDisplay = GameObject.Find("PartDisplayImage").GetComponent<Image>();
        Image partDisplayShadow = GameObject.Find("PartDisplayImageShadow").GetComponent<Image>();

        if (AreSlotsAllSameMaterial(mutePopup: true)) {
            string partType = ActiveBlueprint.name.Replace("Blueprint", "");
            partDisplay.sprite = Inventory.GetItemIconByName(partType + "_" + materialType);
            partDisplayShadow.sprite = Inventory.GetItemIconByName(partType + "_" + materialType);
        } else {
            partDisplay.sprite = Resources.Load<Sprite>("Sprites/AnvilDefaults/Empty");
            partDisplayShadow.sprite = Resources.Load<Sprite>("Sprites/AnvilDefaults/Empty");
        }
    }

    #endregion
}
