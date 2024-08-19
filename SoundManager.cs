using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    #region PublicVariables

    public int AudioSourcePoolSize;

    public List<AudioClip> SoundEffects = new List<AudioClip>();
    public List<string> SingleInstanceSounds = new List<string>();

    #endregion
    #region PrivateVariables

    private readonly List<AudioSource> audioSourcePool = new List<AudioSource>();
    private readonly Dictionary<string, AudioSource> playingSingleInstanceSounds = new Dictionary<string, AudioSource>();

    #endregion
    #region Singleton

    public static SoundManager Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < AudioSourcePoolSize; i++) {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Add(newSource);
        }
    }

    #endregion
    #region PublicMethods

    public void PlaySound(int index) => PlayClip(SoundEffects[index]);

    public void PlaySound(string soundName) {
        AudioClip clip = SoundEffects.Find(sfx => sfx.name == soundName);
        PlayClip(clip, soundName);
    }

    private void PlayClip(AudioClip clip, string soundName = "") {
        if (SingleInstanceSounds.Contains(soundName)) {
            if (playingSingleInstanceSounds.ContainsKey(soundName) && playingSingleInstanceSounds[soundName].isPlaying) {
                return;
            }

            AudioSource singleSource = GetAvailableSource();

            if (singleSource != null) {
                singleSource.clip = clip;
                singleSource.Play();
                playingSingleInstanceSounds[soundName] = singleSource;
            }
        } else {
            AudioSource availableSource = GetAvailableSource();

            if (availableSource != null) {
                availableSource.PlayOneShot(clip);
            }
        }
    }

    private AudioSource GetAvailableSource()
        => audioSourcePool.Find(source => !source.isPlaying);

    #endregion
}
