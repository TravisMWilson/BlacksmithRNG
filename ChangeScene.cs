using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChangeScene : MonoBehaviour {
    #region Singleton

    public static ChangeScene Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion
    #region PublicMethods

    public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);

        var travelSound = new Dictionary<string, Action> {
            { "DungeonScene", () => SoundManager.Instance.PlaySound("Dungeon_Stairs") },
            { "AnvilScene", () => SoundManager.Instance.PlaySound("Hammer_Hit") },
            { "AltarScene", () => SoundManager.Instance.PlaySound("Go_To_Altar") },
            { "FurnaceScene", () => SoundManager.Instance.PlaySound("Fire_Crackling1") },
            { "MapScene", () => SoundManager.Instance.PlaySound("Map_Rustle") },
            { "UpgradeScene", () => SoundManager.Instance.PlaySound("Go_To_Tool_Bench") },
            { "BlacksmithScene", () => {
                if (SceneManager.GetActiveScene().name == "DungeonScene") {
                    SoundManager.Instance.PlaySound("Dungeon_Stairs");
                } else {
                    SoundManager.Instance.PlaySound("Button_Press");
                }}},
            { "World1Scene", () => SoundManager.Instance.PlaySound("GoToWorld") },
            { "World2Scene", () => SoundManager.Instance.PlaySound("GoToWorld") },
            { "World3Scene", () => SoundManager.Instance.PlaySound("GoToWorld") },
            { "World4Scene", () => SoundManager.Instance.PlaySound("GoToWorld") },
            { "World5Scene", () => SoundManager.Instance.PlaySound("GoToWorld") },
            { "World6Scene", () => SoundManager.Instance.PlaySound("GoToWorld") }
        };

        if (travelSound.TryGetValue(sceneName, out Action playSound)) {
            playSound();
        }
    }

    #endregion
}
