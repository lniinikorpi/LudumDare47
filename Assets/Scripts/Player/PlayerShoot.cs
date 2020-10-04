﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    public float fireRate = 1.5f;
    public int maxClipSize = 10;
    int _currentClipSize = 0;
    public float reloadTime = 2;
    bool _reloading;
    float _canShoot;
    bool _shooting;
    [HideInInspector]
    public float damageMultiplier;
    public AudioClip shootClip;
    public AudioSource audioSource;
    bool _tripleShotActive;
    bool _unlimitedAmmoActive;
    public float unlimitedAmmoFireRate = 10;
    public float unlimitedAmmoTimer = 10;
    public float tripleShotTimer = 10;
    float _currentTripleShotTimer;
    float _currentUnlimitedAmmoTimer;

    Vector2 _mousePosition;

    private void Awake()
    {
        _currentClipSize = maxClipSize;
    }

    private void Start()
    {
        GameManager.instance.UpdateBulletCountText(_currentClipSize);
    }

    private void Update()
    {
        if(Time.time > _canShoot)
        {
            if(_shooting)
            {
                Shoot();
            }
        }
    }

    public void OnAim(InputValue value)
    {
        _mousePosition = value.Get<Vector2>();
        _mousePosition = mainCamera.ScreenToWorldPoint(_mousePosition);
        Aim();
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
        _shooting = true;
    }

    public void OnStopShoot()
    {
        _shooting = false;
    }

    public void OnReload()
    {
        if (!_reloading)
        {
            StartCoroutine(ReloadWeapon());
        }
    }

    IEnumerator ReloadWeapon()
    {
        _reloading = true;
        float timeReloading = 0;
        while(timeReloading < reloadTime)
        {
            timeReloading += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _reloading = false;
        _currentClipSize = maxClipSize;
        GameManager.instance.UpdateBulletCountText(_currentClipSize);
    }

    void Shoot()
    {
        if (_reloading)
            return;

        if(_currentClipSize > 0 || _unlimitedAmmoActive)
        {
            if(_unlimitedAmmoActive)
            {
                _canShoot = Time.time + 1 / unlimitedAmmoFireRate;
            }
            else
            {
                _canShoot = Time.time + 1 / fireRate;
                _currentClipSize--;
            }
            GameObject go;
            if(_tripleShotActive)
            {
                go = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                go.GetComponent<Bullet>().speed = bulletSpeed;
                go.GetComponent<Bullet>().damage = bulletDamage;
                for (int i = 1; i < 3; i++)
                {
                    go = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                    go.transform.localEulerAngles = new Vector3(go.transform.localEulerAngles.x, go.transform.localEulerAngles.y, go.transform.localEulerAngles.z + Mathf.Pow(-1, i) * 15);
                    go.GetComponent<Bullet>().speed = bulletSpeed;
                    go.GetComponent<Bullet>().damage = bulletDamage;
                }
            }
            else
            {
                go = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                go.GetComponent<Bullet>().speed = bulletSpeed;
                go.GetComponent<Bullet>().damage = bulletDamage;
            }
            audioSource.clip = shootClip;
            audioSource.Play();
            bulletSpawn.GetComponent<ParticleSystem>().Play();
            GameManager.instance.UpdateBulletCountText(_currentClipSize);
        }
        else
        {
            StartCoroutine(ReloadWeapon());
        }
    }

    public IEnumerator UnlimitedAmmo()
    {
        _currentUnlimitedAmmoTimer = 0;
        _unlimitedAmmoActive = true;
        while (_currentUnlimitedAmmoTimer < unlimitedAmmoTimer)
        {
            _currentUnlimitedAmmoTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _unlimitedAmmoActive = false;
    }

    public IEnumerator TripleShot()
    {
        _currentTripleShotTimer = 0;
        _tripleShotActive = true;
        while (_currentTripleShotTimer < tripleShotTimer)
        {
            _currentTripleShotTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _tripleShotActive = false;
    }
}
