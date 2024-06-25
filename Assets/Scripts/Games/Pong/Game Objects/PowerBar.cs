using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PowerBar : MonoBehaviour
{
    [SerializeField] private PowerUpManager powerUpManager;
    [SerializeField] private TMP_Text powerPercentText;
    [SerializeField] private Slider powerBar, overchargeBar;
    [SerializeField] private bool isPlayer;
    public float PowerPercent { get; private set; }

    public void SetPowerPercent(int amount)
    {
        PowerPercent = amount;
        PowerBarUpdate();
    }
    public void PowerPercentChange(float amount, bool add)
    {
        if (add)
            PowerPercent += amount;
        else
            PowerPercent -= amount;
        if(PowerPercent < 0)
            PowerPercent = 0;
        if(PowerPercent > 200)
            PowerPercent = 200;
        PowerBarUpdate();
    }
    private void PowerBarUpdate()
    {
        ShowValidPowerUps();
        overchargeBar.gameObject.SetActive(PowerPercent > 100);
        powerPercentText.text = $"{Mathf.FloorToInt(PowerPercent)}%";
        if (PowerPercent <= 100)
            powerBar.value = PowerPercent/100;
        else
            overchargeBar.value = (PowerPercent - 100)/100;
    }

    private void ShowValidPowerUps()
    {
        if (isPlayer)
        {
            foreach (Transform powerUpTranform in powerUpManager.playerPowersPanel.transform)
            {
                PowerUpObject powerUp = powerUpTranform.GetComponent<PowerUpObject>();
                powerUp.SetHighlightOpacity(powerUp.powerBarCost <= PowerPercent);
            }
        }
        else
        {
            foreach (Transform powerUpTranform in powerUpManager.opponentPowersPanel.transform)
            {
                PowerUpObject powerUp = powerUpTranform.GetComponent<PowerUpObject>();
                powerUp.SetHighlightOpacity(powerUp.powerBarCost <= PowerPercent);
            }
        }
    }
}
