using UnityEngine;

public class collide : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private float effectLifetime = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided with enemy!");

            if (hitEffect != null)
            {
                ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect.gameObject, effectLifetime);
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayPlayerDeath();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}