using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private float respawnTime = 5f;
    [SerializeField] private ParticleSystem respawnEffectPrefab;
    [SerializeField] private float effectLifetime = 5f;
    [SerializeField] private float effectLeadTime = 1.5f; // how early the effect plays before respawn
    [SerializeField] private float minPlayerDistance = 1f; // min distance player must be for box to reactivate
    [SerializeField] private Transform player; // drag your Player object here

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private bool isActive = true;
    private Collider meshCollider;
    private Renderer meshRenderer;

    void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        meshCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<Renderer>();

        // Fallback: auto-find player by tag if not assigned in Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    public void DestroyBox()
    {
        if (!isActive) return;

        isActive = false;

        meshCollider.enabled = false;
        meshRenderer.enabled = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBoxDestroy();

        StartCoroutine(RespawnBox());
    }

    System.Collections.IEnumerator RespawnBox()
    {
        float waitBeforeEffect = Mathf.Max(0f, respawnTime - effectLeadTime);
        yield return new WaitForSeconds(waitBeforeEffect);

        if (respawnEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(respawnEffectPrefab, spawnPosition, spawnRotation);
            Destroy(effect.gameObject, effectLifetime);
        }

        
        yield return new WaitForSeconds(effectLeadTime);

        // Wait until player is far enough away before reactivating
        while (player != null && Vector3.Distance(player.position, spawnPosition) < minPlayerDistance)
        {
            yield return null; // check again next frame
        }
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBoxRespawn();


        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        meshCollider.enabled = true;
        meshRenderer.enabled = true;
        isActive = true;
    }
}