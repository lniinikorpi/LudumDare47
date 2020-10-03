using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    int _currentHealth;
    public AudioSource audioSource;
    public AudioClip hitClip;
    public AudioClip deathClip;
    void Start()
    {
        _currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int value, Vector2 enemyPosition)
    {
        audioSource.clip = hitClip;
        audioSource.Play();
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

    void Die()
    {
        audioSource.clip = deathClip;
        audioSource.Play();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
}
