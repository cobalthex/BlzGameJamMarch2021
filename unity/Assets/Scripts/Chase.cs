using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Chase : MonoBehaviour
{
    public Transform target;
    public float updateSeconds = 3;
    public float timeToTeleport = 3;
    public AudioClip deathNoise;
 
    private float updateCountdown;
    private NavMeshAgent agent;
    private AudioSource audioSource;
    private bool isPartial;
    private float timePartial;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        agent.destination = target.position;
        updateCountdown = updateSeconds;
        isPartial = false;
        timePartial = 0.0f;
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
            timePartial = 0.0f;
        }

        if (isPartial)
        {
            timePartial += Time.fixedDeltaTime;
            if (timePartial > timeToTeleport)
            {
                Transform[] destinations = getTeleportDestinations();
                if (destinations.Length > 0)
                {
                    Transform destination = getClosestDestination(destinations, target);
                    agent.Warp(destination.position);
                }

                timePartial = 0.0f; // reset how long we've been unable to find a path to the player
            }
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

            animator.Play("Base Layer.DS_onehand_attack_A");
        }
    }

    Transform[] getTeleportDestinations()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("MonsterSpawn");
        Debug.Log("Found " + objects.Length + " spawn points");
        return Array.ConvertAll(objects, new Converter<GameObject, Transform>(gameObjectToTransform));
    }

    Transform gameObjectToTransform(GameObject gameObject)
    {
        return gameObject.GetComponent<Transform>();
    }

    Transform getClosestDestination(Transform[] candidates, Transform player)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = player.position;
        foreach (Transform potentialTarget in candidates)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        Debug.Log("Best target is " + bestTarget.gameObject.name);
        return bestTarget;
    }
}
