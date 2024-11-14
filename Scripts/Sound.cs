using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

[System.Serializable]
public class SoundSet
{
    public string name;
    public List<AudioClip> set;
}

[CreateAssetMenu(fileName = "soundBank", menuName = "Audio/Soundbank", order = 1)]
public class Soundbank : ScriptableObject
{
    public List<AudioClip> sounds;
    public List<SoundSet> soundSets;

    // Get a SoundSet by name
    public SoundSet GetSoundSet(string name)
    {
        return soundSets.FirstOrDefault(soundSet => soundSet.name == name);
    }

    // Play a sound by bank index
    public void PlaySound(int bankIndex, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        Sound.instance.PlaySoundMain(sounds[bankIndex], position, volume, volVar, pitch, pitchVar, priority, pan);
    }

    // Play a sound set by bank set index
    public void PlaySoundSet(int bankSetIndex, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        Sound.instance.PlaySoundMainSet(soundSets[bankSetIndex].set.ToArray(), position, volume, volVar, pitch, pitchVar, priority, pan);
    }
}

public class Sound : MonoBehaviour
{
    public static Sound instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensure persistence
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
        SetReverbState(0);
    }

    #region Music

    [System.Serializable]
    public class MusicHit
    {
        public AudioClip startClip;
        public AudioClip mainClip;
    }

    [Header("Music")]
    public bool isPlaying;
    public float musicVol = 0.5f;
    public MusicHit[] musicHits;
    public AudioClip[] musicClips;
    public AudioSource musicSource;

    // Pause the currently playing music
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    // Unpause the paused music
    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }

    // Play a specific music clip by index
    public void PlayMusicClip(int index, bool loop)
    {
        if (musicClips != null && index >= 0 && index < musicClips.Length && musicClips[index] != null)
        {
            musicSource.clip = musicClips[index];
            musicSource.loop = loop;
            StartCoroutine(StartMusicCor());
        }
        else
        {
            Debug.LogWarning("Invalid music clip index or clip is null.");
        }
    }

    private Coroutine hitCo;

    // Play a music hit sequence by index
    public void PlayMusicHit(int index)
    {
        if (hitCo != null)
        {
            Debug.LogWarning("Another music hit is already starting. Ending current hit.");
            EndMusicNow();
        }
        hitCo = StartCoroutine(PlayMusicHitCor(index));
    }

    private IEnumerator PlayMusicHitCor(int index)
    {
        musicSource.clip = musicHits[index].startClip;
        StartCoroutine(StartMusicCor());
        musicSource.loop = false;

        float startClipLengthCutoff = musicHits[index].startClip.length * 0.99999f;
        float timer = 0f;
        while (timer < startClipLengthCutoff && musicSource.time < startClipLengthCutoff)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = musicHits[index].mainClip;
        musicSource.Play();
        musicSource.loop = true;
        hitCo = null;
    }

    private IEnumerator StartMusicCor()
    {
        musicSource.Play();
        isPlaying = true;
        musicSource.volume = musicVol;
        yield return null;
    }

    // End the currently playing music with a fade out
    public void EndMusic(float fadeTime)
    {
        if (isPlaying)
            StartCoroutine(EndMusicCor(fadeTime));
    }

    private IEnumerator EndMusicCor(float fadeTime)
    {
        float startVol = musicSource.volume;
        float timer = 0;
        while (timer < fadeTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0, timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        EndMusicNow();
    }

    // Immediately end the music
    public void EndMusicNow()
    {
        isPlaying = false;
        musicSource.Stop();
        musicSource.volume = 0f;
        if (hitCo != null) StopCoroutine(hitCo);
        hitCo = null;
    }

    #endregion

    #region Reverb

    [Header("Reverb")]
    public int currentReverbIndex;
    public AudioMixerSnapshot[] audioSnapShots;

    // Set the reverb state by transitioning to a specific snapshot
    public void SetReverbState(int index)
    {
        if (index != currentReverbIndex)
        {
            Debug.Log("Transitioning to snapshot " + index);
            currentReverbIndex = index;
            audioSnapShots[index].TransitionTo(3f);
        }
    }

    #endregion

    #region SFX

    [Header("SFX")]
    public AmbienceSource[] ambiences;
    public AudioSource audioSource;
    public AudioSource uiaudioSource;
    public List<AudioSource> sources = new List<AudioSource>();
    public List<AudioSource> uisources = new List<AudioSource>();
    public Soundbank mainBank;
    public Soundbank uiBank;
    public AudioMixer mixer;
    private List<AudioSource> pausedSources;

    // Pause all currently playing sounds
    public void PauseSounds()
    {
        pausedSources = new List<AudioSource>();
        foreach (AudioSource source in sources.Where(s => s.isPlaying))
        {
            source.Pause();
            pausedSources.Add(source);
        }

        foreach (AmbienceSource source in ambiences)
        {
            source.mySource.Pause();
        }
    }

    // Unpause all paused sounds
    public void UnPauseSounds()
    {
        foreach (AudioSource source in pausedSources)
        {
            source.UnPause();
        }
        foreach (AmbienceSource source in ambiences)
        {
            source.mySource.UnPause();
        }
    }

    // Play a sound from the main soundbank
    public AudioSource PlayBankSound(int mainBankIndex, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        return PlaySoundMain(mainBank.sounds[mainBankIndex], position, volume, volVar, pitch, pitchVar, priority, pan);
    }

    // Play a random sound from a set of sounds
    public AudioSource PlaySoundMainSet(AudioClip[] audioSounds, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        return PlaySoundMain(audioSounds[Random.Range(0, audioSounds.Length)], position, volume, volVar, pitch, pitchVar, priority, pan);
    }

    // Play a sound with volume and pitch variance
    public AudioSource PlaySoundMain(AudioClip audio, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        return PlaySound(audio, position, Random.Range(volume - volVar, volume + volVar), Random.Range(pitch - pitchVar, pitch + pitchVar), priority, pan);
    }

    // Play a sound from the UI soundbank
    public AudioSource PlayUIBankSound(int uiBankIndex, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        AudioClip audio = uiBank.sounds[uiBankIndex];
        AudioSource availableSource = uisources.FirstOrDefault(s => !s.isPlaying) ?? CreateUISoundSource();
        SetSourceData(availableSource, audio, Vector3.zero, volume, pitch, priority, pan);
        availableSource.Play();
        return availableSource;
    }

    // Create a new UI audio source if none are available
    private AudioSource CreateUISoundSource()
    {
        if (uisources.Count >= 50)
        {
            Debug.LogWarning("Request exceeds 50 UI Audio sources.");
            return null;
        }

        GameObject soundObject = new GameObject("UISound");
        soundObject.transform.parent = uiaudioSource.transform.parent;
        AudioSource newSource = soundObject.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = uiaudioSource.outputAudioMixerGroup;
        newSource.playOnAwake = false;
        uisources.Add(newSource);
        return newSource;
    }

    // Play a sound
    public AudioSource PlaySound(AudioClip audio, Vector3 position, float volume, float pitch, int priority, float panValue)
    {
        AudioSource availableSource = sources.FirstOrDefault(s => !s.isPlaying) ?? CreateSoundSource(position);
        SetSourceData(availableSource, audio, position, volume, pitch, priority, panValue);
        availableSource.Play();
        return availableSource;
    }

    // Create a new general audio source if none are available
    private AudioSource CreateSoundSource(Vector3 position)
    {
        if (sources.Count >= 50)
        {
            Debug.LogWarning("Request exceeds 50 Audio sources.");
            return null;
        }

        GameObject soundObject = new GameObject("Sound");
        soundObject.transform.parent = audioSource.transform.parent;
        soundObject.transform.position = position;
        AudioSource newSource = soundObject.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        newSource.playOnAwake = false;
        sources.Add(newSource);
        return newSource;
    }

    // Set the properties of an AudioSource
    public void SetSourceData(AudioSource source, AudioClip audio, Vector3 position, float volume, float pitch, int priority, float panValue)
    {
        source.clip = audio;
        source.transform.position = position;
        source.spatialBlend = (position == Vector3.zero) ? 0f : 0.9f;
        source.volume = volume;
        source.pitch = pitch;
        source.priority = 128 - priority;
        source.panStereo = panValue;
    }

    #endregion
}

public class AmbienceSource
{
    public AudioSource mySource;
}
