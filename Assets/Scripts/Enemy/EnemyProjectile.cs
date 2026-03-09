using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public float projectileSpeed;
    public float torque;
    void Start()
    {
        Vector2 playerPosition = GameObject.Find("PlayerMovePoint").GetComponent<Transform>().position;
        float xRelative = playerPosition.x - transform.position.x;
        float yRelative = playerPosition.y - transform.position.y;
        float angle = Mathf.Atan2(yRelative, xRelative);
        float xForce = projectileSpeed * Mathf.Cos(angle);
        float yForce = projectileSpeed * Mathf.Sin(angle);
        rb.AddTorque(torque);
        rb.AddForce(new Vector2(xForce, yForce));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        if (collider.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
