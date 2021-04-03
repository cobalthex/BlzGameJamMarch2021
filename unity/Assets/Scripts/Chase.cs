using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    public Transform target;
    public float updateSeconds;
    public AudioClip deathNoise;

    private float updateCountdown;
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private bool isPartial;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        agent.destination = target.position;
        updateCountdown = updateSeconds;
        isPartial = false;
    }

    void FixedUpdate()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathPartial && !isPartial)
        {
            Debug.Log("Can't reach dest anymore");
            isPartial = true;
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathComplete && isPartial)
        {
            Debug.Log("Can now reach dest");
            isPartial = false;
        }
        

        updateCountdown -= Time.fixedDeltaTime;
        if (updateCountdown <= 0)
        {
            agent.destination = target.position;
            updateCountdown = updateSeconds;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Player"))
        {
            audioSource.Stop();
            audioSource.clip = deathNoise;
            audioSource.loop = false;
            audioSource.Play();

            // TODO: wire in other end of game state
        }
    }
}
