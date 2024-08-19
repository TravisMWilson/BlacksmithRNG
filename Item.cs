using UnityEngine;

public enum ItemQuality {
    Normal = 0,
    Unique = 1,
    Rare = 2,
    Epic = 3,
    Supreme = 4,
    Legendary = 5,
    Mythical = 6,
    Immortal = 7,
    Transcendent = 8,
    Divine = 9,
    Celestial = 10,
    Ascended = 11,
    Eternal = 12
}

public enum ItemType {
    Material = 0,
    Pommel = 1,
    Handle = 2,
    Guard = 3,
    Blade = 4,
    Sword = 5
}

public class Item {
    #region Properties

    public string Name { get; set; }
    public double Rarity { get; set; }
    public Sprite Icon { get; set; }
    public ItemType Type { get; set; }
    public ItemQuality Quality { get; set; }
    public int Quantity { get; set; }
    public Sword SwordItem { get; set; }

    #endregion
    #region PublicVariables

    public GameObject Slot;

    #endregion
    #region Constructor

    public Item(string name, double rarity, Sprite icon, ItemType type, ItemQuality quality = ItemQuality.Normal, int quantity = 1) {
        Name = name;
        Rarity = rarity;
        Icon = icon;
        Type = type;
        Quality = quality;
        Quantity = quantity;
    }

    #endregion
    #region PublicMethods

    public bool IsBestQuality() {
        if (Quality == ItemQuality.Eternal) {
            PopupManager.Instance.ShowPopup("Already the best quality.", PopupType.OK);
            return true;
        }

        return false;
    }

    #endregion
}
