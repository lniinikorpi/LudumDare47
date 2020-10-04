using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Player _player;
    public float maxSpeed = 5;
    public float maxHealth = 100;
    public int damage = 10;
    public float attackSpeed = 1;
    public float stopDistance = 1;
    public Animator anim;
    float _currentHealth;
    float _canAttack;
    bool inAttackRange;
    public AudioClip hitClip;
    public AudioClip deathClip;
    public AudioClip walkClip;
    public AudioSource audioSource;
    public AudioSource walkSource;
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _currentHealth = maxHealth;
        anim.SetFloat("Movement", 1);
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, _player.gameObject.transform.position) > stopDistance)
        {
            float speed = maxSpeed - (maxSpeed/2 * (1 -_currentHealth / maxHealth)); 
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, speed  * Time.deltaTime); 
        }

        if(inAttackRange)
        {
            if(_canAttack >= 1/attackSpeed)
            {
                Attack();
            }
            else
            {
                _canAttack += Time.deltaTime;
            }
        }
    }

    public void GetHit(float value, Vector2 from, Vector3 bulletAngle)
    {
        SpawnBlood(from, bulletAngle);
        if(_currentHealth - value > 0)
        {
            _currentHealth -= value;
        }
        else
        {
            _currentHealth = 0;
            Die();
        }
    }

    public void Die()
    {
        GameManager.instance.AddScore(GameManager.instance.enemyScore);
        Instantiate(GameManager.instance.bloodSplatDie,transform.position,Quaternion.identity);
        audioSource.clip = deathClip;
        audioSource.Play();
        Destroy(gameObject);
    }

    void SpawnBlood(Vector2 from, Vector3 bulletAngle)
    {
        GameObject go = Instantiate(GameManager.instance.bloodSplatHit, from, Quaternion.identity);
        go.transform.localEulerAngles = new Vector3(0,0,bulletAngle.z);
        audioSource.clip = hitClip;
        audioSource.Play();
    }

    void Attack()
    {
        _player.TakeDamage(damage, transform.position);
        _canAttack = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerHitRange"))
        {
            inAttackRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitRange"))
        {
            inAttackRange = false;
            _canAttack = 0;
        }
    }

    public void PlayWalkSound()
    {
        if(Vector2.Distance(transform.position, _player.transform.position) > 20)
        {
            walkSource.volume = 0;
        }
        else
        {
            walkSource.volume = .4f / Vector2.Distance(transform.position, _player.transform.position);
        }
        walkSource.clip = walkClip;
        walkSource.Play();
    }
}
