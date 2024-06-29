using UnityEngine;
using UnityEngine.UI;

public class PowerUpObject: MonoBehaviour
{
    public PowerUpManager PowerUpManager;
    public PongManager PongManager;
    public bool isPlayer;
    public PowerUp powerUp;
    public Image powerUpImage;
    public Image powerUpHighlight;
    public int powerBarCost;

    public void SetHighlightOpacity(bool shouldShow)
    {
        if (shouldShow)
            powerUpHighlight.color = Color.white;
        else
            powerUpHighlight.color = Color.clear;
    }

    public void SetPowerUp(PowerUp powerUpToSet)
    {
        powerUp = powerUpToSet;
        name = powerUp.ToString();
        powerUpImage.sprite = Resources.Load($"Sprites/{powerUp}", typeof(Sprite)) as Sprite;
        SetHighlightOpacity(false);
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
            case PowerUp.magnet:
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
        if (isPlayer)
        {
            if (PowerUpManager.playerPowerBar.PowerPercent >= powerBarCost)
                PowerUpManager.playerPowerBar.PowerPercentChange(powerBarCost, false);
            else
                return;
        }
        else
        {
            if (PowerUpManager.opponentPowerBar.PowerPercent >= powerBarCost)
                PowerUpManager.opponentPowerBar.PowerPercentChange(powerBarCost, false);
            else
                return;
        }
        switch (powerUp)
        {
            case PowerUp.slow:
                SlowEnemyPaddle();
                break;
            case PowerUp.speed:
                SpeedUpPlayerPaddle();
                break;
            case PowerUp.heal:
                PongManager.ScoreChanged(isPlayer, false, 1);
                break;
            case PowerUp.grow:
                GrowPlayerPaddle();
                break;
            case PowerUp.shrink:
                ShrinkEnemyPaddle();
                break;
            case PowerUp.damage:
                PongManager.GameBall.damage += 1;
                break;
            case PowerUp.stun:
                Debug.Log("used stun");
                break;
            case PowerUp.split:
                Debug.Log("used split");
                break;
            case PowerUp.fastBall:
                PongManager.GameBall.speed = 6;
                break;
            case PowerUp.magnet:
                Debug.Log("used magnet");
                break;
        }
        transform.SetParent(null, false);
        PowerUpManager.SetPowerUps(isPlayer);
        Destroy(gameObject);
    }

    private void SlowEnemyPaddle()
    {
        if (PongManager.gameType != GameType.VSOnline)
        {
            if (isPlayer)
                PongManager.player2Paddle.ChangeSpeed(2.5f);
            else
                PongManager.player1Paddle.ChangeSpeed(2.5f);
        }
        else
        {
            if (isPlayer)
                PongManager.clientPaddle.ChangeSpeed(2.5f);
            else
                PongManager.hostPaddle.ChangeSpeed(2.5f);
        }
    }
    private void SpeedUpPlayerPaddle()
    {
        if (PongManager.gameType != GameType.VSOnline)
        {
            if (isPlayer)
                PongManager.player1Paddle.ChangeSpeed(10);
            else
                PongManager.player2Paddle.ChangeSpeed(10);
        }
        else
        {
            if (isPlayer)
                PongManager.hostPaddle.ChangeSpeed(10);
            else
                PongManager.clientPaddle.ChangeSpeed(10);
        }
    }
    private void ShrinkEnemyPaddle()
    {
        if (PongManager.gameType != GameType.VSOnline)
        {
            if (isPlayer)
                PongManager.player2Paddle.ScaleSize(0.5f);
            else
                PongManager.player1Paddle.ScaleSize(0.5f);
        }
        else
        {
            if (isPlayer)
                PongManager.clientPaddle.ScaleSize(0.5f);
            else
                PongManager.hostPaddle.ScaleSize(0.5f);
        }
    }
    private void GrowPlayerPaddle()
    {
        if (PongManager.gameType != GameType.VSOnline)
        {
            if (isPlayer)
                PongManager.player1Paddle.ScaleSize(2);
            else
                PongManager.player2Paddle.ScaleSize(2);
        }
        else
        {
            if (isPlayer)
                PongManager.hostPaddle.ScaleSize(2);
            else
                PongManager.clientPaddle.ScaleSize(2);
        }
    }
    public void ToggleSelection() => PowerUpManager.PowerUpAddOrRemove(this);
}
