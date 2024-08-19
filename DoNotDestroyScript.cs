using UnityEngine;

public class DoNotDestroyScript : MonoBehaviour {
    #region Singleton

    public static DoNotDestroyScript Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion
}
