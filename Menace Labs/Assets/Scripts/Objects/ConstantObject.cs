using System.Collections.Generic;
using UnityEngine;

public class ConstantObject : MonoBehaviour
{
    [Header("Need and Skill Effects")]
    [SerializeField] private List<ObjectEffect> effects;
    
    [Header("Hover Colour")]
    [SerializeField] protected Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1);
    
    [Header("SFX")]
    [SerializeField] protected SoundManager.SoundName startSound = SoundManager.SoundName.NUM_SOUND_NAMES;
    [SerializeField] protected SoundManager.SoundName useSound = SoundManager.SoundName.NUM_SOUND_NAMES;
    [SerializeField] protected SoundManager.SoundName endSound = SoundManager.SoundName.NUM_SOUND_NAMES;
    
    [Header("Action Icon and Tooltip")]
    [SerializeField] protected Sprite actionIcon;
    [SerializeField] protected string tooltip;
    public Sprite GetActionIcon() { return actionIcon; }
    public string GetTooltip() { return tooltip; }
    
    [Header("Animation Set")]
    [SerializeField] protected AnimationManager.AnimationType animationType = AnimationManager.AnimationType.NUM_ANIMATION_TYPES;
    public AnimationManager.AnimationType GetAnimationType() { return animationType; }

    [Header("Censor")]
    [SerializeField] protected Clone.CensorTypes censorType;

    public enum DirtType
    {
        ALWAYS_CLEAN,
        DIRTIABLE,
        DIRTY,
    }
    public enum BreakType
    {
        NOT_BREAKABLE,
        WORKING,
        BROKEN,
    }
    [Header("Breakable or Dirtiable")]
    [SerializeField] protected BreakType breakType;
    [SerializeField] protected DirtType dirtType;
    [Tooltip("The dirty object should have the clean object and vice versa, always clean objects don't have an alternate")]
    [SerializeField] private GameObject alternateDirtVersion;
    [Tooltip("The working object should have the broken object and vice versa, non-breakable objects don't have an alternate")]
    [SerializeField] private GameObject alternateBreakVersion;
    public BreakType GetBreakType() { return breakType; }

    [Header("Object Use")]
    [Tooltip("Where should the clone be to use the object?")]
    [SerializeField] protected Transform requiredLocation;
    public Transform GetRequiredLocation() { return requiredLocation; }
    [SerializeField] protected bool faceTransformDirection;

    protected Renderer materialRenderer;
    protected AudioSource audioSource;
    protected ParticleSystem particles;

    protected int startedUsingTime;
    protected bool beingUsed;

    private float timeToCheckEffects = 1.0f;
    private float effectsTimer;

    private void Awake()
    {
        // Prefabs that don't exist in the scene will need to collect references to required locations in Awake
        if (name.Contains("Sandwich")) requiredLocation = GameObject.Find("DiningChairRL").transform;
        if (name.Contains("Donut")) requiredLocation = GameObject.Find("DiningChairRL").transform;
    }

    protected void Start()
    {
        materialRenderer = GetComponentInChildren<Renderer>();
        // If the object doesn't have an audio source add one (things like fires should loop and play on awake so they'll have a source already)
        if (!TryGetComponent<AudioSource>(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = ManagerHandler.instance.SoundM.GetSFXMixerGroup();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0.8f;
        }
        particles = GetComponentInChildren<ParticleSystem>();
    }

    protected virtual void Update()
    {
        // Adjust pitch according to timescale so tempo changes accordingly
        if (audioSource.isPlaying && Time.timeScale == 0)
        {
            audioSource.Pause();
        }
        else if (Time.timeScale > 1)
        {
            audioSource.pitch = ManagerHandler.instance.TimeM.GetSpeedySoundPitch();
        }
        else
        {
            audioSource.pitch = Time.timeScale;
        }
    }

    public virtual bool StartUsing()
    {
        // If the clone should face the same direction as the transform to use the object make sure they're rotated
        if (faceTransformDirection)
        {
            ManagerHandler.instance.clone.transform.rotation = requiredLocation.rotation;
        }

        beingUsed = true;
        startedUsingTime = ManagerHandler.instance.TimeM.GetCurrentTime();

        if (audioSource && startSound != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            audioSource.clip = SoundManager.instance.GetClip(startSound);
            audioSource.loop = false;
            audioSource.Play();
        }
        if (particles) particles.Play();

        ManagerHandler.instance.clone.ToggleCensor(censorType, true);

        return true;
    }
    public virtual bool Use()
    {
        if (audioSource && useSound != SoundManager.SoundName.NUM_SOUND_NAMES && !audioSource.isPlaying)
        {
            audioSource.clip = SoundManager.instance.GetClip(useSound);
            audioSource.loop = true;
            audioSource.Play();
        }

        if (particles && !particles.isPlaying) particles.Play();

        effectsTimer += Time.deltaTime;
        if (effectsTimer >= timeToCheckEffects)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                // If the effect is on a need then modify the need
                if (effects[i].GetNeedType() != NeedsManager.NeedType.NONE)
                {
                    ManagerHandler.instance.NeedsM.ModifyNeed(effects[i].GetNeedType(), effects[i].GetValue());
                }
                // If the effect is on a skill then progress the skill
                if (effects[i].GetSkillType() != SkillManager.SkillType.NONE)
                {
                    ManagerHandler.instance.SkillM.ProgressSkill(effects[i].GetSkillType(), effects[i].GetValue());
                }
            }

            effectsTimer = 0.0f;
        }

        return true;
    }
    // Never finish using constant objects
    public virtual void FinishUsing()
    {
    }
    public virtual void CancelUsing()
    {
        // Only play the end use sound if the object use gets cancelled while it's being used
        if (beingUsed && audioSource && endSound != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            audioSource.clip = SoundManager.instance.GetClip(endSound);
            audioSource.loop = false;
            audioSource.Play();
        }
        else if (audioSource)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
        // If we cancelled fixing or cleaning don't tidy particles or change to fixed/clean object
        if (breakType != BreakType.BROKEN && dirtType != DirtType.DIRTY)
        {
            if (particles) particles.Stop();

            if (beingUsed) DirtyOrBrokenCheck();
        }

        ManagerHandler.instance.clone.ToggleCensor(censorType, false);

        // Update Goal Stats
        if (name.Equals("Jukebox")) ManagerHandler.instance.GoalM.ModifyHoursDancing(ManagerHandler.instance.TimeM.TimeSince(startedUsingTime)/60);

        beingUsed = false;
    }

    protected void DirtyOrBrokenCheck()
    {
        // Don't break and dirty at the same time
        bool dirtied = false;
        switch (dirtType)
        {
            case DirtType.DIRTIABLE:
                // Check if object becomes dirty
                ManagerHandler.instance.EventM.SetDirtiableObject(this);
                dirtied = ManagerHandler.instance.EventM.CheckEventTrigger(EventManager.EventType.DIRTYING);
                break;
            case DirtType.DIRTY:
                // If finished using a dirty object then it is now clean
                SetToDirtAlternate();
                break;
        }
        if (!dirtied)
        {
            switch (breakType)
            {
                case BreakType.WORKING:
                    // Check if object breaks
                    ManagerHandler.instance.EventM.SetBreakableObject(this);
                    ManagerHandler.instance.EventM.CheckEventTrigger(EventManager.EventType.BREAKING);
                    break;
                case BreakType.BROKEN:
                    // If finished using a broken object then it is now working
                    SetToBreakableAlternate();
                    break;
            }
        }
    }

    public void SetToDirtAlternate()
    {
        alternateDirtVersion.SetActive(true);
        gameObject.SetActive(false);
    }
    public void SetToBreakableAlternate()
    {
        alternateBreakVersion.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        if (materialRenderer) materialRenderer.material.color = hoverColor;
        else Debug.LogError("Failed to get object renderer!");
    }
    private void OnMouseExit()
    {
        if (materialRenderer) materialRenderer.material.color = Color.white;
        else Debug.LogError("Failed to get object renderer!");
    }
    private void OnMouseDown()
    {
        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.CONSTANT_OBJECT_USE, -1, gameObject);
    }
}

[System.Serializable]
public struct ObjectEffect
{
    [SerializeField] private NeedsManager.NeedType needType;
    [SerializeField] private SkillManager.SkillType skillType;
    [SerializeField] private float value;
    public NeedsManager.NeedType GetNeedType() { return needType; }
    public SkillManager.SkillType GetSkillType() { return skillType; }
    public float GetValue() { return value; }
}