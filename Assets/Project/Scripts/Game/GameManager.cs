using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] Transform world;
    [SerializeField] GameObject carPrefab;
    [SerializeField] GameObject terrainPrefab;
    [SerializeField] Camera mainCamera;

    [Header("Controllers")]
    [SerializeField] UIManager uiManager;
    [SerializeField] SQLManager sqlManager;

    // Internal object references
    Car car;
    Rigidbody2D carBody;
    SpriteShapeTerrain terrain;

    [Header("Internal Settings")]
    [SerializeField] float scorePerMeter = 1;

    // Game status
    bool isGameRunning;
    float rawScore;
    SessionInfo sessionInfo;
    List<string[]> leaderboard;


    #region MonoBehaviour Callbacks

    // Start is called before the first frame update
    void Start()
    {
        // Initialize GUI
        uiManager.ShowHUD(false);
        uiManager.ShowGameOverPanel(false);
        uiManager.ShowMainPanel(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameRunning)
        {
            float carBodyX = carBody.transform.position.x;
            terrain.UpdateTerrain(carBodyX);
            if (carBodyX > sessionInfo.distanceTravelled)
            {
                // Compute score from last frame
                rawScore += (carBodyX - sessionInfo.distanceTravelled) * scorePerMeter;
                sessionInfo.totalScore = (int)rawScore;
                sessionInfo.distanceTravelled = carBodyX;
            }
            sessionInfo.gameDuration += Time.deltaTime;

            // Update UI
            uiManager.UpdateDistance(sessionInfo.distanceTravelled);
            uiManager.UpdateScore(sessionInfo.totalScore);
        }
    }

    #endregion

    public void StartGame()
    {
        // Create new terreain
        if (terrain != null)
            Destroy(terrain.gameObject);
        terrain = Instantiate(terrainPrefab, world).GetComponent<SpriteShapeTerrain>();
        terrain.RandomizeSeed();

        // Create new car
        if (car != null)
            Destroy(car.gameObject);
        car = Instantiate(carPrefab, world).GetComponent<Car>();
        carBody = car.GetBody();
        car.OnDie = GameOver;
        car.OnBonusCollected = OnBonusCollected;
        car.Initialize();

        mainCamera.GetComponent<FollowTarget>().SetTarget(carBody.transform);

        // Initialize new session info
        rawScore = 0;
        sessionInfo = new SessionInfo();
        sessionInfo.playerName = uiManager.GetNameInputText();
        
        // Update UI
        uiManager.ShowMainPanel(false);
        uiManager.ShowHUD(true);

        isGameRunning = true;
    }

    public void EndGame()
    {
        uiManager.ShowGameOverPanel(false);
        uiManager.ShowHUD(false);
        uiManager.ShowMainPanel(true);
    }

    public void ShowLeaderboard(bool show)
    {
        uiManager.ShowMainPanel(!show);

        if (show)
        {
            leaderboard = sqlManager.GetLeaderboard(10);
            uiManager.SetLeaderboardValues(leaderboard);
        }

        uiManager.ShowLeaderboardPanel(show);
    }

    public void ShowLeaderboardEntryInfo(int index)
    {
        if (index < leaderboard.Count)
        {
            int sessionNumber = int.Parse(leaderboard[index][0]);
            string[] info = sqlManager.GetLeaderboardEntryInfo(sessionNumber);
            uiManager.ShowLeaderboardInfo(index, info);
        }
        else
        {
            Debug.Log($"Index {index} out of range.");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #region Input Callbacks

    void OnGas(InputValue value)
    {
        if (isGameRunning)
            car.Gas(value.isPressed);
    }

    void OnReverse(InputValue value)
    {
        if (isGameRunning)
            car.Reverse(value.isPressed);
    }

    void OnTilt(InputValue value)
    {
        if (isGameRunning)
            car.Tilt(value.Get<float>());
    }

    #endregion

    public void OnBonusCollected(int value)
    {
        sessionInfo.bonusScore += value;
        rawScore += value;
    }

    public void GameOver()
    {
        if (!isGameRunning) return;

        isGameRunning = false;
        
        // Stop car controls
        car.Gas(false);
        car.Reverse(false);
        car.Tilt(0);

        // Add session to database
        sqlManager.AddToSession(sessionInfo);

        uiManager.SetValuesText(sessionInfo.totalScore.ToString() + "\n" +
                                sessionInfo.distanceTravelled.ToString("F2") + "m\n" +
                                TimeSpan.FromSeconds(sessionInfo.gameDuration).ToString("mm':'ss"));
        uiManager.ShowGameOverPanel(true);
    }
}
