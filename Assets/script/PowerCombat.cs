using UnityEngine;

public class PowerCombat : MonoBehaviour
{
    [SerializeField] private PowerType myType;
    [SerializeField] private ParticleSystem destroyEffect;
    private float effectLifetime = 5f;

    private bool isDestroyed = false; // prevents double-destroy from simultaneous collisions

    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    void HandleCollision(GameObject other)
    {
        if (isDestroyed) return;

        PowerCombat otherPower = other.GetComponent<PowerCombat>();
        if (otherPower == null) return; // not something we fight

        // Same type = no interaction (tie), skip
        if (otherPower.myType == myType) return;

        if (Beats(myType, otherPower.myType))
        {
            // We win, they lose
            otherPower.DestroySelf();
        }
        else if (Beats(otherPower.myType, myType))
        {
            // They win, we lose
            DestroySelf();
        }
    }

    // Standard RPS rule: does 'a' beat 'b'?
    bool Beats(PowerType a, PowerType b)
    {
        return (a == PowerType.Rock && b == PowerType.Scissors) ||
               (a == PowerType.Scissors && b == PowerType.Paper) ||
               (a == PowerType.Paper && b == PowerType.Rock);
    }

    public void DestroySelf()
{
    if (isDestroyed) return;
    isDestroyed = true;

    if (ScoreManager.Instance != null && CompareTag("Enemy"))
        ScoreManager.Instance.AddScore(1f);

    if (AudioManager.Instance != null)
    {
        // Enemy died = win sound, player died = lose sound
        if (CompareTag("Enemy"))
            AudioManager.Instance.PlayBattleWin();
        else
            AudioManager.Instance.PlayBattleLose();
    }

    if (destroyEffect != null)
    {
        ParticleSystem effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(effect.gameObject, effectLifetime);
    }

    Destroy(gameObject);
}
}