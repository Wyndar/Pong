using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerHealthText, enemyHealthText, gameOverText;
    [SerializeField] private GameObject playerPaddlePrefab, AIEnemyPaddlePrefab, localEnemyPrefab, OnlineEnemyPrefab, ballPrefab;
    [SerializeField] private GameObject gameOverPanel, startScreenPanel, scoreBoard;
    [SerializeField] private Color winColor, loseColor;

    public RectTransform player1Position, player2Position;
    public Paddle playerPaddle, enemyPaddle;
    private int playerHealth, enemyHealth;
    private Ball ball;
    public GameType gameType;
    public void StartGame(GameObject enemyPrefab)
    {
        GameObject b = Instantiate(ballPrefab);
        ball = b.GetComponent<Ball>();
        if (enemyPrefab == OnlineEnemyPrefab)
            gameType = GameType.VSOnline;
        else if(enemyPrefab == localEnemyPrefab)
            gameType = GameType.VSLocal;
        else
            gameType = GameType.VSCOM;
        startScreenPanel.SetActive(false);
        GameObject p = Instantiate(playerPaddlePrefab);
        playerPaddle = p.GetComponent<Paddle>();
        GameObject e = Instantiate(enemyPrefab);
        enemyPaddle = e.GetComponent<Paddle>();
        playerPaddle.resetPosition = player1Position.position;
        enemyPaddle.resetPosition = player2Position.position;
        playerPaddle.ResetPosition();
        enemyPaddle.ResetPosition();
        playerPaddle.paddleSpeed = 5f;
        enemyPaddle.paddleSpeed = 5f;
        playerHealth = 5; enemyHealth = 5;
        scoreBoard.SetActive(true);
        SetScore();
        Reset();
    }
    public void GameOver()
    {
        Destroy(playerPaddle.gameObject);
        Destroy(enemyPaddle.gameObject);
        Destroy(ball.gameObject);
        scoreBoard.SetActive(false);    
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<Image>().color = playerHealth > 0 ? winColor : loseColor;
        string s = playerHealth > 0 ? "You win!" : "You lose.";
        gameOverText.text = $"{s} {System.Environment.NewLine} {System.Environment.NewLine} Touch the screen to play again.";
    }

    public void LoadStartScreen()
    {
        gameOverPanel.SetActive(false);
        startScreenPanel.SetActive(true);
    }
    public void ScoreChanged(bool isPlayer, bool isDamage, int amount)
    {
        if (isDamage)
            amount = ball.damage;
        if (isPlayer)
        {
            if (isDamage)
                playerHealth -= amount;
            else
                playerHealth += amount;
        }
        else
        {
            if (isDamage)
                enemyHealth -= amount;
            else
                enemyHealth += amount;
        }
        if (playerHealth < 0)
            playerHealth = 0;
        if (enemyHealth < 0)
            enemyHealth = 0;
        SetScore();
        if (playerHealth == 0 || enemyHealth == 0)
        {
            GameOver();
            return;
        }
        if (isDamage)
            Reset();
    }
    private void Reset()
    {
        playerPaddle.ResetPosition();
        enemyPaddle.ResetPosition();
        StartCoroutine(ball.ResetBall());
    }
    private void SetScore()
    {
        playerHealthText.text = playerHealth.ToString();
        enemyHealthText.text = enemyHealth.ToString();
    }
    public void ExitGame() => Application.Quit();
}
