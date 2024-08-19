using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonScript : MonoBehaviour {
    #region PublicVariables

    public GameObject tokenPrefab;
    public Canvas tokenCanvas;

    #endregion
    #region PrivateVariables

    private GameObject displaySword;
    private bool swingingSword = false;

    #endregion
    #region Singleton

    public static DungeonScript Instance;

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

    void Update() {
        if (displaySword == null) return;

        Vector3 currentRotation = displaySword.transform.eulerAngles;
        float swingSpeed = 0.4f * (Time.deltaTime * 1000);

        if (swingingSword) {
            displaySword.transform.Rotate(0, 0, swingSpeed);

            if (currentRotation.z >= 80f) {
                swingingSword = false;
            }
        } else if (currentRotation.z >= 16f) {
            displaySword.transform.Rotate(0, 0, -swingSpeed);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "DungeonScene") tokenCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    #endregion
    #region PublicMethods

    public void HitEnemy() {
        GameObject token = Instantiate(tokenPrefab, tokenCanvas.transform);

        _ = StartCoroutine(AnimateToken(token));
        SwingSword();

        SoundManager.Instance.PlaySound("Sword_Hit");
        SoundManager.Instance.PlaySound("Sword_Cut_Slime");
    }

    #endregion
    #region PrivateMethods

    private void SwingSword() {
        displaySword = GameObject.Find("DisplaySword");
        if (displaySword != null) swingingSword = true;
    }

    private IEnumerator AnimateToken(GameObject token) {
        Vector2 randomOffset = new Vector2(Random.Range(-50f, 50f), Random.Range(0f, 50f));
        Vector2 startPosition = GameObject.Find("SlimeButton").transform.position;
        Vector2 endPosition = GameObject.Find("TokenIcon").transform.position;
        Image tokenImage = token.GetComponentInChildren<Image>();

        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            if (token == null) break;

            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;
            token.transform.position = Vector2.Lerp(startPosition, startPosition + randomOffset, t) + Vector2.Lerp(Vector2.zero, endPosition - (startPosition + randomOffset), t * t);

            float rotationSpeed = 360f;
            tokenImage.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        if (token != null) token.transform.position = endPosition;

        if (Inventory.Instance.HasSwordEquipped()) {
            double swordRarity = Inventory.Instance.EquippedSword.SwordItem.Power();
            TokenManager.Instance.IncreaseTokens(swordRarity);
        } else {
            TokenManager.Instance.IncreaseTokens();
        }

        Destroy(token);
    }

    #endregion
}
