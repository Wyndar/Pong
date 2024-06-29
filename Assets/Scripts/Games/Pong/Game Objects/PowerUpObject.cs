using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PowerUpObject : MonoBehaviour
{
    public PowerUpManager PowerUpManager;
    public PongManager PongManager;
    public bool isPlayer;
    public PowerUp PowerUpID;
    public string PowerUpName;
    public int PowerBarCost;
    public string PowerUpInfo;
    public bool isLocked;
    public Image powerUpImage;
    public Image powerUpHighlight;
    public void SetHighlightOpacity(bool shouldShow)
    {
        if (shouldShow)
            powerUpHighlight.color = Color.white;
        else
            powerUpHighlight.color = Color.clear;
    }

    public void SetPowerUp(PowerUpData powerUpToSet)
    {
        PowerUpID = powerUpToSet.PowerUpID;
        PowerUpName= powerUpToSet.PowerUpName;
        PowerBarCost= powerUpToSet.PowerBarCost;
        PowerUpInfo = powerUpToSet.PowerUpInfo;
        isLocked = powerUpToSet.IsLocked;
        name = PowerUpName;
        powerUpImage.sprite = Resources.Load($"Sprites/{PowerUpID}", typeof(Sprite)) as Sprite;
        SetHighlightOpacity(false);
    }
    public void ActivatePowerUp()
    {
        if (isPlayer)
        {
            if (PowerUpManager.playerPowerBar.PowerPercent >= PowerBarCost)
                PowerUpManager.playerPowerBar.PowerPercentChange(PowerBarCost, false);
            else
                return;
        }
        else
        {
            if (PowerUpManager.opponentPowerBar.PowerPercent >= PowerBarCost)
                PowerUpManager.opponentPowerBar.PowerPercentChange(PowerBarCost, false);
            else
                return;
        }
        switch (PowerUpID)
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
