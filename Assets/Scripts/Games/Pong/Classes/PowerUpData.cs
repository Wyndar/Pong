[System.Serializable]
public class PowerUpData
{
    public PowerUp PowerUpID;
    public string PowerUpName;
    public int PowerBarCost;
    public string PowerUpInfo;
    public bool IsLocked;
    public PowerUpData(PowerUp power, string name, int cost, string info, bool locked)
    {
        PowerUpID = power;
        PowerUpName = name;
        PowerBarCost = cost;
        PowerUpInfo = info;
        IsLocked = locked;
    }
}
