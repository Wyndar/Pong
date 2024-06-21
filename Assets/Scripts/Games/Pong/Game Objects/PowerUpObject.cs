using UnityEngine;
using UnityEngine.UI;

public class PowerUpObject: MonoBehaviour
{
    public PongManager PongManager;
    public bool isPlayer1;
    public PowerUp powerUp;
    public Image powerUpImage;
    public int powerBarCost;
    public void SetPowerUp(PowerUp powerUpToSet)
    {
        powerUp = powerUpToSet;
        name = powerUp.ToString();
        powerUpImage.sprite = Resources.Load($"Sprites/{powerUp}", typeof(Sprite)) as Sprite;
        switch (powerUp)
        {
            case PowerUp.slow:
            case PowerUp.speed:
            case PowerUp.fastBall:
                powerBarCost = 30;
                break;
            case PowerUp.heal:
            case PowerUp.grow:
                powerBarCost = 50;
                break;
            case PowerUp.shrink:
            case PowerUp.damage:
                powerBarCost = 80;
                break;
            case PowerUp.stun:
            case PowerUp.split:
                powerBarCost = 100;
                break;
        }
    }    
    public void ActivatePowerUp()
    {
        if (isPlayer1)
        {
            if (PongManager.player1PowerBar.PowerPercent >= powerBarCost)
                PongManager.player1PowerBar.PowerPercentChange(powerBarCost, false);
            else
                return;
        }
        else
        {
            if (PongManager.player2PowerBar.PowerPercent >= powerBarCost)
                PongManager.player2PowerBar.PowerPercentChange(powerBarCost, false);
            else
                return;
        }
        switch (powerUp)
        {
            case PowerUp.slow:
                if (isPlayer1)
                    PongManager.player2Paddle.ChangeSpeed(2.5f);
                else
                    PongManager.player1Paddle.ChangeSpeed(2.5f);
                break;
            case PowerUp.speed:
                if (isPlayer1)
                    PongManager.player1Paddle.ChangeSpeed(10);
                else
                    PongManager.player2Paddle.ChangeSpeed(10);
                break;
            case PowerUp.heal:
                PongManager.ScoreChanged(isPlayer1, false, 1);
                break;
            case PowerUp.grow:
                if (isPlayer1)
                    PongManager.player1Paddle.ScaleSize(2);
                else
                    PongManager.player2Paddle.ScaleSize(2);
                break;
            case PowerUp.shrink:
                if (isPlayer1)
                    PongManager.player2Paddle.ScaleSize(0.5f);
                else
                    PongManager.player1Paddle.ScaleSize(0.5f);
                break;
            case PowerUp.damage:
                PongManager.ball.damage += 1;
                break;
            case PowerUp.stun:
                Debug.Log("used stun");
                break;
            case PowerUp.split:
                Debug.Log("used split");
                break;
            case PowerUp.fastBall:
                PongManager.ball.speed = 6;
                break;
        }
        transform.SetParent(null);
        PongManager.SetPowerUps();
        Destroy(gameObject);
    }
}
