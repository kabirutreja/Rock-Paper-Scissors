using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDeployer : MonoBehaviour
{
 [Header("Prefabs")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject paperPrefab;
    [SerializeField] private GameObject scissorsPrefab;

    [Header("Deploy Settings")]
    [SerializeField] private KeyCode deployKey = KeyCode.Space;
    [SerializeField] private float spawnDistance = 1.5f; // how far in front of the player

    private Playerpower powerSystem;

    void Start()
    {
        powerSystem = GetComponent<Playerpower>();
    }

    void Update()
    {
        if (Input.GetKeyDown(deployKey))
        {
            DeployPower();
        }
    }

    void DeployPower()
    {
        PowerType current = powerSystem.GetCurrentPower();

        if (current == PowerType.None)
        {
            Debug.Log("No power to deploy!");
            return;
        }

        GameObject prefabToSpawn = GetPrefabForPower(current);

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned for power: " + current);
            return;
        }

        Vector3 spawnPos = transform.position + transform.forward * spawnDistance;
        Quaternion spawnRot = transform.rotation;

        Instantiate(prefabToSpawn, spawnPos, spawnRot);

        // Power is consumed -> goes back to None, so player can pick up a new random one
        powerSystem.UsePower();
    }

    GameObject GetPrefabForPower(PowerType type)
    {
        switch (type)
        {
            case PowerType.Rock:     return rockPrefab;
            case PowerType.Paper:    return paperPrefab;
            case PowerType.Scissors: return scissorsPrefab;
            default:                 return null;
        }
    }
}