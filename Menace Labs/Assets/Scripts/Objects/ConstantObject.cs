using System.Collections.Generic;
using UnityEngine;

public class ConstantObject : MonoBehaviour
{
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

    [Header("Need and Skill Effects")]
    [SerializeField] private List<ObjectEffect> effects;
    public List<ObjectEffect> GetObjectEffects() { return effects; }
    private bool affectsSkill = false;

    [Header("Hover Colour")]
    [SerializeField] protected Color hoverColour = new Color(0.8f, 0.8f, 0.8f, 1);
    
    [Header("SFX")]
    [SerializeField] protected SoundManager.SoundName objectStartSound = SoundManager.SoundName.NUM_SOUND_NAMES;
    [SerializeField] protected SoundManager.SoundName objectUseSound = SoundManager.SoundName.NUM_SOUND_NAMES;
    [SerializeField] protected SoundManager.SoundName objectEndSound = SoundManager.SoundName.NUM_SOUND_NAMES;

    [Space(5)]
    [SerializeField] protected SoundManager.SoundName cloneStartSound = SoundManager.SoundName.NUM_SOUND_NAMES;
    [SerializeField] protected SoundManager.SoundName cloneUseSound = SoundManager.SoundName.NUM_SOUND_NAMES;
    [SerializeField] protected SoundManager.SoundName cloneEndSound = SoundManager.SoundName.NUM_SOUND_NAMES;

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

    [Header("Need Levels to Use")]
    [SerializeField] protected RequiredNeed[] requiredNeeds;

    [Header("Results")]
    [SerializeField] protected EventManager.EventType triggerEvent = EventManager.EventType.NUM_EVENT_TYPES;
    [SerializeField] protected bool despawnObject = false;
    [Tooltip("This object will be spawned at the feet of the clone")]
    [SerializeField] protected GameObject spawnObject;
    [Tooltip("This object will be spawned in the clone's hand and they will use it immediately")]
    [SerializeField] protected GameObject giveObject;

    [Header("Variables")]
    [SerializeField] protected bool affectsEnvironment = false;
    [SerializeField] protected int usesFood = 0;

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

    protected bool finished;
    public bool IsFinished() { return finished; }
    public bool AffectsEnvironment() { return affectsEnvironment; }

    protected Renderer[] materialRenderers;
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
        materialRenderers = GetComponentsInChildren<Renderer>();
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

