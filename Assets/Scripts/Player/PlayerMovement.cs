using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 4;
    public Animator anim;
    Vector2 _movement;
    public AudioSource audioSource;
    public AudioClip walkClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
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

    private void Move()
    {
        transform.Translate(_movement * speed * Time.deltaTime);
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
    }

    public void PlayWalkSound()
    {
        audioSource.clip = walkClip;
        audioSource.Play();
    }
}
