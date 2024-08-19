using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;
using System;
using TMPro;

public class MaterialRarity {
    #region Properties

    public string Name { get; set; }
    public double Rarity { get; set; }
    public ItemQuality Quality { get; set; }

    #endregion
    #region Constructor

    public MaterialRarity(string materialName, double rarity, ItemQuality quality = ItemQuality.Normal) {
        Name = materialName;
        Rarity = rarity;
        Quality = quality;
    }

    #endregion
}

public class MaterialRarityList {
    #region PublicMethods

    public static List<MaterialRarity> GetMaterials() {
        List<MaterialRarity> materials = new List<MaterialRarity> {
            new MaterialRarity("Clay", 2),
            new MaterialRarity("Chalk", 3),
            new MaterialRarity("Limestone", 5),
            new MaterialRarity("Tin", 6),
            new MaterialRarity("Sandstone", 8),
            new MaterialRarity("Flint", 12),
            new MaterialRarity("Slate", 18),
            new MaterialRarity("Granite", 25),
            new MaterialRarity("Lead", 31),
            new MaterialRarity("Marble", 35),
            new MaterialRarity("Zinc", 41),
            new MaterialRarity("Basalt", 51),
            new MaterialRarity("Quartz", 71),
            new MaterialRarity("Obsidian", 101),
            new MaterialRarity("Copper", 151),
            new MaterialRarity("Ironwood", 221),
            new MaterialRarity("Aluminum", 301),
            new MaterialRarity("Pewter", 401),
            new MaterialRarity("Nickel", 451),
            new MaterialRarity("Brass Alloy", 551),
            new MaterialRarity("Iron", 651),
            new MaterialRarity("Steel", 751),
            new MaterialRarity("Titanium", 901),
            new MaterialRarity("Carbon Steel", 1001),
            new MaterialRarity("Tungsten", 1301),
            new MaterialRarity("Bronze Alloy", 1501),
            new MaterialRarity("Silver", 2001),
            new MaterialRarity("Cobalt", 2501),
            new MaterialRarity("Gold", 3001),
            new MaterialRarity("Iridium", 4501),
            new MaterialRarity("Platinum", 5001),
            new MaterialRarity("Mithril", 7001),
            new MaterialRarity("Mossy Iron", 9001),
            new MaterialRarity("Adamantine", 10001),
            new MaterialRarity("Orichalcum", 12001),
            new MaterialRarity("Runestone", 14001),
            new MaterialRarity("Electrum", 17001),
            new MaterialRarity("Dragonbone", 20001),
            new MaterialRarity("Forest Steel", 25001),
            new MaterialRarity("Elven Steel", 30001),
            new MaterialRarity("Moonsteel", 35001),
            new MaterialRarity("Darksteel", 45001),
            new MaterialRarity("Star Metal", 70001),
            new MaterialRarity("Moonstone", 100001),
            new MaterialRarity("Bloodstone", 150001),
            new MaterialRarity("Shadowglass", 220001),
            new MaterialRarity("Sunsilver", 300001),
            new MaterialRarity("Aetherium", 450001),
            new MaterialRarity("Void Iron", 500001),
            new MaterialRarity("Voidstone", 600001),
            new MaterialRarity("Thunderite", 800001),
            new MaterialRarity("Lifestone", 1000001),
            new MaterialRarity("Froststeel", 1500001),
            new MaterialRarity("Fireheart", 2000001),
            new MaterialRarity("Stormshard", 3000001),
            new MaterialRarity("Mysticite", 5000001),
            new MaterialRarity("Spectral", 7500001),
            new MaterialRarity("Eternal Ice", 10000001),
            new MaterialRarity("Phoenix Feather", 20000001),
            new MaterialRarity("Necrotite", 30000001),
            new MaterialRarity("Soulstone", 40000001),
            new MaterialRarity("Elderglow", 50000001),
            new MaterialRarity("Nightsilver", 70000001),
            new MaterialRarity("Celestium", 100000001),
            new MaterialRarity("Manasteel", 150000001),
            new MaterialRarity("Twilight", 200000001),
            new MaterialRarity("Netherite", 300000001),
            new MaterialRarity("Arcane Crystal", 400000001)/*,
            new MaterialRarity("Voidcrystal", 500000001),
            new MaterialRarity("Starfire", 700000001),
            new MaterialRarity("Prismatic Shard", 900000001),
            new MaterialRarity("Dreamstone", 1000000001),
            new MaterialRarity("Astralite", 1500000001),
            new MaterialRarity("Leyline", 1750000001),
            new MaterialRarity("Etherite", 1900000001),
            new MaterialRarity("Radiant Gem", 2000000001),
            new MaterialRarity("Eldritch", 3000000001),
            new MaterialRarity("Chronostone", 5000000001),
            new MaterialRarity("Galactic", 7000000001),
            new MaterialRarity("Divinium", 9000000001),
            new MaterialRarity("Omnimetal", 12000000001)*/
        };

        AddNextQualityMaterials(materials);

        materials = materials.OrderBy(m => m.Rarity).ToList();

        return materials;
    }

