using UnityEngine;

public class EnemyCoward : EnemyWalker
{
    public float fireCooldown;
    public GameObject enemyProjectile;
    private float currentCooldown = 0;

    void Update()
    {
        if (enemyStatus is EnemyStatusAggressive)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown < 0)
            {
                currentCooldown = fireCooldown;
                Instantiate(enemyProjectile, transform.position, transform.rotation);
            }
        }
        Walk(false);
    }
}