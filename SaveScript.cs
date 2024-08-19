using UnityEngine;

public class SaveScript {
    #region StaticMethods

    public static void SaveIntValue(string name, int value) {
        PlayerPrefs.SetInt(name, value);
        PlayerPrefs.Save();
    }

    public static void SaveFloatValue(string name, float value) {
        PlayerPrefs.SetFloat(name, value);
        PlayerPrefs.Save();
    }

    public static void SaveStringValue(string name, string value) {
        PlayerPrefs.SetString(name, value);
        PlayerPrefs.Save();
    }

    #endregion
}