    #endregion
    #region PrivateMethods

    private static void AddNextQualityMaterials(List<MaterialRarity> materials) {
        List<MaterialRarity> newMaterials = new List<MaterialRarity>();

        for (int i = 1; i <= 12; i++) {
            foreach (MaterialRarity material in materials) {
                double increasedRarity = material.Rarity * Math.Pow(10, i);
                newMaterials.Add(new MaterialRarity(material.Name, increasedRarity, (ItemQuality)i));
            }
        }

        materials.AddRange(newMaterials);
    }

    #endregion
}

public class MaterialsScript : MonoBehaviour {
    #region PublicVariables

    public RectTransform Panel;
    public GameObject MaterialTilePrefab;

    public List<MaterialRarity> Materials;

    #endregion
    #region PrivateVariables

    private static readonly System.Random random = new System.Random();

    private bool isMaterialChosen = false;
    private bool playedJingle = false;
    private float closeDelay;
    private double totalWeight;
    private double maxRarity;

    #endregion
    #region Singleton

    public static MaterialsScript Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Materials = MaterialRarityList.GetMaterials();
    }

    #endregion
    #region LifecycleMethods

    void Update() {
        if (isMaterialChosen) {
            closeDelay -= Time.deltaTime * 1000;

            if (closeDelay <= 0) {
                ChangeScene.Instance.GoToScene("BlacksmithScene");
                isMaterialChosen = false;
            }
        }

        /*if (Input.GetKeyDown(KeyCode.M)) {
            foreach (MaterialRarity material in materials) {
                Item newItem = new Item(material.Name, material.Rarity, Inventory.GetItemIconByName(material.Name), ItemType.Material, material.Quality, 10);
                Inventory.Instance.Add(newItem);
            }
        }*/
    }

    #endregion
    #region PublicMethods

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name.Contains("World")) {
            Panel = GameObject.Find("RollBar").GetComponent<RectTransform>();
            RollForMaterial();
        }
    }

    public void RollForMaterial() {
        for (int i = 0; i <= WorldManager.Instance.ActiveMultiRuns; i++) {
            _ = StartCoroutine(ScrollMaterials(-70 * i));
        }

        closeDelay = 1000 * (1.25f - ((UpgradeManager.FasterRunLevel + 1) / UpgradeInformation.MaxLevels["Faster Runs"]));
    }

    #endregion
    #region PrivateMethods

    private void GetTotalWeight() {
        int currentWorld = int.Parse(WorldManager.CurrentWorld.SceneName.Replace("World", "").Replace("Scene", ""));
        maxRarity = 1;

        for (int i = 1; i <= currentWorld; i++) {
            maxRarity = i * 50 * maxRarity;
        }

        totalWeight = 0;

        foreach (var material in Materials) {
            double weight = material.Rarity;
            totalWeight += weight / 2 < maxRarity ? weight : weight * 100;
        }
    }

    private IEnumerator ScrollMaterials(int offset) {
        List<GameObject> spawnedTiles = new List<GameObject>();

        float scrollSpeed = 3000f;
        float spawnPosX = 0f;

        Panel.anchoredPosition = new Vector2(-1200, -165);
        spawnedTiles.Clear();

        GetTotalWeight();
        SpawnMaterialTile(spawnPosX, spawnedTiles, offset);

        while (scrollSpeed > 0) {
            while (spawnPosX <= (Panel.anchoredPosition.x * -1) + 1000) {
                spawnPosX += 70.0f;
                SpawnMaterialTile(spawnPosX, spawnedTiles, offset);
            }

            Panel.anchoredPosition -= new Vector2(scrollSpeed * Time.deltaTime, 0);
            RemoveOffScreenTiles(spawnedTiles);
            scrollSpeed -= Time.deltaTime * (300.0f + (UpgradeManager.FasterRunLevel * 5));

            yield return null;
        }

        MaterialRarity chosenMaterial = GetChosenMaterial(spawnedTiles);

        Item materialItem = new Item(chosenMaterial.Name, chosenMaterial.Rarity, null, ItemType.Material, chosenMaterial.Quality);
        bool addedItemSuccessfully = Inventory.Instance.Add(materialItem);

        GameObject chosenMaterialTextObject = GameObject.Find("ChosenMaterialText");
        TextMeshProUGUI chosenMaterialText = chosenMaterialTextObject.GetComponent<TextMeshProUGUI>();

        if (addedItemSuccessfully) {
            chosenMaterialText.text += "You got " + chosenMaterial.Quality + " " + chosenMaterial.Name + "\n";
        } else {
            if (chosenMaterial.Rarity <= Inventory.Instance.AutoDeleteMax) {
                chosenMaterialText.text += chosenMaterial.Quality + " " + chosenMaterial.Name + " was auto-deleted\n";
            }

            if (Inventory.Instance.IsInventoryFull(ItemType.Material)) {
                chosenMaterialText.text += "Inventory too full for " + chosenMaterial.Quality + " " + chosenMaterial.Name + "\n";
            }
        }

        if (!playedJingle) {
            playedJingle = true;
            SoundManager.Instance.PlaySound("Happy_Jingle");
        }

        isMaterialChosen = true;
    }

    private void SpawnMaterialTile(float posX, List<GameObject> spawnedTiles, int offset) {
        MaterialRarity randomMaterial = GetRandomMaterial();

        GameObject tile = Instantiate(MaterialTilePrefab, Panel);
        Image itemIcon = tile.transform.Find("ItemIcon").gameObject.GetComponent<Image>();
        Image itemFrame = tile.transform.Find("ItemFrame").gameObject.GetComponent<Image>();
        TextMeshProUGUI itemRarity = tile.transform.Find("ItemRarity").gameObject.GetComponent<TextMeshProUGUI>();

        tile.GetComponent<TextMeshProUGUI>().text = randomMaterial.Name;
        itemIcon.sprite = Inventory.GetItemIconByName(randomMaterial.Name);
        itemFrame.sprite = Inventory.GetSlotIconByQuality(randomMaterial.Quality);
        itemRarity.text = "1/" + UtilityScript.FormatNumber(randomMaterial.Rarity);
        tile.GetComponent<SlotItemScript>().selectedMaterial = randomMaterial;

        RectTransform tileRect = tile.GetComponent<RectTransform>();
        tileRect.anchoredPosition = new Vector2(posX, offset);

        spawnedTiles.Add(tile);
    }

    private MaterialRarity GetChosenMaterial(List<GameObject> spawnedTiles) {
        float centerPosX = (Panel.anchoredPosition.x * -1) - 717;

        foreach (var tile in spawnedTiles) {
            RectTransform tileRect = tile.GetComponent<RectTransform>();

            float tilePosXMin = tileRect.anchoredPosition.x - (tileRect.sizeDelta.x / 2);
            float tilePosXMax = tileRect.anchoredPosition.x + (tileRect.sizeDelta.x / 2);

            if (tilePosXMin < centerPosX && tilePosXMax > centerPosX) {
                MaterialRarity material = tile.GetComponent<SlotItemScript>().selectedMaterial;
                return Materials.Find(m => m.Name == material.Name && m.Quality == material.Quality && m.Rarity == material.Rarity);
            }
        }

        return Materials[0];
    }

    private void RemoveOffScreenTiles(List<GameObject> spawnedTiles) {
        List<GameObject> tilesToRemove = new List<GameObject>();

        foreach (var tile in spawnedTiles) {
            RectTransform tileRect = tile.GetComponent<RectTransform>();
            Vector3 tileWorldPos = tileRect.TransformPoint(new Vector3(tileRect.rect.width, 0, 0));

            if (tileWorldPos.x < -10) {
                tilesToRemove.Add(tile);
            }
        }

        foreach (var tile in tilesToRemove) {
            _ = spawnedTiles.Remove(tile);
            Destroy(tile);
        }
    }

    private MaterialRarity GetRandomMaterial() {
        int currentWorld = int.Parse(WorldManager.CurrentWorld.SceneName.Replace("World", "").Replace("Scene", ""));
        double luckLevel = ((UpgradeManager.LuckLevel / 10.0d) + 1) * currentWorld;
        double randomValue = random.NextDouble(0, totalWeight);
        Debug.Log("randomValue: " + randomValue);
        double probability = ((randomValue / totalWeight) / luckLevel) + (((1 - ((UpgradeManager.LuckLevel / UpgradeInformation.MaxLevels["Luck"]) / 2)) * (randomValue / totalWeight)) / (currentWorld * 10));
        Debug.Log("probability: " + probability);

        foreach (var material in Materials) {
            if ((1.0d / material.Rarity) / 2 > maxRarity) {
                if ((1.0d / material.Rarity) / 100 < probability) {
                    return material;
                }
            } else if ((1.0d / material.Rarity) < probability) {
                return material;
            }
        }

        return Materials[^1];
    }

    #endregion
}
