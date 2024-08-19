using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {
    #region PublicVariables

    public static MusicManager Instance;

    public AudioClip[] MusicClips;

    #endregion
    #region PrivateVariables

    private AudioSource audioSource;

    #endregion
    #region LifecycleMethods

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        } else {
            Destroy(gameObject);
        }
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    #endregion
    #region PublicMethods

    public void PlayMusic(AudioClip musicClip) {
        if (audioSource.clip != musicClip) {
            audioSource.clip = musicClip;
            audioSource.Play();
        }
    }

    public void StopMusic() => audioSource.Stop();

    #endregion
    #region PrivateMethods

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        => PlayMusic(MusicClips[scene.buildIndex]);

    #endregion
}
