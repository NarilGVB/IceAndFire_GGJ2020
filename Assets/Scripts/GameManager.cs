using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private float spawnDelay = 5f;

    [SerializeField] private List<Transform> spawnPoints;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private UnityEngine.Rendering.Universal.Light2D playerLight;
    [SerializeField] private Gradient lightGradient;

    [Header("Enemies")]
    public GameObject iceSpawnAnimationInstance;
    public GameObject fireSpawnAnimationInstance;
    [SerializeField] private GameObject fireEnemyPrefab;
    [SerializeField] private GameObject iceEnemyPrefab;

    [Header("Slider")]
    [SerializeField] private Slider tempSlider;

    public int points;
    int oldPoints;
    public TMP_Text pointsCounter;
    public GameObject pointsCounterInstance;
    public GameObject pointsCounterInstanceParent;
    public TMP_Text pointsAnimText;

    public SpriteRenderer currentPlayerSprite;
    public Sprite iceDeathSprite;
    public Sprite fireDeathSprite;

    [Header("UI Manager")]
    [SerializeField]
    private UiManager uiManager;

    int levelTemperature = 0;

    public float Temperature
    {
        get => _temperature;
        set
        {
            _temperature = value;
            //tempSlider.value = _temperature;

            if (_temperature <= -100)
            {
                if (IsDead)
                    return;
                AudioManager.instance.PlayPlayerDeadFreeze();
                IsDead = true;
                currentPlayerSprite.sprite = iceDeathSprite;                uiManager.ShowGameOverPanel(points);
            
            } else if (_temperature >= 100)
            {
                if (IsDead)
                    return;
                AudioManager.instance.PlayPlayerDeadBurn();
                IsDead = true;
                currentPlayerSprite.sprite = fireDeathSprite;                uiManager.ShowGameOverPanel(points);
            }
            
        }
    }

    [SerializeField] private float _temperature;
    private float _spawnTimer;

    public static bool IsDead;


    [Header("Enemies to spawn")]

    [SerializeField]
    private bool chasing;
    [SerializeField] private bool runningAway;
    [SerializeField] private bool straight;
    //[SerializeField] bool oscilating;

    private readonly bool[] _typeOfEnemies = new bool[3];

    private void Awake()
    {
        Instance = this;
    }

    private bool _canSpawn = false;

    private void Start()
    {
        dificultyLevel = 0;
        levelTemperature = 0;
        IsDead = false;
        _canSpawn = false;

        _typeOfEnemies[0] = chasing;
        _typeOfEnemies[1] = runningAway;
        _typeOfEnemies[2] = straight;
        //typeOfEnemies[3] = oscilating;

        foreach (bool t in _typeOfEnemies)
        {
            if (t) _canSpawn = true;
        }

        if (!_canSpawn)
            Debug.LogError("�ACTIVA ALG�N ENEMIGO A SPAWNEAR EN EL GAME MANAGER!");

    }

    // Update is called once per frame
    void Update()
    {
        if (IsDead) return;

        CheckDificulty();
        CheckTemperatureLevelChange();
        ChangeLight(_temperature);

        if (Mathf.Abs(_temperature) >= 85)
            AlarmLight();
        else if (playerLight.intensity != 1.48f)
            playerLight.intensity = 1.48f;

        SpawnEnemies();
        pointsCounter.text = points.ToString();

        if(points > oldPoints)
        {
            pointsAnimText.text = ("+" + (points - oldPoints).ToString());
            Instantiate(pointsCounterInstance, pointsCounterInstanceParent.transform.position, Quaternion.identity, pointsCounterInstanceParent.transform);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) MenuManager.Instance.ToggleMenu();
    }

    void LateUpdate()
    {
        oldPoints = points;
    }

    void CheckTemperatureLevelChange()
    {

        switch (levelTemperature)
        {

            case 0:

                if (_temperature >= 10)
                {
                    levelTemperature++;
                    AudioManager.instance.PlayTemperatureLevelUp();
                }
                else if (_temperature <= -10)
                {
                    levelTemperature--;
                    AudioManager.instance.PlayTemperatureLevelUp();
                }

                    break;

            case 1:

                if (_temperature >= 45)
                {
                    levelTemperature++;
                    AudioManager.instance.PlayTemperatureLevelUp();
                }
                else if (_temperature < 10)
                {
                    levelTemperature--;
                    AudioManager.instance.PlayTemperatureLevelDown();
                }

                break;

            case 2:

                if (_temperature >= 80)
                {
                    levelTemperature++;
                    AudioManager.instance.PlayTemperatureLevelUp();
                }
                else if (_temperature < 45)
                {
                    levelTemperature--;
                    AudioManager.instance.PlayTemperatureLevelDown();
                }

                break;

            case 3:

                if (_temperature < 80)
                {
                    levelTemperature--;
                    AudioManager.instance.PlayTemperatureLevelDown();
                }

                break;


            case -1:

                if (_temperature <= -45)
                {
                    levelTemperature--;
                    AudioManager.instance.PlayTemperatureLevelUp();
                }
                else if (_temperature > -10)
                {
                    levelTemperature++;
                    AudioManager.instance.PlayTemperatureLevelDown();
                }

                break;

            case -2:

                if (_temperature <= -80)
                {
                    levelTemperature--;
                    AudioManager.instance.PlayTemperatureLevelUp();
                }
                else if (_temperature > -45)
                {
                    levelTemperature++;
                    AudioManager.instance.PlayTemperatureLevelDown();
                }

                break;

            case -3:

                if (_temperature > -80)
                {
                    levelTemperature++;
                    AudioManager.instance.PlayTemperatureLevelDown();
                }

                break;

        }


    }


    private void SpawnEnemies()
    {
        if (_spawnTimer < spawnDelay)
        {
            _spawnTimer += Time.deltaTime;
            return;
        }

        int spawnPointIndex = Random.Range(0, spawnPoints.Count);
        GameObject prefab = Random.Range(0, 2) == 0 ? fireEnemyPrefab : iceEnemyPrefab;

        
        EnemyController.MoveType moveType = (EnemyController.MoveType) Random.Range(0, 3);
        if (_canSpawn)
        {
            while (!_typeOfEnemies[(int)moveType])
            {
                moveType = (EnemyController.MoveType)Random.Range(0, 3);
            }
        }
        StartCoroutine(SpawnAnimation(prefab, spawnPointIndex, moveType));
        /*EnemyController.Spawn(prefab, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex], 
            player, prefab.transform.localScale, moveType); */

        _spawnTimer = 0;
    }

    IEnumerator SpawnAnimation(GameObject prefab, int spawnPointIndex, EnemyController.MoveType moveType)
    {
        if(prefab == fireEnemyPrefab)
            Instantiate(fireSpawnAnimationInstance, spawnPoints[spawnPointIndex].position, Quaternion.identity);
        else if (prefab == iceEnemyPrefab)
            Instantiate(iceSpawnAnimationInstance, spawnPoints[spawnPointIndex].position, Quaternion.identity);

        yield return new WaitForSeconds(1.2f);
        EnemyController.Spawn(prefab, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex],
            player, prefab.transform.localScale, moveType);
    }

    /*
    private void ResetGame()
    {
        _isDead = true;
        SceneManager.LoadScene(0);
        _isDead = false;
    }
    */

    public void ChangeTemperature(float temperature)
    {
        if ((_temperature <= -10 && temperature < 0) || (_temperature >= 10 && temperature > 0))
            temperature *= 2;

        //print(temperature);

        Temperature += temperature;
        StartCoroutine(ChangeSliderValue(temperature));
    }


    private IEnumerator ChangeSliderValue(float temp)
    {
        if (temp < 0)
        {
            for (float value = tempSlider.value; value >= _temperature; value -= 0.1f)
            {
                tempSlider.value = value;
                yield return null;
            }
        }
        else
        {
            for (float value = tempSlider.value; value <= _temperature; value += 0.1f)
            {
                tempSlider.value = value;
                yield return null;
            }
        }
    }

    void ChangeLight(float temperature)
    {

        float colorValue = (temperature + 100)/200;

        playerLight.color = lightGradient.Evaluate(colorValue);

    }

    bool increaseLight = true;

    void AlarmLight()
    {

        if (increaseLight)
        {
            playerLight.intensity = Mathf.Lerp(playerLight.intensity, 3f, 0.02f);
            if (playerLight.intensity >= 2.9f)  increaseLight = false; 
        }
        else
        {
            playerLight.intensity = Mathf.Lerp(playerLight.intensity, 1.48f, 0.02f);
            if (playerLight.intensity <= 1.5f) increaseLight = true; 
        }

    }

    int dificultyLevel = 0;

    void CheckDificulty()
    {

        if (points / 20 > dificultyLevel)
        {
            IncreaseDificulty();
        }

    }

    void IncreaseDificulty()
    {

        if (spawnDelay < 0.1f)
            return;

        dificultyLevel++;
        spawnDelay -= (float)dificultyLevel / 50f;
        print("Spawn Delay: " + spawnDelay);

    }


}
