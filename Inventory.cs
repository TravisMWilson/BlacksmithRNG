using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

public class Inventory : MonoBehaviour {
    #region PublicVariables

    public GameObject SlotPrefab;
    public GameObject SwordSlotPrefab;
    public GameObject InventoryUI;
    public Transform MaterialsParent;
    public Transform PartsParent;
    public Transform SwordsParent;

    public Item EquippedSword = null;

    public int AutoDeleteMax = 0;

    public List<Item> Items = new List<Item>();

    #endregion
    #region PrivateVariables

    private Transform inventoryContainer;

    private int materialSpace;
    private int partSpace;
    private int swordSpace;
    private int currentMaterialCount;
    private int currentPartCount;
    private int currentSwordCount;

    private readonly List<GameObject> spawnedSlots = new List<GameObject>();

    #endregion
    #region Singleton

    public static Inventory Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #endregion
    #region LifecycleMethods

    void Start() {
        materialSpace = LoadScript.LoadIntValue("MaterialStorageLevel") * 5;
        partSpace = LoadScript.LoadIntValue("PartStorageLevel") * 5;
        swordSpace = LoadScript.LoadIntValue("SwordStorageLevel") * 5;
        AutoDeleteMax = LoadScript.LoadIntValue("AutoDeleteMax");

        string strInventory = LoadScript.LoadStringValue("Inventory");
        string strSwordInventory = LoadScript.LoadStringValue("SwordInventory");
        string strEquippedSwordInventory = LoadScript.LoadStringValue("EquippedSword");

        LoadItemsFromString(strInventory, false);
        LoadItemsFromString(strSwordInventory, true);

        if (!string.IsNullOrEmpty(strEquippedSwordInventory)) {
            Item newSword = DeserializeSword(strEquippedSwordInventory);
            EquippedSword = Items.Find(sword => sword.Name == newSword.Name && sword.Type == newSword.Type && sword.Quality == newSword.Quality);

            if (EquippedSword == null) {
                Debug.Log("Couldn't find equipped sword from save.");
            } else {
                if (GameObject.Find("DisplaySword") != null && SceneManager.GetActiveScene().name != "AnvilScene") {
                    DisplaySword.Instance.UpdateSword(EquippedSword);
                }
            }
        }

        foreach (Item item in Items) {
            if (item.Type == ItemType.Material) {
                MaterialRarity material = MaterialsScript.Instance.Materials.Find(m => m.Name == item.Name && m.Quality == item.Quality);
                item.Rarity = material.Rarity;
            }
        }
    }

    void OnDestroy() {
        SaveItemsToString();
        SaveAutoDeleteMax();
    }

    /*void Update() {
        if (Input.GetKeyDown(KeyCode.X)) {
            Items.Clear();
            spawnedSlots.Clear();
            EquippedSword = null;
        }
    }*/

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        UpdateAutoDeleteMax();

        if (GameObject.Find("DisplaySword") != null && scene.name != "AnvilScene" && EquippedSword != null) {
            DisplaySword.Instance.UpdateSword(EquippedSword);
        }

        InventoryUI = GameObject.Find("Inventory");
        if (InventoryUI == null) return;

        MaterialsParent = GameObject.Find("MaterialsParent").transform;
        PartsParent = GameObject.Find("PartsParent").transform;
        SwordsParent = GameObject.Find("SwordsParent").transform;

        MaterialsParent.gameObject.SetActive(true);
        PartsParent.gameObject.SetActive(false);
        SwordsParent.gameObject.SetActive(false);

        if (scene.name == "AnvilScene") {
            MaterialsParent.gameObject.SetActive(false);
            PartsParent.gameObject.SetActive(true);
        }

        GetItemCounts();
        UpdateStorageText("Material");

        if (scene.name == "BlacksmithScene") {
            CloseInventory();
        }

        spawnedSlots.Clear();

