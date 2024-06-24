using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class PongManager : GameManager
{
    [SerializeField] private TMP_Text playerHealthText, opponentHealthText, gameOverText;
    [SerializeField] private GameObject playerPaddlePrefab, AIEnemyPaddlePrefab, ballPrefab;
    [SerializeField] private GameObject gameOverPanel, startScreenPanel, scoreBoard;
    [SerializeField] private Color winColor, loseColor;
    [SerializeField] private Goal player1Goal, player2Goal;

    public RectTransform player1Position, player2Position, offscreenPosition;
    public Paddle player1Paddle = null, player2Paddle = null;
    public GameType gameType;
    private int playerHealth, opponentHealth;
    private bool hasSetOnlinePowers;
    public List<string> player1Powers, player2Powers;
    public Ball GameBall { get; private set; }
    public PowerUpManager PowerUpManager;

    public override void StartGame() => StartOnlineGameRPC();
    public void StartGame(string game)
    {
        FindObjectOfType<NetworkManager>().enabled = false;
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
        GameSetup();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartOnlineGameRPC()
    {
        gameType = GameType.VSOnline;
        PlayerPaddle[] paddles = FindObjectsOfType<PlayerPaddle>();
        foreach (PlayerPaddle paddle in paddles)
        {
            if (paddle.clientID == 0)
                player1Paddle = paddle;
            if (paddle.clientID == 1)
                player2Paddle = paddle;
        }
        if (IsHost)
        {
            GameObject b = Instantiate(ballPrefab);
            NetworkObject instanceNetworkBall = b.GetComponent<NetworkObject>();
            instanceNetworkBall.Spawn(true);
        }
        else
            Camera.main.transform.rotation = new(0, 0, 180, 0);
        GameBall = FindObjectOfType<Ball>();
        GameSetup();
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
        if (GameBall != null)
            Destroy(GameBall.gameObject);
        scoreBoard.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<Image>().color = playerHealth > 0 ? winColor : loseColor;
        string s = playerHealth > 0 ? "You win!" : "You lose.";
        gameOverText.text = $"{s} {Environment.NewLine} {Environment.NewLine} Touch the screen to play again.";
        Camera.main.transform.rotation = new(0, 0, 0, 0);
    }

    public void LoadStartScreen()
    {
        gameOverPanel.SetActive(false);
        startScreenPanel.SetActive(true);
    }

    public void ScoreChanged(bool isPlayer, bool isDamage, int amount)
    {
        if (isDamage && amount == 0)
            amount = GameBall.damage;
        if (IsHost && gameType == GameType.VSOnline)
        {
            ScoreChangedRPC(isPlayer, isDamage, amount);
            return;
        }   
        if (isPlayer)
        {
            if (isDamage)
            {
                playerHealth -= amount;
                PowerUpManager.DamageCharge(amount, isPlayer);
            }
            else
                playerHealth += amount;
        }
        else
        {
            if (isDamage)
            {
                playerHealth -= amount;
                PowerUpManager.DamageCharge(amount, isPlayer);
            }
            else
                playerHealth += amount;
        }
        if (!isDamage)
            return;
        if (playerHealth < 0)
            playerHealth = 0;
        if (opponentHealth < 0)
            opponentHealth = 0;
        SetScore();
        if (playerHealth == 0 || opponentHealth == 0)
        {
            GameOver();
            return;
        }
        if (!IsHost && gameType == GameType.VSOnline)
            return;
        ResetObjects();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ScoreChangedRPC(bool isPlayer1, bool isDamage, int amount)
    {
        if ((isPlayer1 && IsHost) || (!isPlayer1 && !IsHost))
        {
            if (isDamage)
                playerHealth -= amount;
            else
                playerHealth += amount;
        }
        if((isPlayer1 && !IsHost) || (!isPlayer1 && IsHost)) 
        {
            if (isDamage)
                opponentHealth -= amount;
            else
                opponentHealth += amount;
        }
        if (playerHealth < 0)
            playerHealth = 0;
        if (opponentHealth < 0)
            opponentHealth = 0;
        SetScore();
        if (playerHealth == 0 || opponentHealth == 0)
        {
            GameOver();
            return;
        }
        if (IsHost && isDamage)
            ResetObjects();
    }
    private void ResetObjects()
    {
        player1Paddle.ResetPosition();
        player2Paddle.ResetPosition();
        if (gameType == GameType.VSOnline && !IsHost)
            return;
        StartCoroutine(GameBall.ResetBall());
    }

    public void SetScore()
    {
        playerHealthText.text = playerHealth.ToString();
        opponentHealthText.text = opponentHealth.ToString();
    }
   
    private void GameSetup()
    {
        startScreenPanel.SetActive(false);
        player1Paddle.hasGameStarted = true;
        player2Paddle.hasGameStarted = true;
        player1Paddle.resetPosition = player1Position.position;
        player2Paddle.resetPosition = player2Position.position;
        player1Paddle.ChangeSpeed(5f);
        player2Paddle.ChangeSpeed(5f);
        playerHealth = 8; opponentHealth = 8;
        player1Paddle.SetColor(Color.blue);
        player2Paddle.SetColor(Color.red);
        player1Paddle.name = "Blue Player";
        player2Paddle.name = "Red Player";
        scoreBoard.SetActive(true);
        SetScore();
        ResetObjects();
        PowerUpManager.ToggleUI(true);
        PowerUpManager.PowerUpSetup();
        if (IsHost)
        {
            PowerUpManager.SetLocalPlayerPowers(PowerUpManager.player1PowerUps, true);
            PowerUpManager.SetPowerUps(PowerUpManager.playerPowersPanel, PowerUpManager.player1PowerUps);
        }
        else
        {
            PowerUpManager.SetLocalPlayerPowers(PowerUpManager.player2PowerUps, true);
            PowerUpManager.SetPowerUps(PowerUpManager.playerPowersPanel, PowerUpManager.player2PowerUps);
        }
        List<string> powers = PowerUpManager.SendOwnerStringData(PowerUpManager.ownersCurrentlyActivePowersUps);
            foreach (string power in powers)
                SendPowerUpDataRpc(power);
    }
}

