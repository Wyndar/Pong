using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerHealthText, enemyHealthText, gameOverText, lobbyHeaderText;
    [SerializeField] private GameObject playerPaddlePrefab, AIEnemyPaddlePrefab, localEnemyPrefab, OnlineEnemyPrefab, ballPrefab, lobbyIDPrefab;
    [SerializeField] private GameObject gameOverPanel, startScreenPanel, lobbyPanel, relaySelectPanel, scoreBoard, lobbyClients;
    [SerializeField] private Color winColor, loseColor;

    public RectTransform player1Position, player2Position, offscreenPosition;
    public Paddle player1Paddle = null, player2Paddle = null;
    private int player1Health, player2Health;
    private Ball ball;
    public int PlayerNumber = 1;
    public GameType gameType;
    public void StartGame(string game)
    {
        gameType = Enum.Parse<GameType>(game);
        startScreenPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        ball = Instantiate(ballPrefab).GetComponent<Ball>();
        player1Paddle = Instantiate(playerPaddlePrefab).GetComponent<PlayerPaddle>();
        GameObject e;
        if (gameType == GameType.VSCOM)
            e = Instantiate(AIEnemyPaddlePrefab);
        else
            e = Instantiate(playerPaddlePrefab);
        player2Paddle = e.GetComponent<Paddle>();
        player1Paddle.resetPosition = player1Position.position;
        player2Paddle.resetPosition = player2Position.position;
        player1Paddle.paddleSpeed = 5f;
        player2Paddle.paddleSpeed = 5f;
        player1Health = 5; player2Health = 5;
        scoreBoard.SetActive(true);
        SetScore();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void StartOnlineGameRPC()
    {
        gameType = GameType.VSOnline;
        startScreenPanel.SetActive(false);
        lobbyPanel.SetActive(false);
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
            b.GetComponent<NetworkObject>().Spawn(true);
            ball = b.GetComponent<Ball>();
        }
        player1Paddle.resetPosition = player1Position.position;
        player1Paddle.paddleSpeed = 5f;
        player1Health = 5;
        player2Paddle.resetPosition = player2Position.position;
        player2Paddle.paddleSpeed = 5f;
        player2Health = 5;
        
        scoreBoard.SetActive(true);
        SetScore();
        ResetObjects();
    }

    public void GameOver()
    {
        Destroy(player1Paddle.gameObject);
        Destroy(player2Paddle.gameObject);
        Destroy(ball.gameObject);
        scoreBoard.SetActive(false);    
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<Image>().color = player1Health > 0 ? winColor : loseColor;
        string s = player1Health > 0 ? "You win!" : "You lose.";
        gameOverText.text = $"{s} {System.Environment.NewLine} {System.Environment.NewLine} Touch the screen to play again.";
    }
    
    public void LoadStartScreen()
    {
        gameOverPanel.SetActive(false);
        startScreenPanel.SetActive(true);
    }

    public void ToggleLobby(bool shouldShow)
    {
        lobbyHeaderText.text = IsHost ? "Hosting" : "Joining";
        lobbyPanel.SetActive(shouldShow);
    }
    public void ToggleRelaySelection(bool shouldShow) => relaySelectPanel.SetActive(shouldShow);
    public void LobbyUpdateOnJoin()
    {
        GameObject p = Instantiate(lobbyIDPrefab, lobbyClients.transform);
        PlayerNumber = lobbyClients.transform.childCount;
        p.GetComponentInChildren<Toggle>().isOn = false;
        p.GetComponentInChildren<TMP_Text>().text = PlayerNumber == 1 ? "Player 1 (Host)" : $"Player {PlayerNumber} (Client)";
    }
    public void ScoreChanged(bool isPlayer, bool isDamage, int amount)
    {
        if (isDamage)
            amount = ball.damage;
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
        if (isDamage)
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
    private void SetScore()
    {
        playerHealthText.text = player1Health.ToString();
        enemyHealthText.text = player2Health.ToString();
    }
    public void ExitGame() => Application.Quit();
}
