using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using UnityEngine.UIElements;

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

    public SoundSet GetSoundSet(string name)
    {
        foreach (SoundSet soundSet in soundSets)
        {
            if (soundSet.name == name)
            {
                return soundSet;
            }
        }

        return null;  // or however you'd like to handle not finding a match
    }


    public void PlaySound(int bankIndex, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        Sound.x.PlaySoundMain(sounds[bankIndex], position, volume, volVar, pitch, pitchVar, priority, pan);
    }

    public void PlaySoundSet(int bankSetIndex, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        Sound.x.PlaySoundMainSet(soundSets[bankSetIndex].set.ToArray(), position, volume, volVar, pitch, pitchVar, priority, pan);
    }
}

public class Sound : MonoBehaviour
{
    public static Sound x;
    private void Awake()
    {
        if (!x)
        {
            x = this;
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

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }

    public void PlayMusicClip(int index, bool loop)
    {
        if (musicClips != null && index >= 0 && index < musicClips.Length)
        {
            if (musicClips[index] != null)
            {
                musicSource.clip = musicClips[index];
                musicSource.loop = loop;
                StartCoroutine(StartMusicCor());
            }
            else
            {
                //Debug.Log("Music Clip is null");
            }
        }
        else
        {
            Debug.Log("Music index is not valid");
        }
    }

    Coroutine hitCo;

    public void PlayMusicHit(int index)
    {
        if (hitCo != null)
        {
            Debug.Log("You are starting another music hit while another one is already starting.");
            EndMusicNow();
        }
        hitCo = StartCoroutine(PlayMusicHitCor(index));
    }

    IEnumerator PlayMusicHitCor(int index)
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



    IEnumerator StartMusicCor()
    {
        musicSource.Play();
        isPlaying = true;
        musicSource.volume = musicVol;
        yield return null;
    }

    public void EndMusic(float fadeTime)
    {
        if (isPlaying)
            StartCoroutine(EndMusicCor(fadeTime));
    }



    IEnumerator EndMusicCor(float fadeTime)
    {
        float startVol = musicSource.volume;
        float timer = 0;
        while (timer < fadeTime)
        {
            musicSource.volume = startVol - (timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        if (hitCo != null) StopCoroutine(hitCo);
        hitCo = null;

        isPlaying = false;
        musicSource.Stop();
        musicSource.volume = 0f;

    }

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

    public void SetReverbState(int index)
    {
        if (index == currentReverbIndex)
            return;

        Debug.Log("Transitioning to snapshot " + index);

        currentReverbIndex = index;

        audioSnapShots[index].TransitionTo(3f);

    }

    #endregion


    #region SFX


    [Header("SFX")]

    public AmbienceSource[] ambiences;

    public AudioSource audioSource;
    public AudioSource uiaudioSource;

    public List<AudioSource> sources;
    public List<AudioSource> uisources;

    public Soundbank mainBank;
    public Soundbank uiBank;

    public AudioMixer mixer;

    public List<AudioSource> pausedSources;

    public void PauseSounds()
    {
        pausedSources = new List<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source.isPlaying)
            {
                source.Pause();
                pausedSources.Add(source);

            }

        }

        foreach (AmbienceSource source in ambiences)
        {
            source.mySource.Pause();
        }
    }

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





    public AudioSource PlayBankSound(int mainBankIndex, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        return PlaySoundMain(mainBank.sounds[mainBankIndex], position, volume, volVar, pitch, pitchVar, priority, pan);
    }

    public AudioSource PlaySoundMainSet(AudioClip[] audioSounds, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        return PlaySoundMain(audioSounds[Random.Range(0, audioSounds.Length)], position, volume, volVar, pitch, pitchVar, priority, pan);
    }

    public AudioSource PlaySoundMain(AudioClip audio, Vector3 position, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        return PlaySound(audio, position, Random.Range(volume - volVar, volume + volVar), Random.Range(pitch - pitchVar, pitch + pitchVar), priority, pan);
    }

    public AudioSource PlayUIBankSound(int uiBankIndex, float volume, float volVar, float pitch, float pitchVar, int priority, float pan)
    {
        AudioClip audio = uiBank.sounds[uiBankIndex];

        // Check for an inactive object in the pool
        AudioSource availableSource = uisources.FirstOrDefault(s => !s.isPlaying);

        if (availableSource == null)
        {
            if (uisources.Count >= 50)
            {
                Debug.Log("You're asking for over 50 UI Audio sources at once.");
                return null;
            }

            // Create new GameObject as a child with AudioSource component
            GameObject soundObject = new GameObject("UISound");
            soundObject.transform.parent = uiaudioSource.transform.parent; // Set as a child of the UI SFX object
            soundObject.transform.position = Vector3.zero; // UI sounds typically don't have a position

            AudioSource newSource = soundObject.AddComponent<AudioSource>();
            newSource.outputAudioMixerGroup = uiaudioSource.outputAudioMixerGroup;
            newSource.playOnAwake = false;
            uisources.Add(newSource);
            availableSource = newSource;
        }

        // Set the source data and play the UI sound
        SetSourceData(availableSource, audio, Vector3.zero, volume, pitch, priority, pan);
        availableSource.Play();

        return availableSource;
    }


    public AudioSource PlaySound(AudioClip audio, Vector3 position, float volume, float pitch, int priority, float panValue)
    {
        // Check for an inactive object in the pool
        AudioSource availableSource = sources.FirstOrDefault(s => !s.isPlaying);

        if (availableSource == null)
        {
            if (sources.Count >= 50)
            {
                Debug.Log("You're asking for over 50 Audio sources at once.");
                return null;
            }

            // Create new GameObject as a child with AudioSource component
            GameObject soundObject = new GameObject("Sound");
            soundObject.transform.parent = audioSource.transform.parent; // Set as a child of the SFX object
            soundObject.transform.position = position; // Set the sound object's position

            AudioSource newSource = soundObject.AddComponent<AudioSource>();
            newSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            newSource.playOnAwake = false;
            sources.Add(newSource);
            availableSource = newSource;
        }

        // Set the source data and play the sound
        SetSourceData(availableSource, audio, position, volume, pitch, priority, panValue);
        availableSource.Play();

        return availableSource;
    }


    public void SetSourceData(AudioSource source, AudioClip audio, Vector3 position, float volume, float pitch, int priority, float panValue)
    {
        source.clip = audio;
        source.transform.position = position;
        if (position == Vector3.zero)
            source.spatialBlend = 0f;
        else
            source.spatialBlend = 0.9f;
        source.volume = volume;
        source.pitch = pitch;
        source.priority = 128 - priority;
        source.panStereo = panValue;
    }

    #endregion

}
