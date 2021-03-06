﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public int playerLastGateIndex;
    public List<Gate> gates = new List<Gate>();
    public int playerScore;
    public GameObject enemyPrefab;
    public GameObject bloodSplatHit;
    public GameObject bloodSplatDie;
    public GameObject enemySpawns;
    public GameObject powerUpPrefab;
    public GameObject firstWalls;
    public List<Transform> spawns = new List<Transform>();
    [SerializeField]
    float _spawnInterval = 5;
    float _canSpawn;
    float _canReduceMultiplier;
    public float multiplierReduceTime;
    public float onTrackScoreTimer = 1;
    float _canTrackScore;
    public float lapsCompleted;
    public float multiplierDecayRate = .1f;
    public float scoreMultiplier = 1;
    public bool onTrack;
    public Sprite tripleShot;
    public Sprite unlimitedAmmo;
    public Sprite superSprint;
    public int maxPowerUpsSpawned = 4;
    [HideInInspector]
    public int powerUpsSpawned;
    public float powerUpTimer = 15;
    float _canSpawnPowerUp;
    bool _firstGatePassed;
    public bool gamePlaying = false;
    public int spawnedZombiesCount;
    public int maxSpawnedZombies = 300;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text scoreMultiplierText;
    public TMP_Text bulletCountText;
    public Scrollbar playerHealthBar;
    public Scrollbar scoreMultiplierBar;
    public Scrollbar staminaBar;
    public Animator canvasAnim;
    public GameObject gamePanel;
    public GameObject mainMenuPanel;
    public GameObject endScreen;
    public TMP_Text endScoreText;
    public GameObject tutorialCanvas;
    public GameObject upgradePanel;
    public List<ItemButton> itemButtons = new List<ItemButton>();

    [Header("Scores")]
    public int enemyScore = 100;
    public int gateScore = 50;
    public int lapScore = 400;
    public int trackScore = 10;

    [Header("Lap modifiers")]
    public float enemyHealth = 10;
    public float enemySpawnReduce = .25f;
    public float enemySpeed = 10;
    float enemyHealthBoost;
    float enemySpeedBoost;

    [Header("Upgrades")]
    public float healthUpgrade = 10;
    public float speedUpgrade = .5f;
    public float damageUpgrade = 10;
    public float clipSizeUpgrade = 1;
    public float reloadSpeedUpgrade = .25f;
    public float maxStaminaUpgrade = 10;
    public float staminaRechargeUpgrade = 1;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        spawns = enemySpawns.GetComponentsInChildren<Transform>().ToList();
        spawns.RemoveAt(0);
        _canSpawn = _spawnInterval;
        scoreText.text = playerScore.ToString();
        playerHealthBar.size = 1;
        scoreMultiplierBar.size = 0;
        _canSpawnPowerUp = powerUpTimer;
        UpdateMultiplierText();
        AdjustStaminaBar(1);
        firstWalls.SetActive(true);
        gamePlaying = false;
        mainMenuPanel.SetActive(true);
        endScreen.SetActive(false);
        upgradePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_firstGatePassed || !gamePlaying)
            return;

        if(Time.time >= _canSpawn)
        {
            SpawnEnemy();
        }

        if (Time.time >= _canReduceMultiplier)
        {
            if (scoreMultiplier == 1 && scoreMultiplierBar.size == 0) { }
            else
            {
                AdjustMultiplierBar(-multiplierDecayRate * Time.deltaTime);
            }
        }

        if(Time.time >= _canSpawnPowerUp && powerUpsSpawned < maxPowerUpsSpawned)
        {
            SpawnPowerUp();
        }

        if (onTrack)
        {
            if (Time.time >= _canTrackScore)
            {
                AddScore(trackScore);
                _canTrackScore = Time.time + onTrackScoreTimer;
            } 
        }
        else
        {
            _canTrackScore = Time.time + onTrackScoreTimer;
        }
    }

    private void SpawnEnemy()
    {
        if (spawnedZombiesCount < maxSpawnedZombies)
        {
            spawnedZombiesCount++;
            _canSpawn = Time.time + _spawnInterval;
        reroll:
            Transform spawnPos = spawns[UnityEngine.Random.Range(0, spawns.Count)];
            if (Vector2.Distance(spawnPos.position, GameObject.FindGameObjectWithTag("Player").transform.position) < 15 || Vector2.Distance(spawnPos.position, GameObject.FindGameObjectWithTag("Player").transform.position) > 30)
                goto reroll;
            GameObject enemy = Instantiate(enemyPrefab, spawnPos.position, Quaternion.identity);
            enemy.GetComponent<EnemyMovement>().maxHealth += enemyHealthBoost;
            enemy.GetComponent<EnemyMovement>().maxSpeed += enemySpeedBoost; 
        }
    }

    void SpawnPowerUp()
    {
        _canSpawnPowerUp = Time.time + powerUpTimer;
    reroll:
        Transform spawnPos = spawns[UnityEngine.Random.Range(0, spawns.Count)];
        if (Vector2.Distance(spawnPos.position, GameObject.FindGameObjectWithTag("Player").transform.position) < 15)
            goto reroll;
        Instantiate(powerUpPrefab, spawnPos.position, Quaternion.identity);
        powerUpsSpawned++;
    }

    public void AddScore(int value)
    {
        playerScore += (int)(value * scoreMultiplier);
        _canReduceMultiplier = Time.time + multiplierReduceTime;
        canvasAnim.SetTrigger("AddScore");
        AdjustMultiplierBar((float)value / 700);
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = playerScore.ToString();
    }

    public void UpdateHealthBar(float value)
    {
        playerHealthBar.size = value;
    }

    public void CompleteLap()
    {
        int index = UnityEngine.Random.Range(0, 3);
        switch (index)
        {
            case 0:
                enemyHealthBoost += enemyHealth;
                print("hp boosted");
                break;
            case 1:
                enemySpeedBoost += enemySpeed;
                print("Speed boosted");
                break;
            case 2:
                if(_spawnInterval - enemySpawnReduce > .1f)
                {
                    _spawnInterval -= enemySpawnReduce;
                }
                else
                {
                    _spawnInterval = .1f;
                    print("maksimi");
                }
                print("spawn boosted");
                break;
            default:
                break;
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Heal(20);
        foreach (ItemButton itemButton in itemButtons)
        {
            itemButton.NewItems();
        }
        upgradePanel.SetActive(true);
        gamePlaying = false;
    }

    void AdjustMultiplierBar(float value)
    {
        scoreMultiplierBar.size += value;
        if(scoreMultiplierBar.size >= 1 && scoreMultiplier < 10)
        {
            canvasAnim.SetTrigger("AddMultiplier");
            scoreMultiplierBar.size = .1f;
            scoreMultiplier += .5f;
            scoreMultiplierText.fontSize += 5;
            UpdateMultiplierText();
        }
        else if(scoreMultiplierBar.size <= 0 && scoreMultiplier > 1)
        {
            scoreMultiplierBar.size = .9f;
            scoreMultiplier -= .5f;
            scoreMultiplierText.fontSize -= 5;
            UpdateMultiplierText();
        }
    }

    public void AdjustStaminaBar(float value)
    {
        staminaBar.size = value;
    }

    public void UpdateMultiplierText()
    {
        scoreMultiplierText.text = "x" + scoreMultiplier.ToString();
    }

    public void UpdateBulletCountText(int value)
    {
        bulletCountText.text = value.ToString();
    }

    public void PassFirstGate()
    {
        if(!_firstGatePassed)
        {
            _firstGatePassed = true;
            firstWalls.gameObject.SetActive(false);
            _canSpawn = _spawnInterval + Time.time;
            _canSpawnPowerUp = powerUpTimer + Time.time;
        }
        else
        {
            CompleteLap();
        }
    }

    public void StartGame()
    {
        gamePlaying = true;
    }

    public void ContinueGame()
    {
        gamePlaying = true;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
