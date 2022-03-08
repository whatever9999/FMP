using UnityEngine;
using UnityEngine.AI;

public class Clone : MonoBehaviour
{
    public enum CensorTypes
    {
        NONE,
        FULL_BODY,
        LOWER_BODY,
    }

    [SerializeField] private GameObject fullBodyCensor;
    [SerializeField] private GameObject lowerBodyCensor;

    [SerializeField] private Transform hand;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem smellParticles;
    [SerializeField] private float runSpeed;

    private NavMeshAgent clone;
    private AudioSource audioSource;
    private float walkSpeed;

    private bool isSmelly = false;

    public bool IsSmelly() { return isSmelly; }
    public bool IsOnFire() { return ManagerHandler.instance.NeedsM.IsOnFire(); }
    public bool IsIll() { return ManagerHandler.instance.NeedsM.IsIll(); }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clone = GetComponent<NavMeshAgent>();

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
        TimedObject timedObject = instantiated.GetComponent<TimedObject>();
        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.TIMED_OBJECT_USE, 1, timedObject.gameObject);
    }
    // Spawn an object in front of the clone
    public void SpawnObject(GameObject gameObject)
    {
        Vector3 spawnPos = transform.position + (transform.right);
        GameObject instantiated = Instantiate(gameObject);
        instantiated.transform.position = spawnPos;
    }

    // Play 3D SFX from the clone
    public void PlaySound(SoundManager.SoundName name)
    {
        AudioClip clip = SoundManager.instance.GetClip(name);
        audioSource.PlayOneShot(clip);
    }
    public void SetSoundLooping(bool loop)
    {
        audioSource.loop = loop;
    }

    public void Electrocute()
    {
        PlaySound(SoundManager.SoundName.ELECTROCUTION);
        ManagerHandler.instance.AnimationM.SetAnimation(AnimationManager.AnimationType.ELECTROCUTION, true);
        ManagerHandler.instance.NeedsM.Electrocute();
    }
    public void SetOnFire(bool onFire)
    {
        ManagerHandler.instance.NeedsM.SetOnFire(onFire);
        ManagerHandler.instance.AnimationM.SetAnimation(AnimationManager.AnimationType.ON_FIRE, onFire);
        if (onFire)
        {
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
}
