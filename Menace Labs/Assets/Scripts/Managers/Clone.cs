using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Clone : MonoBehaviour
{
    public enum CensorTypes
    {
        NONE,
        FULL_BODY,
        LOWER_BODY,
    }

    [Tooltip("Any particle effects/objects on the clone that should disappear while testing")]
    [SerializeField] private GameObject additions;

    [SerializeField] private GameObject fullBodyCensor;
    [SerializeField] private GameObject lowerBodyCensor;

    [SerializeField] private Transform hand;
    public Transform GetHand() { return hand; }
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem smellParticles;
    [SerializeField] private ParticleSystem electricParticles;
    [SerializeField] private float runSpeed;

    private NavMeshAgent clone;
    private AudioSource audioSource;
    private Renderer cloneRenderer;
    private float walkSpeed;

    private bool isSmelly = false;

    public bool IsSmelly() { return isSmelly; }
    public bool IsOnFire() { return ManagerHandler.instance.NeedsM.IsOnFire(); }
    public bool IsIll() { return ManagerHandler.instance.NeedsM.IsIll(); }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clone = GetComponent<NavMeshAgent>();
        cloneRenderer = GetComponentInChildren<Renderer>();

        walkSpeed = clone.speed;
    }

    private void Update()
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

        if (Input.GetMouseButtonDown(0))
        {
            ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.MOVE, -1);
        }
    }

    public bool ReachedDestination()
    {
        // If the clone is within stopping distance then they've reached their destination
        if (clone.transform.position.x > clone.destination.x - clone.stoppingDistance &&
            clone.transform.position.x < clone.destination.x + clone.stoppingDistance &&
            clone.transform.position.z > clone.destination.z - clone.stoppingDistance &&
            clone.transform.position.z < clone.destination.z + clone.stoppingDistance)
        {
            return true;
        }
        return false;
    }

    // Put an object in the clone's hand that they will use immediately
    public void GiveObject(GameObject gameObject)
    {
        GameObject instantiated = Instantiate(gameObject, hand);
        ConstantObject giveObject = instantiated.GetComponent<ConstantObject>();
        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.OBJECT_USE, 1, giveObject.gameObject);
    }
    // Spawn an object in front of the clone
    public void SpawnObject(GameObject gameObject)
    {
        Vector3 spawnPos = transform.position + (transform.right);
        GameObject instantiated = Instantiate(gameObject);
        instantiated.transform.position = spawnPos;
    }

    // Play 3D SFX from the clone
    public void PlaySound(SoundManager.SoundName name, bool loop)
    {
        AudioClip clip = SoundManager.instance.GetClip(name);
        audioSource.loop = loop;
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void SetSoundLooping(bool loop)
    {
        audioSource.loop = loop;
    }
    public void StopSound()
    {
        audioSource.Stop();
    }
    public bool IsPlayingSound()
    {
        return audioSource.isPlaying;
    }

    public void Electrocute()
    {
        electricParticles.Play();
        PlaySound(SoundManager.SoundName.ELECTROCUTION, false);
        ManagerHandler.instance.AnimationM.SetAnimation(AnimationManager.AnimationType.ELECTROCUTION, true);
        ManagerHandler.instance.NeedsM.Electrocute();
        ManagerHandler.instance.NotificationM.AddNotification(NotificationManager.NotificationType.ELECTROCUTION);
        ManagerHandler.instance.camera.JumpToClone();
    }
    public void SetOnFire(bool onFire)
    {
        ManagerHandler.instance.NeedsM.SetOnFire(onFire);
        if (onFire)
        {
            ManagerHandler.instance.camera.JumpToClone();
            ManagerHandler.instance.ActionM.CancelAllActions();
            fireParticles.gameObject.SetActive(true);
            fireParticles.Play();
            clone.speed = runSpeed;
        }
        else
        {
            fireParticles.gameObject.SetActive(false);
            fireParticles.Stop();
            clone.speed = walkSpeed;
        }
    }
    public void SetSmelly(bool smelly)
    {
        isSmelly = smelly;
        if (smelly)
        {
            smellParticles.gameObject.SetActive(true);
            smellParticles.Play();
        }
        else
        {
            smellParticles.gameObject.SetActive(false);
            smellParticles.Stop();
        }
    }

    public void ToggleCensor(CensorTypes censor, bool enable)
    {
        switch (censor)
        {
            case CensorTypes.FULL_BODY:
                fullBodyCensor.SetActive(enable);
                break;
            case CensorTypes.LOWER_BODY:
                lowerBodyCensor.SetActive(enable);
                break;
        }
    }

    public void ToggleClone(bool visible)
    {
        cloneRenderer.enabled = visible;
        additions.SetActive(visible);
    }

    public void ToggleAgent(bool enabled) 
    { 
        clone.enabled = enabled; 
    }
    public IEnumerator EnableAgent(float delay)
    {
        yield return new WaitForSeconds(delay);
        clone.enabled = true;
    }

    public void DeathPopup()
    {
        ManagerHandler.instance.PopupM.ShowDeath();
    }
}
