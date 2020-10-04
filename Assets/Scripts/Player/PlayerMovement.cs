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
    bool _superSprintActive;
    public float superSprintMultiplier = 5;
    public float superSprintTimer = 5;
    float _currentSuperSprintTimer;
    // Start is called before the first frame update
    void Start()
    {
        _currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.gamePlaying)
            return;
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
        if (!GameManager.instance.gamePlaying)
            return;
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
        if (!GameManager.instance.gamePlaying)
            return;
        sprinting = true; 
    }

    public void OnStopSprint()
    {
        if (!GameManager.instance.gamePlaying)
            return;
        if (_superSprintActive)
            return;

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
        if(_superSprintActive)
        {
            return;
        }
        else
        {
            if (_currentStamina - staminaDecayRate * Time.deltaTime < 0)
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

    public IEnumerator SuperSprint()
    {
        _currentSuperSprintTimer = 0;
        _superSprintActive = true;
        _movementMultiplier = superSprintMultiplier;
        trailRenderer.emitting = true;
        anim.speed = superSprintMultiplier;
        _canRechargeStamina = maxStamina;
        GameManager.instance.AdjustStaminaBar(_currentStamina / maxStamina);
        while(_currentSuperSprintTimer < superSprintTimer)
        {
            _currentSuperSprintTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        trailRenderer.emitting = false;
        _superSprintActive = false;
        _movementMultiplier = 1;
        anim.speed = 1;
    }
}
