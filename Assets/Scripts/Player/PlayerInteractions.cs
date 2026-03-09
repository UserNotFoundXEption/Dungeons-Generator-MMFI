using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private Player player;
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.tag)
        {
            case "Enemy":
                player.Damage();
                break;
            case "EnemyProjectile":
                player.Damage();
                Destroy(collider.gameObject);
                break;
            case "MageOrbital":
                player.Damage();
                break;
            case "Treasure":
                if (player.HasKey())
                {
                    player.AddAmmo();
                    Destroy(collider.gameObject);
                }
                break;
            case "Key":
                player.AddKey();
                Destroy(collider.gameObject);
                break;
        }
    }


}
