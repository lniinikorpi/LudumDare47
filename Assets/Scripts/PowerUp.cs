using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    superSprint,
    unlimitedAmmo,
    tripleShot
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
    public SpriteRenderer spriteRenderer;
    public GameObject pickuParticle;
    // Start is called before the first frame update
    void Start()
    {
        int power = Random.Range(0, 3);
        switch (power)
        {
            case 0:
                powerUpType = PowerUpType.superSprint;
                spriteRenderer.sprite = GameManager.instance.superSprint;
                break;
            case 1:
                powerUpType = PowerUpType.unlimitedAmmo;
                spriteRenderer.sprite = GameManager.instance.unlimitedAmmo;
                break;
            case 2:
                powerUpType = PowerUpType.tripleShot;
                spriteRenderer.sprite = GameManager.instance.tripleShot;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.powerUpsSpawned--;
            Instantiate(pickuParticle, transform.position, pickuParticle.transform.rotation);
            collision.gameObject.GetComponent<Player>().PowerUp(powerUpType);
            Destroy(gameObject);
        }
    }
}
