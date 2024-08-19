using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public static class UpgradeInformation {
    #region PublicVariables

    public static Dictionary<string, string> Descriptions = new Dictionary<string, string> {
        { "Material Storage", "Increase the number of inventory slots dedicated to holding materials. This upgrade allows you to gather and store a larger variety of raw materials, ensuring you have everything you need for crafting without running out of space." },
        { "Part Storage", "Expand your inventory to hold more parts. With this upgrade, you can store a greater number of components used in crafting, making it easier to assemble complex items and keep track of your crafting supplies." },
        { "Sword Storage", "Boost the capacity of your inventory to hold more swords. This upgrade enables you to carry and store a larger arsenal of swords, ensuring you always have the right weapon for any situation and increasing your collection without space constraints." },
        { "Multi-Run", "Increase the number of materials you can gather in a single run. With each upgrade, you'll be able to collect more materials per expedition. This means fewer trips and more resources to work with." },
        { "Lower Combine Cost", "Reduce the cost of combining materials by 1% per upgrade. This upgrade helps you save valuable resources and combine more efficiently, allowing you to craft better items at a fraction of the usual cost." },
        { "Luck", "Boost your luck to find rarer materials. Each upgrade increases your luck. Higher luck means better chances of encountering rare and valuable materials during your runs." },
        { "Faster Runs", "Speed up your runs to gather materials more quickly. Each upgrade decreases the time required for a run. Spend less time waiting and more time crafting with this essential upgrade." },
        { "Auto Run", "Enable automatic runs to gather materials without manual input. This upgrade allows your character to continuously embark on runs, collecting resources while you sit back and relax." },
        { "Auto Run Speed", "Decrease the time between each automatic run. This upgrade ensures that your auto runs happen more frequently, maximizing your resource collection over time." },
        { "Auto Hit", "Enable automatic clicks to gather tokens from the dungeon. This upgrade automates the dungeon token collection process, ensuring a steady supply of tokens with minimal effort." },
        { "Auto Hit Speed", "Decrease the time between each automatic dungeon click. This upgrade speeds up your auto dungeon, allowing for faster token collection and more efficient gameplay." },
        { "Auto Combine", "Automatically combine lower-quality materials into higher-quality ones as soon as you have enough resources. This upgrade streamlines your inventory management by eliminating the need for manual combinations." }
    };

    public static Dictionary<string, int> MaxLevels = new Dictionary<string, int> {
        { "Material Storage", 200 },
        { "Part Storage", 200 },
        { "Sword Storage", 200 },
        { "Multi-Run", 4 },
        { "Lower Combine Cost", 80 },
        { "Luck", 1000000 },
        { "Faster Runs", 3000 },
        { "Auto Run", 1 },
        { "Auto Run Speed", 3000 },
        { "Auto Hit", 1 },
        { "Auto Hit Speed", 3000 },
        { "Auto Combine", 1 }
    };

    #endregion
}

public class UpgradeManager : MonoBehaviour {
    #region PublicVariables

    public static int MaterialStorageLevel { get; private set; }
    public static int PartStorageLevel { get; private set; }
    public static int SwordStorageLevel { get; private set; }
    public static int MultiRunLevel { get; private set; }
    public static int LuckLevel { get; private set; }
    public static int CostReductionLevel { get; private set; }
    public static int FasterRunLevel { get; private set; }
    public static int AutoRunLevel { get; private set; }
    public static int AutoDungeonLevel { get; private set; }

    public static bool HasAutoRun { get; private set; }
    public static bool HasAutoDungeon { get; private set; }
    public static bool HasAutoCombine { get; private set; }

    public bool AutoRunOn { get; private set; } = false;
    public bool AutoDungeonOn { get; private set; } = false;

    public string CurrentUpgrade = string.Empty;

    #endregion
    #region PrivateVariables

