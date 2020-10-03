using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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
    public List<Transform> spawns = new List<Transform>();
    float _spawnInterval = 5;
    float _canSpawn;
    float _canReduceMultiplier;
    public float multiplierReduceTime;
    public float lapsCompleted;
    public float multiplierDecayRate = .1f;
    public float scoreMultiplier = 1;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text scoreMultiplierText;
    public Scrollbar playerHealthBar;
    public Scrollbar scoreMultiplierBar;
    public Scrollbar staminaBar;

    [Header("Scores")]
    public int enemyScore = 100;
    public int gateScore = 50;
    public int lapScore = 400;

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
        _canSpawn = _spawnInterval;
        scoreText.text = playerScore.ToString();
        playerHealthBar.size = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= _canSpawn)
        {
            _canSpawn = Time.time + _spawnInterval;
            Transform spawnPos = spawns[UnityEngine.Random.Range(0, spawns.Count)];
            Instantiate(enemyPrefab, spawnPos.position, Quaternion.identity);
        }

        if(Time.time >= _canReduceMultiplier)
        {
            AdjustMultiplierBar(-multiplierDecayRate * Time.deltaTime);
        }
    }

    public void AddScore(int value)
    {
        playerScore += (int)(value * scoreMultiplier);
        _canReduceMultiplier = Time.time + multiplierReduceTime;
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
        if(lapsCompleted % 2 == 0)
        {
            _spawnInterval -= .5f;
        }
    }

    void AdjustMultiplierBar(float value)
    {
        scoreMultiplierBar.size += value;
        if(scoreMultiplierBar.size >= 1)
        {
            scoreMultiplierBar.size = .1f;
            scoreMultiplier += .5f;
        }
        else if(scoreMultiplierBar.size <= 0)
        {
            scoreMultiplierBar.size = .9f;
            scoreMultiplier -= .5f;
        }
    }
}
