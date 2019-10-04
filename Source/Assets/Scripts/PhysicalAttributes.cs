using System;

[Serializable]
public struct PhysicalAttributes
{
    public float maxHealth;
    public float armor;
    public float attackPower;
    public float attackSpeed;
    public float minRange;
    public float maxRange;
    public int destroyGoldValue;
    public int spawnGoldCosts;
    public int upgradeGoldCosts;
    public float movemenetSpeed;
    public float despawnTime;

    public PhysicalAttributes(float maxHealth, float armor, float attackPower, float attackSpeed, float minRange, float maxRange, int destroyGoldValue, int spawnGoldCosts, int upgradeGoldCosts, float movemenetSpeed, float despawnTime)
    {
        this.maxHealth = maxHealth;
        this.armor = armor;
        this.attackPower = attackPower;
        this.attackSpeed = attackSpeed;
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.destroyGoldValue = destroyGoldValue;
        this.spawnGoldCosts = spawnGoldCosts;
        this.upgradeGoldCosts = upgradeGoldCosts;
        this.movemenetSpeed = movemenetSpeed;
        this.despawnTime = despawnTime;
    }

    public override string ToString()
    {
        return ("maxHealth: " + maxHealth + Environment.NewLine
              + "attackPower: " + attackPower + Environment.NewLine
              + "attackSpeed: " + attackSpeed + Environment.NewLine
              + "armor: " + armor + Environment.NewLine
              + "minRange: " + minRange + Environment.NewLine
              + "maxRange: " + maxRange + Environment.NewLine
              + "destroyGoldValue: " + destroyGoldValue + Environment.NewLine
              + "spawnGoldCosts: " + spawnGoldCosts + Environment.NewLine
              + "upgradeGoldCosts: " + upgradeGoldCosts + Environment.NewLine
              + "movementSpeed: " + movemenetSpeed + Environment.NewLine
              + "despawnTime: " + despawnTime);
    }
}