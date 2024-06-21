using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class PongManager : GameManager
{
    [SerializeField] private TMP_Text player1HealthText, player2HealthText, gameOverText;
    [SerializeField] private GameObject playerPaddlePrefab, AIEnemyPaddlePrefab, ballPrefab, powerUpPrefab;
    [SerializeField] private GameObject gameOverPanel, startScreenPanel, scoreBoard, player1PowerBarsPanel, player2PowerBarsPanel,
        player1PowersPanel,player2PowersPanel;
    [SerializeField] private Color winColor, loseColor;

    public List<PowerUp> player1PowerUps, player2PowerUps;
    public PowerBar player1PowerBar, player2PowerBar;
    public RectTransform player1Position, player2Position, offscreenPosition;
    public Paddle player1Paddle = null, player2Paddle = null;
    public int playerNumber = 1;
    public GameType gameType;
    private int player1Health, player2Health;
    public Ball ball { get; private set; }

    public override void StartGame() => StartOnlineGameRPC();
    public void StartGame(string game)
    {
        gameType = Enum.Parse<GameType>(game);
        ball = Instantiate(ballPrefab).GetComponent<Ball>();
        player1Paddle = Instantiate(playerPaddlePrefab).GetComponent<PlayerPaddle>();
        GameObject e;
        if (gameType == GameType.VSCOM)
            e = Instantiate(AIEnemyPaddlePrefab);
        else
            e = Instantiate(playerPaddlePrefab);
        player2Paddle = e.GetComponent<Paddle>();
        GameSetup();
        scoreBoard.SetActive(true);
        SetScore();
        RandomPowerUpAdd();
        SetPowerUps();
        ResetObjects();
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
            var b = Instantiate(ballPrefab);
            var instanceNetworkBall = b.GetComponent<NetworkObject>();
            instanceNetworkBall.Spawn(true);
        }
        else
            Camera.main.transform.rotation = new (0, 0, 180, 0); 
        ball = FindObjectOfType<Ball>();
        GameSetup();
        scoreBoard.SetActive(true);
        SetScore();
        player1PowerBar.SetPowerPercent(0);
        ResetObjects();
    }

    public void GameOver()
    {
        if (player1Paddle != null)
            Destroy(player1Paddle.gameObject);
        if (player2Paddle != null)
            Destroy(player2Paddle.gameObject);
        if (ball != null)
            Destroy(ball.gameObject);
        scoreBoard.SetActive(false);
        player1PowerBarsPanel.SetActive(false);
        player2PowerBarsPanel.SetActive(false);
        player1PowersPanel.SetActive(false);
        player2PowersPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<Image>().color = player1Health > 0 ? winColor : loseColor;
        string s = player1Health > 0 ? "Blue wins!" : "Red wins!";
        gameOverText.text = $"{s} {Environment.NewLine} {Environment.NewLine} Touch the screen to play again.";
        Camera.main.transform.rotation = new(0, 0, 0, 0);
    }

    public void LoadStartScreen()
    {
        gameOverPanel.SetActive(false);
        startScreenPanel.SetActive(true);
    }

    public void ScoreChanged(bool isPlayer1, bool isDamage, int amount)
    {
        if (isDamage && amount == 0)
            amount = ball.damage;
        if(IsHost&&gameType==GameType.VSOnline)
        {
            ScoreChangedRPC(isPlayer1, isDamage, amount);
            return;
        }    
        if (isPlayer1)
        {
            if (isDamage)
            {
                player1Health -= amount;
                player1PowerBar.PowerPercentChange(amount * 25, true);
                player2PowerBar.PowerPercentChange(amount * 5, true);
            }
            else
                player1Health += amount;
        }
        else
        {
            if (isDamage)
            {
                player1PowerBar.PowerPercentChange(amount * 5, true);
                player2PowerBar.PowerPercentChange(amount * 25, true);
                player2Health -= amount;
            }
            else
                player2Health += amount;
        }
        if (!isDamage)
            return;
        if (player1Health < 0)
            player1Health = 0;
        if (player2Health < 0)
            player2Health = 0;
        SetScore();
        if (player1Health == 0 || player2Health == 0)
        {
            GameOver();
            return;
        }
        if (!IsHost && gameType == GameType.VSOnline)
            return;
        ResetObjects();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ScoreChangedRPC(bool isPlayer, bool isDamage, int amount)
    {
        if (isPlayer)
        {
            if (isDamage)
                player1Health -= amount;
            else
                player1Health += amount;
        }
        else
        {
            if (isDamage)
                player2Health -= amount;
            else
                player2Health += amount;
        }
        if (player1Health < 0)
            player1Health = 0;
        if (player2Health < 0)
            player2Health = 0;
        SetScore();
        if (player1Health == 0 || player2Health == 0)
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
        StartCoroutine(ball.ResetBall());
    }

    public void SetScore()
    {
        player1HealthText.text = player1Health.ToString();
        player2HealthText.text = player2Health.ToString();
    }
    public void RandomPowerUpAdd()
    {
        List<PowerUp> powers = new();
        foreach (PowerUp powerUp in Enum.GetValues(typeof(PowerUp)))
            powers.Add(powerUp);
        player1PowerUps.Clear();
        player2PowerUps.Clear();
        while (player2PowerUps.Count < 6)
        {
            int x = UnityEngine.Random.Range(0, powers.Count);
            player2PowerUps.Add(powers[x]);
            player1PowerUps.Add(powers[x]);
            powers.RemoveAt(x);
        }
    } 
    public void SetPowerUps()
    {
        player1PowersPanel.SetActive(true);
        player2PowersPanel.SetActive(true);
        while (player1PowersPanel.transform.childCount < 3)
        {
            if (player1PowerUps.Count == 0)
                break;
            int x = UnityEngine.Random.Range(0, player1PowerUps.Count);
            GameObject g = Instantiate(powerUpPrefab, player1PowersPanel.transform);
            PowerUpObject p = g.GetComponent<PowerUpObject>();
            p.PongManager = this;
            p.isPlayer1 = true;
            p.SetPowerUp(player1PowerUps[x]);
            player1PowerUps.RemoveAt(x);
        }
        while (player2PowersPanel.transform.childCount < 3)
        {
            if (player2PowerUps.Count == 0)
                break;
            int x = UnityEngine.Random.Range(0, player2PowerUps.Count);
            GameObject g = Instantiate(powerUpPrefab, player2PowersPanel.transform);
            PowerUpObject p = g.GetComponent<PowerUpObject>();
            p.PongManager = this;
            p.isPlayer1 = false;
            p.SetPowerUp(player2PowerUps[x]);
            player2PowerUps.RemoveAt(x);    
        }
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
        player1Health = 5; player2Health = 5;
        player1Paddle.SetColor(Color.blue);
        player2Paddle.SetColor(Color.red);
        player1Paddle.name = "Blue Player";
        player2Paddle.name = "Red Player";
        player1PowerBarsPanel.SetActive(true);
        player1PowerBar.SetPowerPercent(0);
        player2PowerBarsPanel.SetActive(true);
        player2PowerBar.SetPowerPercent(0);
        player1Paddle.powerBar = player1PowerBar;
        player2Paddle.powerBar = player2PowerBar;
    }
}

