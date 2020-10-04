using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    int _currentHealth;
    public AudioSource audioSource;
    public AudioClip hitClip;
    public AudioClip deathClip;
    public GameObject playerPanel;
    public Scrollbar reloadBar;
    public Animator cameraAnim;

    public PlayerShoot playerShoot;
    public PlayerMovement playerMovement;
    void Start()
    {
        _currentHealth = maxHealth;
        playerPanel.SetActive(false);
        reloadBar.size = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int value, Vector2 enemyPosition)
    {
        audioSource.clip = hitClip;
        audioSource.Play();
        cameraAnim.Play("CameraShake");
        if(_currentHealth - value > 0)
        {
            _currentHealth -= value;
            float percentage = (float)_currentHealth / (float)maxHealth;
            SpawnBlood(enemyPosition);
            GameManager.instance.UpdateHealthBar(percentage);
        }
        else
        {
            _currentHealth = 0;
            Die();
        }
    }

    public void Heal(int value)
    {
        _currentHealth += value;
        if (_currentHealth > maxHealth)
            _currentHealth = maxHealth;

        float percentage = (float)_currentHealth / (float)maxHealth;
        GameManager.instance.UpdateHealthBar(percentage);
    }

    void Die()
    {
        audioSource.clip = deathClip;
        audioSource.Play();
        GameManager.instance.gamePlaying = false;
        GameManager.instance.endScreen.SetActive(true);
        GameManager.instance.endScoreText.text = "Score: " + GameManager.instance.playerScore;
        Destroy(gameObject);
    }

    void SpawnBlood(Vector2 enemyPosition)
    {
        GameObject blood = Instantiate(GameManager.instance.bloodSplatHit, transform.position, Quaternion.identity);

        Vector2 direction = enemyPosition - (Vector2)transform.position;
        direction = direction.normalized;

        float angle = Vector2.SignedAngle(Vector2.up, direction);
        Vector3 directionVector = new Vector3(0, 0, 180 + angle);
        blood.transform.localEulerAngles = directionVector;
    }

    public void PowerUp(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.superSprint:
                StartCoroutine(playerMovement.SuperSprint());
                break;
            case PowerUpType.unlimitedAmmo:
                StartCoroutine(playerShoot.UnlimitedAmmo());
                break;
            case PowerUpType.tripleShot:
                StartCoroutine(playerShoot.TripleShot());
                break;
        }
    }
}
