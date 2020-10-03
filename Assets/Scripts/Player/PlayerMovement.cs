using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 4;
    public Animator anim;
    Vector2 _movement;
    public AudioSource audioSource;
    public AudioClip walkClip;
    public float maxStamina = 100;
    public float staminaRechargeRate = 10;
    public float staminaRechargeTimer = 4;
    float _canRechargeStamina;
    float _currentStamina;
    public float staminaDecayRate = 2;
    bool sprinting;
    public float dashMultiplier = 2;
    float _movementMultiplier = 1;
    public TrailRenderer trailRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (sprinting && _currentStamina > 0)
        {
            Sprint(); 
        }

        if(Time.time >= _canRechargeStamina)
        {
            RechargeStamina();
        }
    }

    public void OnMovement(InputValue value)
    {
        _movement = value.Get<Vector2>();
        if(_movement.x == 0 && _movement.y == 0)
        {
            anim.SetFloat("Movement", 0);
        }
        else
        {
            anim.SetFloat("Movement", 1);
        }
    }

    public void OnSprint()
    {
        sprinting = true; 
    }

    public void OnStopSprint()
    {
        sprinting = false;
        _movementMultiplier = 1;
        anim.speed = 1;
        trailRenderer.emitting = false;
    }

    private void Move()
    {
        transform.Translate(_movement * _movementMultiplier * speed * Time.deltaTime);
    }

    void Sprint()
    {
        if(_currentStamina - staminaDecayRate * Time.deltaTime < 0)
        {
            _movementMultiplier = 1;
            anim.speed = 1;
            sprinting = false;
            _currentStamina = 0;
            trailRenderer.emitting = false;
        }
        else
        {
            _movementMultiplier = dashMultiplier;
            _currentStamina -= staminaDecayRate * Time.deltaTime;
            anim.speed = dashMultiplier;
            trailRenderer.emitting = true;
        }
        _canRechargeStamina = Time.time + staminaRechargeTimer;
        GameManager.instance.AdjustStaminaBar(_currentStamina / maxStamina);
    }

    void RechargeStamina()
    {
        if (_currentStamina < maxStamina)
        {
            _currentStamina += staminaRechargeRate * Time.deltaTime;
            GameManager.instance.AdjustStaminaBar(_currentStamina / maxStamina);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("FirstCollider"))
        {
            collision.gameObject.GetComponentInParent<Gate>().firstColliderDone = true;
        }
        else if (collision.gameObject.CompareTag("SecondCollider"))
        {
            collision.gameObject.GetComponentInParent<Gate>().CheckIfThrough();
        }
        if(collision.gameObject.CompareTag("Track"))
        {
            GameManager.instance.onTrack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Track"))
        {
            GameManager.instance.onTrack = false;
        }
    }

    public void PlayWalkSound()
    {
        audioSource.clip = walkClip;
        audioSource.Play();
    }
}