    private const int STARTING_MATERIAL_LEVEL = 4;
    private const int STARTING_PART_LEVEL = 2;
    private const int STARTING_SWORD_LEVEL = 1;
    private const int DEFAULT_AUTO_RUN_SPEED = 2000;
    private const int DEFAULT_AUTO_HIT_SPEED = 500;

    private float autoRunTimer = 0;
    private float autoDungeonTimer = 0;

    private double cost;

    #endregion
    #region Singleton

    public static UpgradeManager Instance;

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
        LoadUpgrades();

        if (MaterialStorageLevel == 0) { 
            MaterialStorageLevel = STARTING_MATERIAL_LEVEL;
        }

        if (PartStorageLevel == 0) { 
            PartStorageLevel = STARTING_PART_LEVEL;
        }

        if (SwordStorageLevel == 0) {
            SwordStorageLevel = STARTING_SWORD_LEVEL;
        }

        ColorAutoRunButton();
        ColorAutoDungeonButton();

        autoRunTimer = DEFAULT_AUTO_RUN_SPEED - AutoRunLevel;
        autoDungeonTimer = DEFAULT_AUTO_HIT_SPEED - AutoDungeonLevel;
    }

    void OnDestroy() => SaveUpgrades();

    void Update() {
        AutoRun();
        AutoDungeon();

        /*if (Input.GetKeyDown(KeyCode.C)) {
            MaterialStorageLevel = 200;
            PartStorageLevel = 200;
            SwordStorageLevel = 200;
            MultiRunLevel = 4;
            LuckLevel = 1000000;
            CostReductionLevel = 80;
            FasterRunLevel = 3000;
            AutoRunLevel = 5000;
            AutoDungeonLevel = 5000;
            HasAutoRun = true;
            HasAutoDungeon = true;
        } else if (Input.GetKeyDown(KeyCode.X)) {
            MaterialStorageLevel = 4;
            PartStorageLevel = 2;
            SwordStorageLevel = 1;
            MultiRunLevel = 0;
            LuckLevel = 0;
            CostReductionLevel = 0;
            FasterRunLevel = 0;
            AutoRunLevel = 0;
            AutoDungeonLevel = 0;
            HasAutoRun = false;
            HasAutoDungeon = false;
        }*/
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        ColorAutoRunButton();
        ColorAutoDungeonButton();
    }

    #endregion
    #region PublicMethods

    public void AutoRun() {
        if (SceneManager.GetActiveScene().name != "BlacksmithScene" || !HasAutoRun || !AutoRunOn) return;

        autoRunTimer -= Time.deltaTime * 1000;

        if (autoRunTimer <= 0) {
            autoRunTimer = DEFAULT_AUTO_RUN_SPEED - (AutoRunLevel / (UpgradeInformation.MaxLevels["Auto Run Speed"] / DEFAULT_AUTO_RUN_SPEED));
            if (!WorldManager.Instance.LoadWorld()) AutoRunOn = false;
        }
    }

    public void ToggleAutoRun() {
        if (!HasAutoRun) {
            SoundManager.Instance.PlaySound("UI Alert Error");
            return;
        }

        SoundManager.Instance.PlaySound("Button_Press");
        AutoRunOn = !AutoRunOn;
        ColorAutoRunButton();
    }

    public void ColorAutoRunButton() {
        GameObject autoRun = GameObject.Find("AutoRunButton");
        if (autoRun == null) return;

        Image autoRunImage = autoRun.GetComponent<Image>();
        Button autoRunButton = autoRun.GetComponent<Button>();
        SpriteState spriteState = autoRunButton.spriteState;

        if (AutoRunOn) {
            autoRunImage.sprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Green_Normal");
            spriteState.highlightedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Green_Hovered");
            spriteState.pressedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Green_Pressed");
        } else {
            autoRunImage.sprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Red_Normal");
            spriteState.highlightedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Red_Hovered");
            spriteState.pressedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Red_Pressed");
        }

        autoRunButton.spriteState = spriteState;
    }

    public void AutoDungeon() {
        if (SceneManager.GetActiveScene().name != "DungeonScene" || !HasAutoDungeon || !AutoDungeonOn) return;

        autoDungeonTimer -= Time.deltaTime * 1000;

        if (autoDungeonTimer <= 0) {
            autoDungeonTimer = DEFAULT_AUTO_HIT_SPEED - (AutoDungeonLevel / (UpgradeInformation.MaxLevels["Auto Hit Speed"] / DEFAULT_AUTO_HIT_SPEED));
            DungeonScript.Instance.HitEnemy();
        }
    }

    public void ToggleAutoDungeon() {
        if (!HasAutoDungeon) {
            SoundManager.Instance.PlaySound("UI Alert Error");
            return;
        }

        SoundManager.Instance.PlaySound("Button_Press");
        AutoDungeonOn = !AutoDungeonOn;
        ColorAutoDungeonButton();
    }

    public void ColorAutoDungeonButton() {
        GameObject autoDungeon = GameObject.Find("AutoDungeonButton");
        if (autoDungeon == null) return;

        Image autoDungeonImage = autoDungeon.GetComponent<Image>();
        Button autoDungeonButton = autoDungeon.GetComponent<Button>();
        SpriteState spriteState = autoDungeonButton.spriteState;

        if (AutoDungeonOn) {
            autoDungeonImage.sprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Green_Normal");
            spriteState.highlightedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Green_Hovered");
            spriteState.pressedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Green_Pressed");
        } else {
            autoDungeonImage.sprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Orange_Normal");
            spriteState.highlightedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Orange_Hovered");
            spriteState.pressedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Orange_Pressed");
        }

        autoDungeonButton.spriteState = spriteState;
    }

    public void ShowUpgrade(string upgradeName) {
        SoundManager.Instance.PlaySound("Button_Press");

        TextMeshProUGUI titleText = GameObject.Find("TitleText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = GameObject.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI maxLevelText = GameObject.Find("MaxLevelText").GetComponent<TextMeshProUGUI>();

        titleText.text = upgradeName;
        descriptionText.text = UpgradeInformation.Descriptions[upgradeName];
        maxLevelText.text = "Max Level: " + UtilityScript.FormatNumber(UpgradeInformation.MaxLevels[upgradeName]);

        int increaseUpgradeBy = 1;
        int currentLevel = 0;
        cost = 0;

        var upgradeCalculations = new Dictionary<string, (Func<double>, Func<int>)> {
            { "Material Storage", (() => Math.Pow(MaterialStorageLevel + increaseUpgradeBy, 3) * 150, () => MaterialStorageLevel) },
            { "Part Storage", (() => Math.Pow(PartStorageLevel + increaseUpgradeBy, 3) * 150, () => PartStorageLevel) },
            { "Sword Storage", (() => Math.Pow(SwordStorageLevel + increaseUpgradeBy, 3) * 150, () => SwordStorageLevel) },
            { "Multi-Run", (() => Math.Pow((MultiRunLevel + increaseUpgradeBy) * (40 * (MultiRunLevel + increaseUpgradeBy)), 4), () => MultiRunLevel) },
            { "Lower Combine Cost", (() => Math.Pow(CostReductionLevel + increaseUpgradeBy, 4) * 1500, () => CostReductionLevel) },
            { "Luck", (() => (Math.Pow((LuckLevel + increaseUpgradeBy) / 15, 2.5) + (LuckLevel + increaseUpgradeBy)) * 500, () => LuckLevel) },
            { "Faster Runs", (() => (Math.Pow((FasterRunLevel + increaseUpgradeBy) / 15, 4) + (FasterRunLevel + increaseUpgradeBy)) * 1500, () => FasterRunLevel) },
            { "Auto Run", (() => 700000L, () => HasAutoRun ? 1 : 0) },
            { "Auto Run Speed", (() => (Math.Pow((AutoRunLevel + increaseUpgradeBy) / 15, 4) + (AutoRunLevel + increaseUpgradeBy)) * 1500, () => AutoRunLevel) },
            { "Auto Hit", (() => 500000L, () => HasAutoDungeon ? 1 : 0) },
            { "Auto Hit Speed", (() => (Math.Pow((AutoDungeonLevel + increaseUpgradeBy) / 15, 4) + (AutoDungeonLevel + increaseUpgradeBy)) * 1500, () => AutoDungeonLevel) },
            { "Auto Combine", (() => 1000000000000L, () => HasAutoCombine ? 1 : 0) }
        };

        for (int i = 0; i < getPurchaseAmount(); i++) {
            if (upgradeCalculations.TryGetValue(upgradeName, out var calculations)) {
                cost += calculations.Item1();
            }

            increaseUpgradeBy++;
        }

        if (upgradeCalculations.TryGetValue(upgradeName, out var calculation)) {
            currentLevel = calculation.Item2();

            if (currentLevel + getPurchaseAmount() > UpgradeInformation.MaxLevels[upgradeName]) {
                GameObject upgradeAmountInput = GameObject.Find("UpgradeAmountInput");
                upgradeAmountInput.GetComponent<TMP_InputField>().text = "0";
                PopupManager.Instance.ShowPopup("That amount would exceed the max, try again.", PopupType.OK);
                ShowUpgrade(upgradeName);
            }
        }

        UpdateCurrentAndCost(currentLevel, upgradeName);
    }

    public void PurchaseUpgrade() {
        TextMeshProUGUI upgradeText = GameObject.Find("TitleText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI costText = GameObject.Find("CostText").GetComponent<TextMeshProUGUI>();

        if (string.IsNullOrEmpty(upgradeText.text)) {
            SoundManager.Instance.PlaySound("UI Alert Error");
            PopupManager.Instance.ShowPopup("No upgrade selected. Select an upgrade first.", PopupType.OK);
            return;
        }

        if (string.IsNullOrEmpty(costText.text)) {
            SoundManager.Instance.PlaySound("UI Alert Error");
            PopupManager.Instance.ShowPopup("Upgrade is already maxed.", PopupType.OK);
            return;
        }

        if (!TokenManager.Instance.HasEnoughTokens(cost)) {
            SoundManager.Instance.PlaySound("UI Alert Error");
            PopupManager.Instance.ShowPopup("Not enough tokens to purchase upgrade at current amount.", PopupType.OK);
            return;
        }

        SoundManager.Instance.PlaySound("Button_Press");
        TokenManager.Instance.DecreaseTokens(cost);

        var upgradeActions = new Dictionary<string, Action> {
            { "Material Storage", () => MaterialStorageLevel += getPurchaseAmount() },
            { "Part Storage", () => PartStorageLevel += getPurchaseAmount() },
            { "Sword Storage", () => SwordStorageLevel += getPurchaseAmount() },
            { "Multi-Run", () => MultiRunLevel += getPurchaseAmount() },
            { "Lower Combine Cost", () => CostReductionLevel += getPurchaseAmount() },
            { "Luck", () => LuckLevel += getPurchaseAmount() },
            { "Faster Runs", () => FasterRunLevel += getPurchaseAmount() },
            { "Auto Run", () => HasAutoRun = true },
            { "Auto Run Speed", () => AutoRunLevel += getPurchaseAmount() },
            { "Auto Hit", () => HasAutoDungeon = true },
            { "Auto Hit Speed", () => AutoDungeonLevel += getPurchaseAmount() },
            { "Auto Combine", () => HasAutoCombine = true }
        };

        if (upgradeActions.TryGetValue(upgradeText.text, out Action upgradeAction)) {
            upgradeAction();
            ShowUpgrade(upgradeText.text);
        }

        ShowUpgrade(upgradeText.text);
    }

    public void UpdateCurrentAndCost(int currentLevel, string upgradeName) {
        if (string.IsNullOrEmpty(upgradeName)) return;

        CurrentUpgrade = upgradeName;

        TextMeshProUGUI currentLevelText = GameObject.Find("CurrentLevelText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI costText = GameObject.Find("CostText").GetComponent<TextMeshProUGUI>();

        if (currentLevel == UpgradeInformation.MaxLevels[upgradeName]) {
            currentLevelText.text = "Current Level: Maxed";
            costText.text = string.Empty;
        } else {
            currentLevelText.text = "Current Level: " + currentLevel;
            costText.text = "Cost: " + UtilityScript.FormatNumber(cost);
        }
    }

    #endregion
    #region PrivateMethods

    private int getPurchaseAmount() {
        GameObject upgradeAmountInput = GameObject.Find("UpgradeAmountInput");

        if (upgradeAmountInput != null) {
            string upgradeAmountText = upgradeAmountInput.GetComponent<TMP_InputField>().text;

            if (!string.IsNullOrEmpty(upgradeAmountText)) {
                return int.Parse(upgradeAmountText);
            }
        }

        return 0;
    }

    private void SaveUpgrades() {
        PlayerPrefs.SetInt("MaterialStorageLevel", MaterialStorageLevel);
        PlayerPrefs.SetInt("PartStorageLevel", PartStorageLevel);
        PlayerPrefs.SetInt("SwordStorageLevel", SwordStorageLevel);
        PlayerPrefs.SetInt("MultiRunLevel", MultiRunLevel);
        PlayerPrefs.SetInt("LuckLevel", LuckLevel);
        PlayerPrefs.SetInt("CostReductionLevel", CostReductionLevel);
        PlayerPrefs.SetInt("FasterRunLevel", FasterRunLevel);
        PlayerPrefs.SetInt("AutoRunLevel", AutoRunLevel);
        PlayerPrefs.SetInt("AutoDungeonLevel", AutoDungeonLevel);
        PlayerPrefs.SetInt("HasAutoRun", HasAutoRun ? 1 : 0);
        PlayerPrefs.SetInt("HasAutoDungeon", HasAutoDungeon ? 1 : 0);
        PlayerPrefs.SetInt("AutoRunOn", AutoRunOn ? 1 : 0);
        PlayerPrefs.SetInt("HasAutoCombine", HasAutoCombine ? 1 : 0);
    }

    private void LoadUpgrades() {
        MaterialStorageLevel = PlayerPrefs.GetInt("MaterialStorageLevel", STARTING_MATERIAL_LEVEL);
        PartStorageLevel = PlayerPrefs.GetInt("PartStorageLevel", STARTING_PART_LEVEL);
        SwordStorageLevel = PlayerPrefs.GetInt("SwordStorageLevel", STARTING_SWORD_LEVEL);
        MultiRunLevel = PlayerPrefs.GetInt("MultiRunLevel", 0);
        LuckLevel = PlayerPrefs.GetInt("LuckLevel", 0);
        CostReductionLevel = PlayerPrefs.GetInt("CostReductionLevel", 0);
        FasterRunLevel = PlayerPrefs.GetInt("FasterRunLevel", 0);
        AutoRunLevel = PlayerPrefs.GetInt("AutoRunLevel", 0);
        AutoDungeonLevel = PlayerPrefs.GetInt("AutoDungeonLevel", 0);
        HasAutoRun = PlayerPrefs.GetInt("HasAutoRun", 0) == 1;
        HasAutoDungeon = PlayerPrefs.GetInt("HasAutoDungeon", 0) == 1;
        AutoRunOn = PlayerPrefs.GetInt("AutoRunOn", 0) == 1;
        HasAutoCombine = PlayerPrefs.GetInt("HasAutoCombine", 0) == 1;
    }

    #endregion
}
