using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class AnvilScript : MonoBehaviour {
    #region PrivateVariable

    private const int DEFAULT_POMMEL_START_POSITION = 5;
    private const int DEFAULT_HANDLE_START_POSITION = 15;
    private const int DEFAULT_HANDLE_END_POSITION = 20;
    private const int DEFAULT_GUARD_START_POSITION = 8;
    private const int DEFAULT_GUARD_END_POSITION = 1;
    private const int DEFAULT_BLADE_START_POSITION = 24;

    private GameObject pommelSlotIcon;
    private GameObject pommelSlotFrame;
    private GameObject handleSlotIcon;
    private GameObject handleSlotFrame;
    private GameObject guardSlotIcon;
    private GameObject guardSlotFrame;
    private GameObject bladeSlotIcon;
    private GameObject bladeSlotFrame;

    private Item swordPommel;
    private Item swordHandle;
    private Item swordGuard;
    private Item swordBlade;

    private string swordName;

    #endregion
    #region Singleton

    public static AnvilScript Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion
    #region PublicMethods

    public void CraftSword() {
        if (!IsSwordNamed() || !AreSlotsFilled() || !HasSpaceInSwordInventory()) return;

        List<Item> itemsToRemove = new List<Item>();

        itemsToRemove.Add(swordPommel);
        itemsToRemove.Add(swordHandle);
        itemsToRemove.Add(swordGuard);
        itemsToRemove.Add(swordBlade);

        foreach (Item item in itemsToRemove) {
            Inventory.Instance.Remove(item);
        }

        Sword newSword = new Sword(swordPommel, swordHandle, swordGuard, swordBlade);
        Item newItem = new Item(swordName, newSword.Power(), null, ItemType.Sword, newSword.Quality()) {
            SwordItem = newSword
        };

        _ = Inventory.Instance.Add(newItem);

        ReturnAnvilSlotsToDefault();
        ClearPowerDisplay();
        SoundManager.Instance.PlaySound("Craft_Sword");
    }

    #endregion
    #region PrivateMethods

    private bool AreSlotsFilled() {
        if (!SetMaterialInfo(ref pommelSlotIcon, ref pommelSlotFrame, "PommelBackground", out swordPommel)) {
            PopupManager.Instance.ShowPopup("Select a pommel first.", PopupType.OK);
            return false;
        }

        if (!SetMaterialInfo(ref handleSlotIcon, ref handleSlotFrame, "HandleBackground", out swordHandle)) {
            PopupManager.Instance.ShowPopup("Select a handle first.", PopupType.OK);
            return false;
        }

        if (!SetMaterialInfo(ref guardSlotIcon, ref guardSlotFrame, "GuardBackground", out swordGuard)) {
            PopupManager.Instance.ShowPopup("Select a guard first.", PopupType.OK);
            return false;
        }

        if (!SetMaterialInfo(ref bladeSlotIcon, ref bladeSlotFrame, "BladeBackground", out swordBlade)) {
            PopupManager.Instance.ShowPopup("Select a blade first.", PopupType.OK);
            return false;
        }

        return true;
    }

    private bool SetMaterialInfo(ref GameObject slotIcon, ref GameObject slotFrame, string backgroundName, out Item material) {
        GameObject block = GameObject.Find(backgroundName);
        GameObject slot = block.transform.Find("SlotItem").gameObject;
        slotIcon = slot.transform.Find("ItemIcon").gameObject;
        slotFrame = slot.transform.Find("ItemFrame").gameObject;

        if (Inventory.GetMaterialFromIcon(slotIcon).Contains("Anvil")) {
            material = null;
            return false;
        }

        material = slot.GetComponent<SlotItemScript>().selectedItem;
        return true;
    }

    private void ReturnAnvilSlotsToDefault() {
        if (pommelSlotIcon == null) return;

        ResetSlot(pommelSlotIcon, "Pommel", pommelSlotFrame);
        ResetSlot(handleSlotIcon, "Handle", handleSlotFrame);
        ResetSlot(guardSlotIcon, "Guard", guardSlotFrame);
        ResetSlot(bladeSlotIcon, "Blade", bladeSlotFrame);

        ResetDisplayImage("Pommel", (DEFAULT_POMMEL_START_POSITION + DEFAULT_HANDLE_START_POSITION + DEFAULT_HANDLE_END_POSITION + DEFAULT_GUARD_START_POSITION) * -2);
        ResetDisplayImage("Handle", (DEFAULT_HANDLE_END_POSITION + DEFAULT_GUARD_START_POSITION) * -2);
        ResetDisplayImage("Guard", 0);
        ResetDisplayImage("Blade", (DEFAULT_BLADE_START_POSITION + DEFAULT_GUARD_END_POSITION) * 2);
    }

    private void ClearPowerDisplay()
        => GameObject.Find("PowerDisplayText").GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;

    private void ResetSlot(GameObject slotIcon, string partName, GameObject slotFrame) {
        slotIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AnvilDefaults/Anvil" + partName + "Image");
        slotFrame.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Frames/ItemFrame_Normal");
    }

    private void ResetDisplayImage(string partName, int position) {
        GameObject imageDisplay = GameObject.Find(partName + "Image");
        imageDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AnvilDefaults/Anvil" + partName + "Image");
        imageDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, position);

        GameObject shadowDisplay = GameObject.Find(partName + "Shadow");
        shadowDisplay.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/AnvilDefaults/Anvil" + partName + "Image");
        shadowDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector2(4.4f, position * 1.2f);
    }

    private bool HasSpaceInSwordInventory() {
        int currentSwordCount = 0;

        foreach (Item item in Inventory.Instance.Items) {
            if (item.Type == ItemType.Sword) {
                currentSwordCount++;
            }
        }

        if (currentSwordCount == UpgradeManager.SwordStorageLevel * 5) {
            PopupManager.Instance.ShowPopup("Not enough space in inventory.", PopupType.OK);
            return false;
        }

        return true;
    }

    private bool IsSwordNamed() {
        swordName = GameObject.Find("SwordNameInput").GetComponent<TextMeshProUGUI>().text[..^1];

        if (swordName.Contains("Cheat.")) {
            string[] cheat = swordName.Split('.');

            if (swordName.Contains("Material")) {
                MaterialRarity chosenMaterial = MaterialsScript.Instance.Materials.Find(material => material.Name == cheat[2] && material.Quality == Enum.Parse<ItemQuality>(cheat[3]));
                Item materialItem = new Item(chosenMaterial.Name, chosenMaterial.Rarity, null, ItemType.Material, chosenMaterial.Quality, int.Parse(cheat[4]));
                _ = Inventory.Instance.Add(materialItem);
            } else if (swordName.Contains("Money")) {
                TokenManager.Instance.IncreaseTokens(double.Parse(cheat[2]));
            }
            
            return false;
        }

        if (Inventory.Instance.Items.Find(item => item.Name == swordName) != null) {
            PopupManager.Instance.ShowPopup("There is already a sword with that name.", PopupType.OK);
            return false;
        }

        if (swordName == "​") {
            PopupManager.Instance.ShowPopup("Sword hasn't been named yet.", PopupType.OK);
            return false;
        }

        return true;
    }

    #endregion
}
