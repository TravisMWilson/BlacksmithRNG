using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class World {
    #region Properties

    public string Name { get; set; }
    public double TokenCost { get; set; }
    public string SceneName { get; set; }

    #endregion
    #region Constructor

    public World(string name, double tokenCost, string scene) {
        Name = name;
        TokenCost = tokenCost;
        SceneName = scene;
    }

    #endregion
}

public class WorldList {
    #region PublicMethods

    public static List<World> GetWorlds() {
        return new List<World> {
            new World("Grassy Plains", 5, "World1Scene"),
            new World("Whispering Woods", 2500, "World2Scene"),
            new World("Stony Ridge", 375000, "World3Scene"),
            new World("Amber Hills", 75000000, "World4Scene"),
            new World("Volcanic Crater", 18750000000, "World5Scene"),
            new World("Shadow Gorge", 5625000000000, "World6Scene")/*,
            new World("Crystal Cave", 1968750000000000, "World7Scene"),
            new World("Starfall Cliffs", 787500000000000000, "World8Scene"),
            new World("Eternal Citadel", 354375000000000000000, "World9Scene")*/
        };
    }

    #endregion
}

public class WorldManager : MonoBehaviour {
    #region PublicVariables

    public static World CurrentWorld;
    public int ActiveMultiRuns = 0;

    #endregion
    #region PrivateVariables

    private static List<World> worldList;
    private bool runAnywaysChoice;

    #endregion
    #region Singleton

    public static WorldManager Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        worldList = WorldList.GetWorlds();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #endregion
    #region LifecycleMethods

    void Start() {
        LoadSavedWorld();
        UpdateMapCosts();

        runAnywaysChoice = LoadScript.LoadIntValue("runAnywaysChoice") == 1;
    }

    void OnDestroy() {
        SaveScript.SaveStringValue("CurrentWorld", CurrentWorld.Name);
        SaveScript.SaveIntValue("runAnywaysChoice", runAnywaysChoice ? 1 : 0);
        SaveScript.SaveStringValue("TitleBackground", CurrentWorld.SceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        UpdateMapCosts();
        UpdateWorldText();
        ActivateMultiRun(ActiveMultiRuns);
    }

    #endregion
    #region PublicMethods

    public void ActivateMultiRun(int number) {
        if (UpgradeManager.MultiRunLevel < number) return;

        ActiveMultiRuns = number;

        for (int i = 0; i <= UpgradeInformation.MaxLevels["Multi-Run"]; i++) {
            if (i == number) {
                ColorMultiRunButton(GameObject.Find("Multirun" + i), true);
            } else {
                ColorMultiRunButton(GameObject.Find("Multirun" + i), false);
            }
        }

        UpdateMapCosts();
    }

    public void ColorMultiRunButton(GameObject run, bool turnOn) {
        if (run == null) return;

        Image runImage = run.GetComponent<Image>();
        Button runButton = run.GetComponent<Button>();
        SpriteState spriteState = runButton.spriteState;

        if (turnOn) {
            runImage.sprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Green_Normal");
            spriteState.highlightedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Green_Hovered");
            spriteState.pressedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Green_Pressed");
        } else {
            runImage.sprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Red_Normal");
            spriteState.highlightedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Red_Hovered");
            spriteState.pressedSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Red_Pressed");
        }

        runButton.spriteState = spriteState;
    }

    public void SelectWorld(string worldName) {
        World newWorld = worldList.Find(w => w.Name == worldName);
        double totalCost = newWorld.TokenCost * (ActiveMultiRuns + 1);

        if (newWorld != null && TokenManager.Instance.HasEnoughTokens(totalCost)) {
            CurrentWorld = newWorld;
            UpdateWorldText();
            SoundManager.Instance.PlaySound("Select_World");
        } else {
            SoundManager.Instance.PlaySound("UI Alert Error");
            PopupManager.Instance.ShowPopup("You need more tokens!", PopupType.OK);

            GetAffordablWorld();
        }
    }

    private void GetAffordablWorld() {
        World newWorld = null;
        double totalCost;

        for (int i = 5; i >= 0; i--) {
            newWorld = worldList[i];
            totalCost = newWorld.TokenCost * (ActiveMultiRuns + 1);

            if (TokenManager.Instance.HasEnoughTokens(totalCost)) break;
        }

        CurrentWorld = newWorld;
        UpdateWorldText();
    }

    private void LoadSavedWorld() {
        string savedWorld = LoadScript.LoadStringValue("CurrentWorld");

        if (string.IsNullOrEmpty(savedWorld)) {
            savedWorld = worldList[0].Name;
            SaveScript.SaveStringValue("CurrentWorld", savedWorld);
        } else {
            CurrentWorld = worldList.Find(w => w.Name == savedWorld);
        }

        UpdateWorldText();
    }

    public bool LoadWorld() {
        double totalCost = CurrentWorld.TokenCost * (ActiveMultiRuns + 1);

        if (Inventory.Instance.IsInventoryFull(ItemType.Material) && !runAnywaysChoice) {
            PopupManager.Instance.ShowPopup("You have no space for new items. Continue to get existing?", PopupType.YesNo, yesCallback: () => runAnywaysChoice = true);

            if (!runAnywaysChoice) return false;
        }

        if (!TokenManager.Instance.HasEnoughTokens(totalCost)) {
            GetAffordablWorld();

            totalCost = CurrentWorld.TokenCost * (ActiveMultiRuns + 1);
        }

        if (TokenManager.Instance.HasEnoughTokens(totalCost)) {
            TokenManager.Instance.DecreaseTokens(totalCost);
            ChangeScene.Instance.GoToScene(CurrentWorld.SceneName);

            return true;
        } else {
            PopupManager.Instance.ShowPopup("You need more tokens!", PopupType.OK);
        }

        return false;
    }

    public void UpdateMapCosts() {
        if (SceneManager.GetActiveScene().name == "MapScene") {
            foreach (Transform pin in GameObject.Find("Canvas").transform) {
                if (pin.name.Contains("Pin")) {
                    TextMeshProUGUI pinText = pin.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();

                    string[] pinLines = pinText.text.Split('\n');
                    string worldName = pinLines[0].Trim();

                    World world = worldList.Find(w => w.Name == worldName);

                    double newCost = world.TokenCost * (ActiveMultiRuns + 1);
                    pinText.text = world.Name + "\n" + UtilityScript.FormatNumber(newCost);
                }
            }
        }
    }

    #endregion
    #region PrivateMethods

    private void UpdateWorldText() {
        GameObject worldTextObject = GameObject.Find("WorldText");
        if (worldTextObject == null) return;

        CurrentWorld ??= worldList[0];

        if (worldTextObject.TryGetComponent<TextMeshProUGUI>(out var worldText)) {
            worldText.SetText(CurrentWorld.Name + " - " + UtilityScript.FormatNumber(CurrentWorld.TokenCost * (ActiveMultiRuns + 1)));
        }
    }

    #endregion
}
