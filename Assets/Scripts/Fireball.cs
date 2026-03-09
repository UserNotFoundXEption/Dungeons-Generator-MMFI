using UnityEngine;

public class Fireball : MonoBehaviour
{
    public Rigidbody2D rb;
    public float fireballSpeed;
    void Start()
    {
        float xRelative = Input.mousePosition.x - Screen.width / 2;
        float yRelative = Input.mousePosition.y - Screen.height / 2;
        float angle = Mathf.Atan2(yRelative, xRelative);
        float xForce = fireballSpeed * Mathf.Cos(angle);
        float yForce = fireballSpeed * Mathf.Sin(angle);
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);
        rb.AddForce(new Vector2(xForce, yForce));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        if (collider.CompareTag("Enemy"))
        {
            collider.GetComponent<Enemy>().Kill();
            Destroy(gameObject);
        }
        if (collider.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
