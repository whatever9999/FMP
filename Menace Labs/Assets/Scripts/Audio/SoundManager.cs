using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public enum SoundName
    {
        ARCADE_MACHINE,
        BORED,
        BROKEN_ITEM,
        BUTTON_CLICK,
        CAMERA,
        CHANGE_VOLUME,
        COUGHING,
        DEATH,
        EARTHQUAKE,
        EATING,
        ELECTROCUTION,
        FAILURE,
        FIRE,
        FLARE,
        FLOOD,
        FRIDGE,
        JUKEBOX,
        OVEN,
        PHONE,
        PUDDLE,
        REACTING,
        REFUSING,
        RIOT,
        RUBBISH,
        SHOWER,
        SLEEP,
        DARTS,
        SOFA,
        STEP,
        SUCCESS,
        THROWING_UP,
        TYPING,
        FIXING,
        CLEANING,
        NUM_SOUND_NAMES,
    }
    public enum AmbienceName
    {
        DAY,
        NIGHT,
    }
    public enum MusicName
    {
        HESITATING,
        ST_JAMES,
        ST_LOUIS,
        WORRIED_MAN,
    }

    #region Setup
    [SerializeField] private SoundEffect[] soundEffects;
    [SerializeField] private AmbienceTrack[] ambienceTracks;
    [SerializeField] private MusicTrack[] musicTracks;

    [SerializeField] private AudioMixerGroup SFXMixerGroup;
    public AudioMixerGroup GetSFXMixerGroup() { return SFXMixerGroup; }

    private Dictionary<SoundName, AudioClip> soundDictionary = new Dictionary<SoundName, AudioClip>();
    private Dictionary<AmbienceName, AudioClip> ambienceDictionary = new Dictionary<AmbienceName, AudioClip>();
    private MusicTrack[] musicTrackList = new MusicTrack[4];

    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();

        // Populate sfx dictionary
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundDictionary.ContainsKey(soundEffects[i].name))
            {
                Debug.LogError("Trying to add multiple sound effects of type " + soundEffects[i].name);
            }
            else
            {
                soundDictionary.Add(soundEffects[i].name, soundEffects[i].clip);
            }
        }

        // Populate ambience track dictionary
        for (int i = 0; i < ambienceTracks.Length; i++)
        {
            if (ambienceDictionary.ContainsKey(ambienceTracks[i].name))
            {
                Debug.LogError("Trying to add multiple ambience tracks of type " + soundEffects[i].name);
            }
            else
            {
                ambienceDictionary.Add(ambienceTracks[i].name, ambienceTracks[i].clip);
            }
        }
        // Set the ambience
        ChangeAmbienceTrack(AmbienceName.DAY);

        // Randomly order the music tracks
        for (int i = 0; i < musicTracks.Length; i++)
        {
            bool gotTrack = false;
            while (!gotTrack)
            {
                int randomIndex = Random.Range(0, musicTracks.Length);
                if (!musicTrackList[randomIndex].clip)
                {
                    musicTrackList[randomIndex] = musicTracks[i];
                    gotTrack = true;
                }
            }
        }
    }
    #endregion // Setup

    [SerializeField] private AudioSource ambienceSource;
    public void ChangeAmbienceTrack(AmbienceName track)
    {
        AudioClip newClip;
        ambienceDictionary.TryGetValue(track, out newClip);

        ambienceSource.clip = newClip;
        ambienceSource.time = Random.Range(0.0f, newClip.length);
        ambienceSource.Play();
    }

    [SerializeField] private AudioSource musicSource;
    private int currentTrack = 0;
    private void Update()
    {
        // Start the next music track when the previous one is finished
        if (!musicSource.isPlaying)
        {
            musicSource.clip = musicTrackList[currentTrack].clip;
            musicSource.Play();
            currentTrack++;
            // Loop around to the first track once we've reached the end of the track list
            if (currentTrack == musicTrackList.Length) currentTrack = 0;
        }
    }

    public AudioClip GetClip(SoundName sound)
    {
        AudioClip clip;
        soundDictionary.TryGetValue(sound, out clip);
        return clip;
    }

    // Play non-3D SFX clips on the SoundManager
    public void PlayClip(SoundName sound)
    {
        audioSource.PlayOneShot(GetClip(sound));
    }

    public void PlayClipOnObject(SoundName sound, string objectName)
    {
        AudioSource objectAudioSource;
        bool foundObject = GameObject.Find(objectName).TryGetComponent<AudioSource>(out objectAudioSource);

        if (foundObject) objectAudioSource.PlayOneShot(GetClip(sound));
    }
}

[System.Serializable]
public struct SoundEffect
{
    public SoundManager.SoundName name;
    public AudioClip clip;
}

[System.Serializable]
public struct AmbienceTrack
{
    public SoundManager.AmbienceName name;
    public AudioClip clip;
}

[System.Serializable]
public struct MusicTrack
{
    public SoundManager.MusicName name;
    public AudioClip clip;
}