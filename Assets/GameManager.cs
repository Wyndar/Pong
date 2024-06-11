using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerHealthText, enemyHealthText, gameOverText;
    [SerializeField] private GameObject playerPaddlePrefab, AIEnemyPaddlePrefab, localEnemyPrefab, OnlineEnemyPrefab, ballPrefab;
    [SerializeField] private GameObject gameOverPanel, startScreenPanel, scoreBoard;
    [SerializeField] private Color winColor, loseColor;

    public GameObject playerPaddle, enemyPaddle, ball;
    private int playerHealth, enemyHealth;
    private Ball ballScript;
    // Start is called before the first frame update
    public void StartGame(GameObject enemyPrefab)
    {
        ball = Instantiate(ballPrefab);
        ballScript = ball.GetComponent<Ball>();
        if (enemyPrefab == OnlineEnemyPrefab)
        {
            playerPaddle.GetComponent<Paddle>().isOnline = true;
            //more work needs to be done here
            return;
        }
        startScreenPanel.SetActive(false);
        enemyPaddle = Instantiate(enemyPrefab);
        playerPaddle = FindObjectOfType<Paddle>().gameObject;
        
        playerHealth = 5; enemyHealth = 5;
        scoreBoard.SetActive(true);
        SetScore();
    }
    public void GameOver()
    {
        Destroy(playerPaddle);
        Destroy(enemyPaddle);
        Destroy(ball);
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
            amount = ballScript.damage;
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
        playerPaddle.GetComponent<Paddle>().ResetPosition();
        if (enemyPaddle.GetComponent<AIPaddle>() != null)
            enemyPaddle.GetComponent<AIPaddle>().ResetPosition();
        else if (enemyPaddle.GetComponent<PaddleSecondary>() != null)
            enemyPaddle.GetComponent<PaddleSecondary>().ResetPosition();
        StartCoroutine(ball.GetComponent<Ball>().ResetBall());

    }

    private void SetScore()
    {
        playerHealthText.text = playerHealth.ToString();
        enemyHealthText.text = enemyHealth.ToString();
    }
    public void ExitGame()=>Application.Quit();
}
