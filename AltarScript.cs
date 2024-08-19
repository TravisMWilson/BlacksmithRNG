using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class AltarScript : MonoBehaviour {
    #region Singleton

    public static AltarScript Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion
    #region PublicMethods

    public void CombineMaterials() {
        if (!IsAltarFilled()) return;

        Item material = GetMaterialFromSlot();
        if (material == null || material.IsBestQuality()) return;

        if (TryCombineMaterial(material)) {
            if (UpgradeManager.HasAutoCombine) CombineMaxMaterials();

            UpdateQuantityNeededText(material);
            SoundManager.Instance.PlaySound("Altar_Combine");
        }
    }

    public void CombineMaxMaterials() {
        bool isCombiningMaterials;

        do {
            isCombiningMaterials = false;

            List<Item> materialsToCombine = Inventory.Instance.Items
                .Where(item => item.Type == ItemType.Material)
                .ToList();

            foreach (Item material in materialsToCombine) {
                if (!material.IsBestQuality()) {
                    int maxCombines = GetMaxCombines(material);
                    bool combined = CombineMaterial(material, maxCombines);

                    if (combined) {
                        isCombiningMaterials = true;
                    }
                }
            }
        } while (isCombiningMaterials);
    }

    #endregion
    #region PrivateMethods

    private int GetMaxCombines(Item material) {
        ItemQuality nextQuality = material.Quality + 1;
        float costReduction = 1 - (UpgradeManager.CostReductionLevel / 100.0f);
        int combineCost = GetCombineCost(nextQuality);

        return material.Quantity / combineCost;
    }

    private bool CombineMaterial(Item material, int maxCombines) {
        if (maxCombines > 0) {
            ItemQuality nextQuality = material.Quality + 1;
            int combineCost = GetCombineCost(nextQuality);

            Item newMaterial = new Item(material.Name, material.Rarity * 10, material.Icon, material.Type, nextQuality, maxCombines);

            if (Inventory.Instance.Add(newMaterial)) {
                Item removeMaterial = new Item(material.Name, material.Rarity, material.Icon, material.Type, material.Quality, combineCost * maxCombines);
                Inventory.Instance.Remove(removeMaterial);

                return true;
            }
        }

        return false;
    }

    private bool TryCombineMaterial(Item material) {
        ItemQuality nextQuality = material.Quality + 1;
        float costReduction = 1 - (UpgradeManager.CostReductionLevel / 100.0f);
        int combineCost = GetCombineCost(nextQuality);

        if (material.Quantity >= combineCost) {
            Item newMaterial = new Item(material.Name, material.Rarity * 10, material.Icon, material.Type, nextQuality, 1);

            if (Inventory.Instance.Add(newMaterial)) {
                Item removeMaterial = new Item(material.Name, material.Rarity, material.Icon, material.Type, material.Quality, combineCost);
                Inventory.Instance.Remove(removeMaterial);

                return true;
            } else {
                PopupManager.Instance.ShowPopup("Not enough space in inventory.", PopupType.OK);
            }
        } else {
            PopupManager.Instance.ShowPopup("Not enough materials.", PopupType.OK);
        }

        return false;
    }

    private Item GetMaterialFromSlot() {
        string materialType = Inventory.GetMaterialFromIcon(GameObject.Find("ItemIcon"));
        string materialQuality = Inventory.GetQualityFromFrame(GameObject.Find("ItemFrame"));

        return Inventory.Instance.Items.Find(material => material.Name == materialType && material.Quality == Enum.Parse<ItemQuality>(materialQuality));
    }

    private bool IsAltarFilled() {
        string materialType = Inventory.GetMaterialFromIcon(GameObject.Find("ItemIcon"));

        if (materialType == "Blank Material") {
            PopupManager.Instance.ShowPopup("No materials added yet.", PopupType.OK);
            return false;
        }

        return true;
    }

    private void UpdateQuantityNeededText(Item material) {
        int combineCost = GetCombineCost(material.Quality + 1);
        GameObject.Find("QuantityNeededText").GetComponentInChildren<TextMeshProUGUI>().text = material.Quantity + "/" + combineCost;
    }

    private int GetCombineCost(ItemQuality nextQuality) {
        float costReduction = 1 - (UpgradeManager.CostReductionLevel / 100.0f);
        return Mathf.CeilToInt(100.0f / (int)nextQuality * costReduction);
    }

    #endregion
}
