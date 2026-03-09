using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int hp = 3;
    public int keys = 1;
    public int ammo = 3;
    public int ammoGain = 2;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI keysText;
    public TextMeshProUGUI ammoText;
    public GameObject fireball;
    public Sprite spriteNormal;
    public Sprite spriteDamaged;
    public double invincibilityFrame = 2;
    private double invincibilityTimer = 0;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        UpdateCounters();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (hp <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
        invincibilityTimer -= Time.deltaTime;
        if (invincibilityTimer < 0)
        {
            spriteRenderer.sprite = spriteNormal;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && ammo > 0)
        {
            Vector3 spawnCoords = new(transform.position.x, transform.position.y, 0);
            Instantiate(fireball, spawnCoords, new Quaternion(0, 0, 0, 0));
            ammo--;
            UpdateCounters();
        }
    }
    public void Damage()
    {
        if (invincibilityTimer < 0)
        {
            hp--;
            UpdateCounters();
            invincibilityTimer = 2;
            spriteRenderer.sprite = spriteDamaged;
        }

    }

    public void AddAmmo()
    {
        ammo += ammoGain;
        keys--;
        UpdateCounters();

    }

    public void AddKey()
    {
        keys++;
        UpdateCounters();
    }

    public bool HasKey()
    {
        return keys > 0;
    }

    private void UpdateCounters()
    {
        hpText.text = "" + hp;
        keysText.text = "" + keys;
        ammoText.text = "" + ammo;
    }
}
