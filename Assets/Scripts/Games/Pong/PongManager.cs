using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using System;

public class PongManager : GameManager
{
    [SerializeField] private TMP_Text playerHealthText, enemyHealthText, gameOverText;
    [SerializeField] private GameObject playerPaddlePrefab, AIEnemyPaddlePrefab, ballPrefab;
    [SerializeField] private GameObject gameOverPanel, startScreenPanel, scoreBoard;
    [SerializeField] private Color winColor, loseColor;

    public RectTransform player1Position, player2Position, offscreenPosition;
    public Paddle player1Paddle = null, player2Paddle = null;
    private int player1Health, player2Health;
    private Ball ball;
    public int PlayerNumber = 1;
    public GameType gameType;

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
        SetPositionSpeedAndUI();
        scoreBoard.SetActive(true);
        SetScore();
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
        SetPositionSpeedAndUI();
        scoreBoard.SetActive(true);
        SetScore();
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
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<Image>().color = player1Health > 0 ? winColor : loseColor;
        string s = player1Health > 0 ? "You win!" : "You lose.";
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
            amount = ball.damage;
        if(IsHost&&gameType==GameType.VSOnline)
        {
            ScoreChangedRPC(isPlayer, isDamage, amount);
            return;
        }    
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
        if (!IsHost && gameType == GameType.VSOnline)
            return;
        if (isDamage)
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
        playerHealthText.text = player1Health.ToString();
        enemyHealthText.text = player2Health.ToString();
    }

    //paddles should set their own speed, please change later
    private void SetPositionSpeedAndUI()
    {
        startScreenPanel.SetActive(false);
        player1Paddle.hasGameStarted = true;
        player2Paddle.hasGameStarted = true;
        player1Paddle.resetPosition = player1Position.position;
        player2Paddle.resetPosition = player2Position.position;
        player1Paddle.paddleSpeed = 5f;
        player2Paddle.paddleSpeed = 5f;
        player1Health = 5; player2Health = 5;
        player1Paddle.SetColor(Color.blue);
        player2Paddle.SetColor(Color.red);
        player1Paddle.name = "Blue Player";
        player2Paddle.name = "Red Player";
    }
}