        foreach (Item item in Items) {
            SetInventoryContainer(item.Type);
            AddNewItemToInventory(item);
        }
    }

    #endregion
    #region StaticMethods

    public static string GetMaterialFromIcon(GameObject icon)
        => icon.GetComponent<Image>().sprite.ToString().Replace(" (UnityEngine.Sprite)", "");

    public static string GetOnlyMaterialFromIcon(GameObject icon) {
        string spriteName = icon.GetComponent<Image>().sprite.ToString().Replace(" (UnityEngine.Sprite)", "");

        int underscoreIndex = spriteName.IndexOf('_');

        if (underscoreIndex != -1) {
            spriteName = spriteName[(underscoreIndex + 1)..];
        }

        return spriteName;
    }

    public static string GetQualityFromFrame(GameObject frame) {
        Image imageComponent = frame.GetComponent<Image>();

        return imageComponent.sprite.ToString().Contains("Sword")
            ? imageComponent != null ? imageComponent.sprite.ToString().Replace("SwordFrame_", "").Replace(" (UnityEngine.Sprite)", "") : string.Empty
            : imageComponent != null ? imageComponent.sprite.ToString().Replace("ItemFrame_", "").Replace(" (UnityEngine.Sprite)", "") : string.Empty;
    }

    public static Sprite GetItemIconByName(string itemName) {
        string path = itemName.Contains("Pommel") ? "Sprites/Parts/Pommels/" :
                      itemName.Contains("Handle") ? "Sprites/Parts/Handles/" :
                      itemName.Contains("Guard") ? "Sprites/Parts/Guards/" :
                      itemName.Contains("Blade") ? "Sprites/Parts/Blades/" : "Sprites/Materials/";

        return Resources.Load<Sprite>(path + itemName);
    }

    public static Sprite GetSlotIconByQuality(ItemQuality quality) {
        string path = "Sprites/Frames/ItemFrame_" + quality;
        return Resources.Load<Sprite>(path);
    }

    public static Sprite GetSwordSlotIconByQuality(ItemQuality quality) {
        string path = "Sprites/Frames/SwordFrame_" + quality;
        return Resources.Load<Sprite>(path);
    }

    #endregion
    #region PublicMethods

    public void UpdateAutoDeleteMax() {
        GameObject autoDelete = GameObject.Find("AutoDeleteText");

        if (autoDelete != null) {
            autoDelete.GetComponent<TMP_InputField>().text = AutoDeleteMax.ToString();
        }
    }

    public void SaveAutoDeleteMax() {
        GameObject autoDelete = GameObject.Find("AutoDeleteText");

        if (autoDelete != null) {
            string text = autoDelete.GetComponent<TMP_InputField>().text;

            if (!string.IsNullOrEmpty(text)) {
                AutoDeleteMax = int.Parse(autoDelete.GetComponent<TMP_InputField>().text);
                SaveScript.SaveIntValue("AutoDeleteMax", AutoDeleteMax);
            }
        }
    }

    public bool Add(Item item) {
        bool inWorld = SceneManager.GetActiveScene().name.Contains("World");

        SetInventoryContainer(item.Type);

        if (item.Rarity <= AutoDeleteMax && item.Type == ItemType.Material) {
            if (!inWorld) PopupManager.Instance.ShowPopup("Item has been auto-deleted.", PopupType.OK);
            return false;
        }

        if (TryAddExistingItem(item)) return true;

        if (IsInventoryFull(item.Type)) {
            if (!inWorld) PopupManager.Instance.ShowPopup("Not enough room for a new item.", PopupType.OK);
            return false;
        }

        Items.Add(item);
        UpdateStorageText(item.Type.ToString());
        AddNewItemToInventory(item);

        return true;
    }

    public void Remove(Item item) {
        // Check if item is already existing and if it is decrease the quantity
        Item inventoryItem = Items.FirstOrDefault(it => it.Name == item.Name && it.Type == item.Type && it.Quality == item.Quality);
        if (inventoryItem == null) return;

        GameObject slotButton = inventoryItem.Slot.transform.Find("ItemButton").gameObject;

        inventoryItem.Quantity -= item.Quantity;

        if (item.Type != ItemType.Sword) {
            TextMeshProUGUI slotQuantity = slotButton.transform.Find("ItemQuantity").gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI slotRarity = slotButton.transform.Find("ItemRarity").gameObject.GetComponent<TextMeshProUGUI>();

            slotQuantity.text = inventoryItem.Quantity.ToString();
            slotRarity.text = "1/" + UtilityScript.FormatNumber(item.Rarity);
        }

        // Remove the item from the inventory if none left
        if (inventoryItem.Quantity <= 0) {
            _ = spawnedSlots.Remove(inventoryItem.Slot);
            Destroy(inventoryItem.Slot);
            _ = Items.Remove(inventoryItem);
            UpdateStorageText(item.Type.ToString());
        }
    }

    public void ShowTab(GameObject tab) {
        MaterialsParent.gameObject.SetActive(false);
        PartsParent.gameObject.SetActive(false);
        SwordsParent.gameObject.SetActive(false);

        tab.SetActive(true);
        UpdateStorageText(tab.name);

        GameObject.Find("Scroll View").GetComponent<ScrollRect>().content = tab.GetComponent<RectTransform>();
    }

    public void OpenInventory() {
        SoundManager.Instance.PlaySound("Button_Press");
        SoundManager.Instance.PlaySound("Backpack_Open");
        InventoryUI.SetActive(true);
        GetItemCounts();
        UpdateStorageText("Material");
    }

    public void CloseInventory() {
        SoundManager.Instance.PlaySound("Button_Press");
        SoundManager.Instance.PlaySound("Backpack_Close");
        InventoryUI.SetActive(false);
    }

    public bool HasSwordEquipped()
        => EquippedSword != null;

    public bool IsInventoryFull(ItemType itemType) {
        GetItemCounts();

        return (itemType == ItemType.Material && currentMaterialCount >= materialSpace) ||
               ((itemType == ItemType.Pommel || itemType == ItemType.Handle || itemType == ItemType.Guard || itemType == ItemType.Blade) && currentPartCount >= partSpace) ||
               (itemType == ItemType.Sword && currentSwordCount >= swordSpace);
    }

    public int SpaceLeftInInventory(ItemType itemType) {
        GetItemCounts();

        return itemType == ItemType.Material ? currentMaterialCount :
               itemType == ItemType.Sword ? currentSwordCount : currentPartCount;
    }

    public void SortInventory(string sortBy) {
        var sortCriteria = new Dictionary<string, Comparison<Item>> {
            { "Ascending Name", (a, b) => a.Name.CompareTo(b.Name) },
            { "Descending Name", (a, b) => b.Name.CompareTo(a.Name) },
            { "Ascending Quality", (a, b) => a.Quality.CompareTo(b.Quality) },
            { "Descending Quality", (a, b) => b.Quality.CompareTo(a.Quality) },
            { "Ascending Rarity", (a, b) => a.Rarity.CompareTo(b.Rarity) },
            { "Descending Rarity", (a, b) => b.Rarity.CompareTo(a.Rarity) },
            { "Ascending Quantity", (a, b) => a.Quantity.CompareTo(b.Quantity) },
            { "Descending Quantity", (a, b) => b.Quantity.CompareTo(a.Quantity) }
        };

        if (sortCriteria.TryGetValue(sortBy, out Comparison<Item> comparison)) {
            spawnedSlots.Sort((a, b) => comparison(a.GetComponent<InventorySlot>().SlotItem, b.GetComponent<InventorySlot>().SlotItem));
            Items.Sort((a, b) => comparison(a, b));

            for (int i = 0; i < spawnedSlots.Count; i++) {
                spawnedSlots[i].GetComponent<RectTransform>().SetSiblingIndex(i);
            }
        } else {
            Debug.LogWarning("Sort criteria not found: " + sortBy);
        }
    }

    #endregion
    #region PrivateMethods

    private void LoadItemsFromString(string itemsString, bool isSword) {
        while (!string.IsNullOrEmpty(itemsString)) {
            int index = itemsString.IndexOf(",");
            string nextItem = itemsString[..index];
            itemsString = itemsString[(index + 1)..];

            Item newItem = isSword ? DeserializeSword(nextItem) : DeserializeItem(nextItem);
            _ = Add(newItem);
        }
    }

    private Item DeserializeItem(string itemString) {
        string[] itemInfo = itemString.Split(':');

        string itemName = itemInfo[0];
        double itemRarity = double.Parse(itemInfo[1]);
        Sprite itemIcon = GetItemIconByName(itemName);
        ItemType itemType = Enum.Parse<ItemType>(itemInfo[2]);
        ItemQuality itemQuality = Enum.Parse<ItemQuality>(itemInfo[3]);
        int itemQuantity = int.Parse(itemInfo[4]);

        return new Item(itemName, itemRarity, itemIcon, itemType, itemQuality, itemQuantity);
    }

    private string SerializeSword(Item sword) {
        return sword.Name + ":" + sword.Rarity + ":" + sword.Type + ":" + sword.Quality + ":" + sword.Quantity + ":" +
            sword.SwordItem.Pommel.Name + ":" + sword.SwordItem.Pommel.Rarity + ":" + sword.SwordItem.Pommel.Type + ":" + sword.SwordItem.Pommel.Quality + ":" + sword.SwordItem.Pommel.Quantity + ":" +
            sword.SwordItem.Handle.Name + ":" + sword.SwordItem.Handle.Rarity + ":" + sword.SwordItem.Handle.Type + ":" + sword.SwordItem.Handle.Quality + ":" + sword.SwordItem.Handle.Quantity + ":" +
            sword.SwordItem.Guard.Name + ":" + sword.SwordItem.Guard.Rarity + ":" + sword.SwordItem.Guard.Type + ":" + sword.SwordItem.Guard.Quality + ":" + sword.SwordItem.Guard.Quantity + ":" +
            sword.SwordItem.Blade.Name + ":" + sword.SwordItem.Blade.Rarity + ":" + sword.SwordItem.Blade.Type + ":" + sword.SwordItem.Blade.Quality + ":" + sword.SwordItem.Blade.Quantity;
    }

    private Item DeserializeSword(string swordSave) {
        string[] itemInfo = swordSave.Split(':');

        string itemName = itemInfo[0];
        ItemType itemType = Enum.Parse<ItemType>(itemInfo[2]);
        ItemQuality itemQuality = Enum.Parse<ItemQuality>(itemInfo[3]);
        int itemQuantity = int.Parse(itemInfo[4]);

        Item pommel = new Item(itemInfo[5], double.Parse(itemInfo[6]), GetItemIconByName(itemInfo[5]), Enum.Parse<ItemType>(itemInfo[7]), Enum.Parse<ItemQuality>(itemInfo[8]), int.Parse(itemInfo[9]));
        Item handle = new Item(itemInfo[10], double.Parse(itemInfo[11]), GetItemIconByName(itemInfo[10]), Enum.Parse<ItemType>(itemInfo[12]), Enum.Parse<ItemQuality>(itemInfo[13]), int.Parse(itemInfo[14]));
        Item guard = new Item(itemInfo[15], double.Parse(itemInfo[16]), GetItemIconByName(itemInfo[15]), Enum.Parse<ItemType>(itemInfo[17]), Enum.Parse<ItemQuality>(itemInfo[18]), int.Parse(itemInfo[19]));
        Item blade = new Item(itemInfo[20], double.Parse(itemInfo[21]), GetItemIconByName(itemInfo[20]), Enum.Parse<ItemType>(itemInfo[22]), Enum.Parse<ItemQuality>(itemInfo[23]), int.Parse(itemInfo[24]));
        Sword newSword = new Sword(pommel, handle, guard, blade);

        double itemRarity = newSword.Power();

        Item newItem = new Item(itemName, itemRarity, null, itemType, itemQuality, itemQuantity) {
            SwordItem = newSword
        };

        return newItem;
    }

    private void SaveItemsToString() {
        string save = string.Empty;
        string swordSave = string.Empty;

        foreach (Item item in Items) {
            if (item.Type != ItemType.Sword) {
                save += item.Name + ":" + item.Rarity + ":" + item.Type + ":" + item.Quality + ":" + item.Quantity + ",";
            } else {
                swordSave += SerializeSword(item) + ",";
            }
        }

        SaveScript.SaveStringValue("Inventory", save);
        SaveScript.SaveStringValue("SwordInventory", swordSave);

        if (EquippedSword != null) {
            SaveScript.SaveStringValue("EquippedSword", SerializeSword(EquippedSword));
        } else {
            SaveScript.SaveStringValue("EquippedSword", string.Empty);
        }
    }

    private void SetInventoryContainer(ItemType itemType) {
        inventoryContainer = itemType == ItemType.Material ? MaterialsParent : 
            itemType == ItemType.Sword ? SwordsParent : PartsParent;
    }

    private bool TryAddExistingItem(Item item) {
        Item inventoryItem = Items.Find(it => it.Name == item.Name && it.Type == item.Type && it.Quality == item.Quality && item.Type != ItemType.Sword);
        if (inventoryItem == null) return false;

        inventoryItem.Quantity += item.Quantity;

        if (inventoryItem.Slot != null) {
            GameObject slotButton = inventoryItem.Slot.transform.Find("ItemButton").gameObject;
            TextMeshProUGUI slotQuantity = slotButton.transform.Find("ItemQuantity").gameObject.GetComponent<TextMeshProUGUI>();
            slotQuantity.text = inventoryItem.Quantity.ToString();
        }

        return true;
    }

    private void AddNewItemToInventory(Item item) {
        GameObject slot;
        GameObject slotIcon;
        GameObject iconPommelImage;
        GameObject iconHandleImage;
        GameObject iconGuardImage;
        GameObject iconBladeImage;
        GameObject itemPommelName;
        GameObject itemHandleName;
        GameObject itemGuardName;
        GameObject itemBladeName;
        TextMeshProUGUI slotQuantity;
        TextMeshProUGUI slotRarity;

        slot = item.Type == ItemType.Sword ? Instantiate(SwordSlotPrefab, inventoryContainer) : Instantiate(SlotPrefab, inventoryContainer);

        GameObject slotButton = slot.transform.Find("ItemButton").gameObject;
        slotIcon = slotButton.transform.Find("ItemIcon").gameObject;

        TextMeshProUGUI slotName = slotButton.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>();
        slotName.text = item.Name;

        if (item.Type == ItemType.Sword) {
            iconPommelImage = slotIcon.transform.Find("PommelImage").gameObject;
            iconHandleImage = slotIcon.transform.Find("HandleImage").gameObject;
            iconGuardImage = slotIcon.transform.Find("GuardImage").gameObject;
            iconBladeImage = slotIcon.transform.Find("BladeImage").gameObject;

            iconPommelImage.GetComponent<Image>().sprite = item.SwordItem.Pommel.Icon;
            iconHandleImage.GetComponent<Image>().sprite = item.SwordItem.Handle.Icon;
            iconGuardImage.GetComponent<Image>().sprite = item.SwordItem.Guard.Icon;
            iconBladeImage.GetComponent<Image>().sprite = item.SwordItem.Blade.Icon;

            DisplaySword.Instance.CacheDisplayObjects(slotIcon);
            DisplaySword.Instance.RepositionDisplaySword();

            itemPommelName = slotButton.transform.Find("ItemPommel").gameObject;
            itemHandleName = slotButton.transform.Find("ItemHandle").gameObject;
            itemGuardName = slotButton.transform.Find("ItemGuard").gameObject;
            itemBladeName = slotButton.transform.Find("ItemBlade").gameObject;

            itemPommelName.GetComponent<TextMeshProUGUI>().text = item.SwordItem.Pommel.Name;
            itemHandleName.GetComponent<TextMeshProUGUI>().text = item.SwordItem.Handle.Name;
            itemGuardName.GetComponent<TextMeshProUGUI>().text = item.SwordItem.Guard.Name;
            itemBladeName.GetComponent<TextMeshProUGUI>().text = item.SwordItem.Blade.Name;

            slotRarity = slotButton.transform.Find("ItemRarity").gameObject.GetComponent<TextMeshProUGUI>();
            slotRarity.text = UtilityScript.FormatNumber(item.SwordItem.Power());
            slotButton.GetComponent<Image>().sprite = GetSwordSlotIconByQuality(item.Quality);
        } else {
            slotQuantity = slotButton.transform.Find("ItemQuantity").gameObject.GetComponent<TextMeshProUGUI>();
            slotRarity = slotButton.transform.Find("ItemRarity").gameObject.GetComponent<TextMeshProUGUI>();

            slotButton.GetComponent<Image>().sprite = GetSlotIconByQuality(item.Quality);
            slotIcon.GetComponent<Image>().sprite = GetItemIconByName(item.Name);

            slotRarity.text = "1/" + UtilityScript.FormatNumber(item.Rarity);
            slotQuantity.text = item.Quantity.ToString();
        }

        slot.GetComponent<InventorySlot>().SlotItem = item;
        item.Slot = slot;
        spawnedSlots.Add(slot);
    }

    private void GetItemCounts() {
        if (materialSpace < UpgradeManager.MaterialStorageLevel * 5) {
            materialSpace = UpgradeManager.MaterialStorageLevel * 5;
        }

        if (partSpace < UpgradeManager.PartStorageLevel * 5) {
            partSpace = UpgradeManager.PartStorageLevel * 5;
        }

        if (swordSpace < UpgradeManager.SwordStorageLevel * 5) {
            swordSpace = UpgradeManager.SwordStorageLevel * 5;
        }

        currentMaterialCount = Items.Count(item => item.Type is ItemType.Material);
        currentPartCount = Items.Count(item => item.Type is ItemType.Pommel or ItemType.Handle or ItemType.Guard or ItemType.Blade);
        currentSwordCount = Items.Count(item => item.Type is ItemType.Sword);
    }

    private void UpdateStorageText(string tab) {
        GameObject storageLabel = GameObject.Find("StorageText");
        if (storageLabel == null) return;

        TextMeshProUGUI storageText = storageLabel.GetComponent<TextMeshProUGUI>();

        storageText.text = tab.Contains("Material") ? currentMaterialCount + "/" + materialSpace :
            tab.Contains("Part") ? currentPartCount + "/" + partSpace :
            tab.Contains("Sword") ? currentSwordCount + "/" + swordSpace : string.Empty;
    }

    #endregion
}
