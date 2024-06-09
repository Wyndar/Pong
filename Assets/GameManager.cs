using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerHealthText, enemyHealthText;
    [SerializeField] private GameObject playerPaddle, enemyPaddle, ball;
    private int playerHealth, enemyHealth;
    private Ball ballScript;
    // Start is called before the first frame update
    void Start()
    {
        ballScript = ball.GetComponent<Ball>();
        playerHealth = 5; enemyHealth = 5;
        SetScore();
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
        if (playerHealth > 0 && enemyHealth > 0)
        {
            playerPaddle.GetComponent<Paddle>().ResetPosition();
            enemyPaddle.GetComponent<AIPaddle>().ResetPosition();
            StartCoroutine(ball.GetComponent<Ball>().ResetBall());
        }
    }

    private void SetScore()
    {
        playerHealthText.text = playerHealth.ToString();
        enemyHealthText.text = enemyHealth.ToString();
    }
}
