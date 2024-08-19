using UnityEngine;

public class LoadScript {
    #region StaticMethods

    public static int LoadIntValue(string name)
        => PlayerPrefs.GetInt(name, 0);

    public static float LoadFloatValue(string name)
        => PlayerPrefs.GetFloat(name, 0.0f);

    public static string LoadStringValue(string name)
        => PlayerPrefs.GetString(name, string.Empty);

    #endregion
}