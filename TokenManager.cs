using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TokenManager : MonoBehaviour {
    #region PrivateVariables

    private static double tokens = 0;

    #endregion
    #region Singleton

    public static TokenManager Instance;

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
        string strTokens = LoadScript.LoadStringValue("Tokens");

        if (!string.IsNullOrEmpty(strTokens)) {
            tokens = double.Parse(LoadScript.LoadStringValue("Tokens"));
        } else {
            SaveScript.SaveStringValue("Tokens", "0");
        }

        GameObject tokenTextObject = GameObject.Find("TokensText");

        if (tokenTextObject != null) {
            UpdateTokenText();
        }
    }

    void OnDestroy() => SaveScript.SaveStringValue("Tokens", tokens.ToString());

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (GameObject.Find("TokensText") != null) UpdateTokenText();
    }

    #endregion
    #region PublicMethods

    public void IncreaseTokens() {
        tokens++;
        UpdateTokenText();
    }

    public void IncreaseTokens(double amount) {
        tokens += amount;
        UpdateTokenText();
    }

    public void DecreaseTokens() {
        tokens--;
        UpdateTokenText();
    }

    public void DecreaseTokens(double amount) {
        tokens -= amount;
        UpdateTokenText();
    }

    public void SetTokens(double amount) {
        tokens = amount;
        UpdateTokenText();
    }

    public double GetTokens() => tokens;
    public bool HasEnoughTokens(double amount) => amount <= tokens;

    #endregion
    #region PrivateMethods

    private void UpdateTokenText() {
        GameObject tokenTextObject = GameObject.Find("TokensText");

        if (tokenTextObject != null) {
            TextMeshProUGUI tokenText = tokenTextObject.GetComponent<TextMeshProUGUI>();
            tokenText.SetText("Tokens: " + UtilityScript.FormatNumber(tokens));
        }
    }

    #endregion
}
