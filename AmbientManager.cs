using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class AmbientManager : MonoBehaviour {
    #region PublicVariables

    public static AmbientManager Instance;

    public List<AudioClip> AmbientClips1 = new List<AudioClip>();
    public List<AudioClip> AmbientClips2 = new List<AudioClip>();

    #endregion
    #region PrivateVariables

    private AudioSource audioSource1;
    private AudioSource audioSource2;

    #endregion
    #region LifecycleMethods

    void Awake() {
        if (Instance == null) {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            audioSource1 = gameObject.AddComponent<AudioSource>();
            audioSource2 = gameObject.AddComponent<AudioSource>();
            audioSource1.loop = true;
            audioSource2.loop = true;
        } else {
            Destroy(gameObject);
        }
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    #endregion
    #region PublicMethods

    public void PlayAmbient(AudioClip ambientClip, int inputChannel = 1) {
        if (inputChannel == 1) {
            if (audioSource1.clip != ambientClip) {
                audioSource1.clip = ambientClip;
                audioSource1.Play();
            }
        } else {
            if (audioSource2.clip != ambientClip) {
                audioSource2.clip = ambientClip;
                audioSource2.Play();
            }
        }
    }

    public void StopAmbient(int inputChannel = 1) {
        if (inputChannel == 1) {
            audioSource1.Stop();
        } else if (inputChannel == 2) {
            audioSource2.Stop();
        } else {
            audioSource1.Stop();
            audioSource2.Stop();
        }
    }

    #endregion
    #region PrivateMethods

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        PlayAmbient(AmbientClips1[scene.buildIndex]);
        PlayAmbient(AmbientClips2[scene.buildIndex], 2);
    }

    #endregion
}
