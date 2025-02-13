using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

public class PongManager : GameManager
{
    private const float paddleStartSpeed = 5f;
    private const int startingHP = 8;
    private const string powerUpDataLocation = "Load Data/powerUpInfo";

    [SerializeField] private TMP_Text playerHealthText, opponentHealthText, gameOverText, confirmationPanelText;
    [SerializeField] private TMP_Dropdown gameInputTypeDropdown;
    [SerializeField] private GameObject playerPaddlePrefab, AIEnemyPaddlePrefab, ballPrefab, networkBallPrefab, networkPaddlePrefab, leftBorderPrefab, rightBorderPrefab;
    [SerializeField] private GameObject gameOverPanel, startScreenPanel, settingsPanel, confirmationMessagePanel, leaveGameButton, scoreBoard;
    [SerializeField] private Color winColor, loseColor;
    [SerializeField] private Goal player1Goal, player2Goal;
   
    public RectTransform player1Position, player2Position, offscreenPosition;
    public Paddle player1Paddle = null, player2Paddle = null;
    public NetworkPaddle hostPaddle, clientPaddle;
    public GameType gameType;
    private int playerHealth, opponentHealth;
    private bool hasSetOnlinePowers;
    public List<string> player1Powers, player2Powers;
    public Ball GameBall { get; private set; }
    public PowerUpManager PowerUpManager;

