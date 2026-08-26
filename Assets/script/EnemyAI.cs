using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private ParticleSystem activeEffect;
    public float spawnWaitTime = 5f;
    public ParticleSystem spawnEffect;
    public float spawnEffectDuration = 3f;
    public float chaseSpeed = 3f;
    public float stoppingDistance = 1f;
    public float rotationSpeed = 5f;
    private NavMeshAgent agent;
    private Transform player;
    private bool isActive = false;
    private float spawnTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent)
        {
            Debug.LogError("NavMeshAgent component not found!");
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!player)
        {
            Debug.LogError("Player object not found!");
        }
        agent.stoppingDistance = stoppingDistance;
        agent.speed = chaseSpeed;
        agent.enabled = false; 
        agent.updateRotation = false; // Disable the agent initially   
        spawnTimer = 0f; // Reset the spawn timer
        isActive = false; // Set the enemy as inactive initially
    }

    // Update is called once per frame
    public void Update()
    {
        if (!isActive)
        {
            spawnTimer += Time.deltaTime;
            if(spawnTimer == Time.deltaTime && spawnEffect != null)
            {
                // Play the spawn effect immediately when the enemy is spawned
                
                   activeEffect = Instantiate(spawnEffect, transform.position, Quaternion.identity);
                
            }
            if (spawnTimer >= spawnWaitTime)
            {
                // Play the spawn effect
                 if(activeEffect != null)
                 Destroy(activeEffect.gameObject);
                // Start a coroutine to wait for the effect duration before activating the enemy
                agent.enabled = true; // Enable the agent
                isActive = true; // Set the enemy as active
            

            }
        }
        else
        {
            if (player != null && agent.enabled)
            {
                FacePlayer();
                agent.SetDestination(player.position);
            }
        }
    }
    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the rotation on the horizontal plane
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
   
}
