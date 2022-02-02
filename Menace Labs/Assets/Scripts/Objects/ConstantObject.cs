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
    [SerializeField] BreakType breakType;
    [SerializeField] DirtType dirtType;
    [Tooltip("The dirty object should have the clean object and vice versa, always clean objects don't have an alternate")]
    [SerializeField] GameObject alternateDirtVersion;
    [Tooltip("The working object should have the broken object and vice versa, non-breakable objects don't have an alternate")]
    [SerializeField] GameObject alternateBreakVersion;

    protected Renderer materialRenderer;
    protected AudioSource audioSource;
    protected ParticleSystem particles;

    protected bool beingUsed;

    private float timeToCheckEffects = 1.0f;
    private float effectsTimer;

    protected void Start()
    {
        materialRenderer = GetComponentInChildren<Renderer>();
        audioSource = GetComponent<AudioSource>();
        particles = GetComponentInChildren<ParticleSystem>();
    }
    private void Update()
    {
        if (beingUsed)
        {
            Use();
        }
    }

    public virtual bool StartUsing()
    {
        beingUsed = true;

        if (audioSource && startSound != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            audioSource.clip = SoundManager.instance.GetClip(startSound);
            audioSource.loop = false;
            audioSource.Play();
        }
        if (particles) particles.Play();

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
    public virtual void FinishUsing()
    {
        if (audioSource && endSound != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            audioSource.clip = SoundManager.instance.GetClip(endSound);
            audioSource.loop = false;
            audioSource.Play();
        }
        if (particles) particles.Stop();

        DirtyOrBrokenCheck();

        beingUsed = false;
    }
    public virtual void CancelUsing()
    {
        if (audioSource && endSound != SoundManager.SoundName.NUM_SOUND_NAMES)
        {
            audioSource.clip = SoundManager.instance.GetClip(endSound);
            audioSource.loop = false;
            audioSource.Play();
        }
        if (particles) particles.Stop();

        DirtyOrBrokenCheck();

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
                    SetToElectricAlternate();
                    break;
            }
        }
    }

    public void SetToDirtAlternate()
    {
        alternateDirtVersion.SetActive(true);
        gameObject.SetActive(false);
    }
    public void SetToElectricAlternate()
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
        ManagerHandler.instance.ActionM.AddAction(this);
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