    public override void Awake()
    {
        base.Awake();
        PowerUpManager.powerUpsList.AddRange(SaveManager.LoadPowerUps(powerUpDataLocation));
    }
    public override void StartGame() => StartOnlineGameRPC();
    public void StartGame(string game)
    {
        gameType = Enum.Parse<GameType>(game);
        GameBall = Instantiate(ballPrefab).GetComponent<Ball>();
        player1Paddle = Instantiate(playerPaddlePrefab).GetComponent<PlayerPaddle>();
        GameObject e;
        if (gameType == GameType.VSCOM)
            e = Instantiate(AIEnemyPaddlePrefab);
        else
            e = Instantiate(playerPaddlePrefab);
        PowerUpManager.SetLocalPlayerPowers(PowerUpManager.player1PowerUps, true);
        PowerUpManager.SetLocalPlayerPowers(PowerUpManager.player2PowerUps, false);
        player2Paddle = e.GetComponent<Paddle>();
        OfflineGameSetup();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartOnlineGameRPC()
    {
        gameType = GameType.VSOnline;
        foreach (NetworkPaddle paddle in FindObjectsByType<NetworkPaddle>(FindObjectsSortMode.None))
            paddle.InitializePaddle();
        if (IsHost)
        {
            GameObject b = Instantiate(networkBallPrefab);
            NetworkObject instanceNetworkBall = b.GetComponent<NetworkObject>();
            instanceNetworkBall.Spawn(true);
        }
        else
            Camera.main.transform.rotation = new(0, 0, 180, 0);
        GameBall = FindFirstObjectByType<Ball>();
        StartCoroutine(WaitForPaddlesBeforeSetup());
    }

    [Rpc(SendTo.NotMe)]
    private void SendPowerUpDataRpc(string powers)
    {
        if (IsHost)
        {
            if (player2Powers.Contains(powers)||hasSetOnlinePowers)
                return;
            player2Powers.Add(powers);
            if (player2Powers.Count == 3)
            {
                PowerUpManager.SetOnlinePlayerPowers(player2Powers, PowerUpManager.player2PowerUps);
                PowerUpManager.SetPowerUps(PowerUpManager.opponentPowersPanel, PowerUpManager.player2PowerUps);
                hasSetOnlinePowers = true;
            }
        }
        else
        {
            if (player1Powers.Contains(powers)||hasSetOnlinePowers)
                return;
            player1Powers.Add(powers);
            if (player1Powers.Count == 3)
            {
                PowerUpManager.SetOnlinePlayerPowers(player1Powers, PowerUpManager.player1PowerUps);
                PowerUpManager.SetPowerUps(PowerUpManager.opponentPowersPanel, PowerUpManager.player1PowerUps);
                hasSetOnlinePowers = true;
            }
        } 
    }

    public void GameOver()
    {
        if (player1Paddle != null)
            Destroy(player1Paddle.gameObject);
        if (player2Paddle != null)
            Destroy(player2Paddle.gameObject);
        if(clientPaddle!=null)
            Destroy(clientPaddle.gameObject);
        if(hostPaddle!=null)
            Destroy(hostPaddle.gameObject);
        if (GameBall != null)
            Destroy(GameBall.gameObject);
        scoreBoard.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<Image>().color = playerHealth > 0 ? winColor : loseColor;
        string s = playerHealth > 0 ? "You win!" : "You lose.";
        gameOverText.text = $"{s} {Environment.NewLine} {Environment.NewLine} Touch the screen to play again.";
        Camera.main.transform.rotation = new(0, 0, 0, 0);
        if (Accelerometer.current.enabled)
            InputSystem.DisableDevice(Accelerometer.current);
    }

    public void LoadStartScreen()
    {
        gameOverPanel.SetActive(false);
        startScreenPanel.SetActive(true);
    }

    public void ScoreChanged(bool isPlayer1, bool isDamage, int amount)
    {
        if (isDamage && amount == 0)
            amount = GameBall.damage;
        if (IsHost && gameType == GameType.VSOnline)
        {
            ScoreChangedRPC(isPlayer1, isDamage, amount);
            return;
        }
        if (isPlayer1)
        {
            if (isDamage)
            {
                playerHealth -= amount;
                PowerUpManager.DamageCharge(amount, isPlayer1);
            }
            else
                playerHealth += amount;
        }
        else
        {
            if (isDamage)
            {
                opponentHealth -= amount;
                PowerUpManager.DamageCharge(amount, isPlayer1);
            }
            else
                opponentHealth += amount;
        }
        SetScore();
        if (!isDamage)
            return;
        SetHealth();
        if (playerHealth == 0 || opponentHealth == 0)
            GameOver();
        else
            ResetObjects();
    }

    //DO NOT TOUCH THIS... GRRRR!!!!!! GO AWAY!
    [Rpc(SendTo.ClientsAndHost)]
    public void ScoreChangedRPC(bool isPlayer1, bool isDamage, int amount)
    {
        if ((isPlayer1 && IsHost) || (!isPlayer1 && !IsHost))
        {
            if (isDamage)
            {
                PowerUpManager.DamageCharge(amount, true);
                playerHealth -= amount;
            }
            else
                playerHealth += amount;
        }
        if ((isPlayer1 && !IsHost) || (!isPlayer1 && IsHost))
        {
            if (isDamage)
            {
                PowerUpManager.DamageCharge(amount, false);
                opponentHealth -= amount;
            }
            else
                opponentHealth += amount;
        }
        SetScore();
        if (!isDamage)
            return;
        SetHealth();
        if (playerHealth == 0 || opponentHealth == 0)
            GameOver();
        else if (IsHost)
            ResetObjects();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PowerBarChargeRpc(float amount, bool add, bool isPlayer1)
    {
        if ((isPlayer1 && IsHost) || (!isPlayer1 && !IsHost))
            PowerUpManager.playerPowerBar.PowerPercentChange(amount, add);
        if ((!isPlayer1 && IsHost) || (isPlayer1 && !IsHost))
            PowerUpManager.opponentPowerBar.PowerPercentChange(amount, add);
    }
    private void ResetObjects()
    {
        if (gameType != GameType.VSOnline)
        {
            player1Paddle.ResetPosition();
            player2Paddle.ResetPosition();
        }
        else
        {
            clientPaddle.ResetPosition();
            hostPaddle.ResetPosition();
            if (!IsHost)
                return;
        }
        StartCoroutine(GameBall.ResetBall());
    }

    private void SetScore()
    {
        playerHealthText.text = playerHealth.ToString();
        opponentHealthText.text = opponentHealth.ToString();
    }
    private void SetHealth()
    {
        if (playerHealth < 0)
            playerHealth = 0;
        if (opponentHealth < 0)
            opponentHealth = 0;
    }
    
    public void ExitGameCheck()
    {
        confirmationMessagePanel.SetActive(true);
        confirmationPanelText.text = $"Are you sure you want to leave this game? {Environment.NewLine}" +
            $" (Leaving this game will count as a loss.)";
    }
    public void ConfirmExit(bool shouldExit)
    {
        confirmationMessagePanel.SetActive(false);
        if (shouldExit)
        {
            if (gameType != GameType.VSOnline)
            {
                player1Paddle.gameObject.SetActive(false);
                player2Paddle.gameObject.SetActive(false);
            }
            else
            {
                clientPaddle.gameObject.SetActive(false);
                hostPaddle.gameObject.SetActive(false);
            }
            GameBall.gameObject.SetActive(false);
            LoadScene(1);
        }
    }

    public void ToggleSettingsPanel(bool shouldShow) => settingsPanel.SetActive(shouldShow);

    public void SetGameInputType()=> gameInputType = Enum.Parse<InputType>(gameInputTypeDropdown.value.ToString());
    private void OfflineGameSetup()
    {
        player1Paddle.hasGameStarted = true;
        player2Paddle.hasGameStarted = true;
        player1Paddle.resetPosition = player1Position.position;
        player2Paddle.resetPosition = player2Position.position;
        player1Paddle.ChangeSpeed(paddleStartSpeed);
        player2Paddle.ChangeSpeed(paddleStartSpeed);
        player1Paddle.SetColor(Color.blue);
        player2Paddle.SetColor(Color.red);
        player1Paddle.name = "Blue Player";
        player2Paddle.name = "Red Player";
        GeneralGameSetup();
    }
    private IEnumerator WaitForPaddlesBeforeSetup()
    {
        float timeout = 5f;
        float elapsedTime = 0f;

        while ((hostPaddle == null || clientPaddle == null) && elapsedTime < timeout)
        {
            yield return new WaitForSeconds(0.2f);
            elapsedTime += 0.1f;
        }

        if (hostPaddle != null && clientPaddle != null)
        {
            Debug.Log("Paddles initialized! Proceeding with OnlineGameSetup...");
            OnlineGameSetup();
        }
        else
            Debug.Log("Paddles were not set in time! Multiplayer might be broken.");
        yield break;
    }
    private void OnlineGameSetup()
    {
        hostPaddle.resetPosition = player1Position.position;
        clientPaddle.resetPosition = player2Position.position;
        hostPaddle.ChangeSpeed(paddleStartSpeed);
        clientPaddle.ChangeSpeed(paddleStartSpeed);
        hostPaddle.SetColor(Color.blue);
        clientPaddle.SetColor(Color.red);
        hostPaddle.name = "Blue Player";
        clientPaddle.name = "Red Player";
        GeneralGameSetup();
        if (IsHost)
        {
            PowerUpManager.SetLocalPlayerPowers(PowerUpManager.player1PowerUps, true);
            hostPaddle.powerBar = PowerUpManager.playerPowerBar;
            clientPaddle.powerBar = PowerUpManager.opponentPowerBar;
            PowerUpManager.SetPowerUps(PowerUpManager.playerPowersPanel, PowerUpManager.player1PowerUps);
        }
        else
        {
            PowerUpManager.SetLocalPlayerPowers(PowerUpManager.player2PowerUps, true);
            clientPaddle.powerBar = PowerUpManager.playerPowerBar;
            hostPaddle.powerBar = PowerUpManager.opponentPowerBar;
            PowerUpManager.SetPowerUps(PowerUpManager.playerPowersPanel, PowerUpManager.player2PowerUps);
        }
        List<string> powers = PowerUpManager.SendOwnerStringData(PowerUpManager.ownersCurrentlyActivePowersUps);
        foreach (string power in powers)
            SendPowerUpDataRpc(power);
        hostPaddle.hasGameStarted = true;
        clientPaddle.hasGameStarted = true;
    }
    private void GeneralGameSetup()
    {
        if (gameInputType != InputType.Touchscreen)
            InputSystem.EnableDevice(Accelerometer.current);
        startScreenPanel.SetActive(false);
        playerHealth = startingHP;
        opponentHealth = startingHP;
        scoreBoard.SetActive(true);
        leaveGameButton.SetActive(true);
        SetScore();
        ResetObjects();
        PowerUpManager.ToggleUI(true);
        PowerUpManager.PowerUpSetup();
    }
}