        // Identify if this object has any effect on skill
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].GetSkillType() != SkillManager.SkillType.NONE)
            {
                affectsSkill = true;
                break;
            }
        }
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
        // If the game object is disabled but we're trying to use it then it's likely that it's broken so cancel the action
        if (!gameObject.activeInHierarchy) return false;

        // Ensure the clone's needs are good enough to use the object
        for (int i = 0; i < requiredNeeds.Length; i++)
        {
            if (!requiredNeeds[i].AtRequiredLevel())
            {
                ManagerHandler.instance.NeedsM.FlashRequiredNeed(requiredNeeds[i].GetNeedType());
                return false;
            }
        }

        // If there isn't enough food for this object to be used cancel the action
        if (usesFood > 0 && !ManagerHandler.instance.FoodM.GotEnoughFood(usesFood))
        {
            return false;
        }
        else if (usesFood > 0)
        {
            ManagerHandler.instance.FoodM.ModifyFoodAmount(-usesFood);
        }

        // If the clone should face the same direction as the transform to use the object make sure they're rotated
        if (faceTransformDirection)
        {
            ManagerHandler.instance.clone.transform.rotation = requiredLocation.rotation;
        }

        beingUsed = true;
        finished = false;
        startedUsingTime = ManagerHandler.instance.TimeM.GetCurrentTime();

        if (audioSource && objectStartSound != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            audioSource.clip = SoundManager.instance.GetClip(objectStartSound);
            audioSource.loop = false;
            audioSource.Play();
        }
        if (cloneStartSound != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            ManagerHandler.instance.clone.PlaySound(cloneStartSound, false);
        }
        if (particles) particles.Play();

        ManagerHandler.instance.clone.ToggleCensor(censorType, true);

        // If this object affects a skill show the skill bar
        if (affectsSkill) ManagerHandler.instance.SkillM.ToggleSkillCapsule(true);

        // If the clone is on fire and this is the shower put them out
        if (name.Contains("Shower")) ManagerHandler.instance.clone.SetOnFire(false);

        return true;
    }
    public virtual bool Use()
    {
        // If the game object is disabled but we're trying to use it then it's likely that it's broken so cancel the action
        if (!gameObject.activeInHierarchy) return false;

        // Ensure the clone's needs are good enough to use the object
        for (int i = 0; i < requiredNeeds.Length; i++)
        {
            if (!requiredNeeds[i].AtRequiredLevel())
            {
                ManagerHandler.instance.NeedsM.FlashRequiredNeed(requiredNeeds[i].GetNeedType());
                return false;
            }
        }

        if (audioSource && objectUseSound != SoundManager.SoundName.NUM_SOUND_NAMES && !audioSource.isPlaying)
        {
            audioSource.clip = SoundManager.instance.GetClip(objectUseSound);
            audioSource.loop = true;
            audioSource.Play();
        }
        if (cloneUseSound != SoundManager.SoundName.NUM_SOUND_NAMES && !ManagerHandler.instance.clone.IsPlayingSound())
        {
            ManagerHandler.instance.clone.PlaySound(cloneUseSound, true);
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
    // We don't finish using ConstantObject but TimedObject and MaxNeedObject will use this function
    public virtual void FinishUsing()
    {
        if (audioSource)
        {
            audioSource.loop = false;
            if (objectEndSound != SoundManager.SoundName.NUM_SOUND_NAMES)
            {
                audioSource.clip = SoundManager.instance.GetClip(objectEndSound);
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }

            if (cloneEndSound != SoundManager.SoundName.NUM_SOUND_NAMES)
            {
                ManagerHandler.instance.clone.PlaySound(cloneEndSound, false);
            }
            else
            {
                ManagerHandler.instance.clone.StopSound();
            }
        }
        if (particles) particles.Stop();

        if (beingUsed) DirtyOrBrokenCheck();

        ManagerHandler.instance.clone.ToggleCensor(censorType, false);

        // Update Goal Stats
        if (name.Equals("Fridge")) ManagerHandler.instance.GoalM.ModifyMealsMade(1);
        else if (name.Equals("Oven")) ManagerHandler.instance.GoalM.ModifyMealsMade(1);
        else if (name.Contains("Dirty")) ManagerHandler.instance.GoalM.ModifyTimesCleaned(1);
        else if (name.Contains("Rubbish")) ManagerHandler.instance.GoalM.ModifyTimesCleaned(1);
        else if (name.Contains("Puddle")) ManagerHandler.instance.GoalM.ModifyTimesCleaned(1);
        else if (name.Contains("Fire")) ManagerHandler.instance.GoalM.ModifyFiresSurvived(1);

        if (affectsSkill) ManagerHandler.instance.SkillM.ToggleSkillCapsule(false);

        bool eventTriggered = false;
        if (triggerEvent != EventManager.EventType.NUM_EVENT_TYPES) eventTriggered = ManagerHandler.instance.EventM.CheckEventTrigger(triggerEvent);
        if (giveObject)
        {
            // Don't give the clone an object if a fire was triggered before this
            if (triggerEvent != EventManager.EventType.FIRE || (triggerEvent == EventManager.EventType.FIRE && !eventTriggered))
            {
                ManagerHandler.instance.clone.GiveObject(giveObject);
            }
        }
        if (spawnObject)
        {
            ManagerHandler.instance.clone.SpawnObject(spawnObject);
        }
        if (despawnObject) Destroy(gameObject);

        finished = true;
        beingUsed = false;
    }
    public virtual void CancelUsing()
    {
        if (audioSource)
        {
            audioSource.loop = false;
            // Only play the end use sound if the object use gets cancelled while it's being used
            if (beingUsed && objectEndSound != SoundManager.SoundName.NUM_SOUND_NAMES)
            {
                audioSource.clip = SoundManager.instance.GetClip(objectEndSound);
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }

            if (cloneEndSound != SoundManager.SoundName.NUM_SOUND_NAMES)
            {
                ManagerHandler.instance.clone.PlaySound(cloneEndSound, false);
            }
            else
            {
                ManagerHandler.instance.clone.StopSound();
            }
        }
        // If we cancelled fixing or cleaning don't tidy particles or change to fixed/clean object
        if (breakType != BreakType.BROKEN && dirtType != DirtType.DIRTY)
        {
            if (particles) particles.Stop();

            if (beingUsed) DirtyOrBrokenCheck();
        }

        ManagerHandler.instance.clone.ToggleCensor(censorType, false);

        // Update Goal Stats
        if (name.Equals("Jukebox")) ManagerHandler.instance.GoalM.ModifyHoursDancing(ManagerHandler.instance.TimeM.TimeSince(startedUsingTime)/60.0f);

        if (affectsSkill) ManagerHandler.instance.SkillM.ToggleSkillCapsule(false);

        // If this object is in the clone's hand then cancelling means it needs to be destroyed
        if (transform.parent == ManagerHandler.instance.clone.GetHand())
        {
            Destroy(gameObject);
        }

        beingUsed = false;
        finished = true;
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

    private void OnMouseOver()
    {
        if (!ManagerHandler.instance.UIM.PauseMenuOpen())
        {
            if (!IsColor(hoverColour) && !ManagerHandler.instance.UIM.IsMouseOverUI())
            {
                ChangeColor(hoverColour);
                
            }
            else if (IsColor(hoverColour) && ManagerHandler.instance.UIM.IsMouseOverUI())
            {
                OnMouseExit();
            }
        }
    }
    private void OnMouseExit()
    {
        ChangeColor(Color.white);
    }
    private void OnMouseDown()
    {
        if (!ManagerHandler.instance.UIM.IsMouseOverUI())
        {
            ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.OBJECT_USE, -1, gameObject);
        }
    }

    private void ChangeColor(Color newColour)
    {
        for (int i = 0; i < materialRenderers.Length; i++)
        {
            // Make sure we're not trying to change the colour of PFX (which don't have this property)
            if (materialRenderers[i].material.HasProperty("_Color"))
            {
                // Ensure alpha stays the same
                newColour.a = materialRenderers[i].material.color.a;
                materialRenderers[i].material.color = newColour;
            }
        }
    }
    private bool IsColor(Color compareColor)
    {
        return (materialRenderers[0].material.color == compareColor);
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

[System.Serializable]
public struct RequiredNeed
{
    [SerializeField] private NeedsManager.NeedType need;
    [SerializeField] private NeedsManager.NeedLevel requiredLevel;

    public NeedsManager.NeedType GetNeedType() { return need; }
    public bool AtRequiredLevel()
    {
        return ManagerHandler.instance.NeedsM.GetNeedValue(need) >= (float)requiredLevel;
    }
}