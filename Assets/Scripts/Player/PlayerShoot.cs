using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject weapon;
    public Transform bulletSpawn;
    public GameObject bullet;
    public float bulletSpeed;
    public float bulletDamage;
    [HideInInspector]
    public float damageMultiplier;

    Vector2 _mousePosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
    }

    public void OnAim(InputValue value)
    {
        _mousePosition = value.Get<Vector2>();
        _mousePosition = mainCamera.ScreenToWorldPoint(_mousePosition);
    }

    void Aim()
    {
        Vector2 direction = _mousePosition - (Vector2)transform.position;
        direction = direction.normalized;

        float angle = Vector2.SignedAngle(Vector2.up, direction);
        Vector3 directionVector = new Vector3(0, 0, angle);
        weapon.transform.localEulerAngles = directionVector;
    }

    public void OnShoot()
    {
        GameObject go = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
        go.GetComponent<Bullet>().speed = bulletSpeed;
        go.GetComponent<Bullet>().damage = bulletDamage;
    }
}
