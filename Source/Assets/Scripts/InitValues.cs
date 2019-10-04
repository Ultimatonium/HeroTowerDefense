using UnityEngine;
using UnityEngine.SceneManagement;

public class InitValues : MonoBehaviour
{
    public static int soundsPlaying = 0;
    public const int maxSoundsPlaying = 30;

    public const int startGold = 1000;

    public const float upgradeMeleeMinionAttackPower = 1;
    public const float upgradeMeleeMinionHealth = 10;
    public const float upgradeRangeMinionAttackPower = 1;
    public const float upgradeRangeMinionHealth = 10;

    public const float upradeMeleeTowerAttackPower = 10;
    public const float upradeMeleeTowerAttackSpeed = 0.1f;
    public const float upradeMeleeTowerHealth = 150;

    public const float upradeRangeTowerAttackPower = 12;
    public const float upradeRangeTowerAttackSpeed = 0.1f;
    public const float upradeRangeTowerHealth = 100;

    public const float upgradeHeroAttackPower = 5;
    public const float upgradeHeroArmor = 1;
    public const float upgradeHeroAttackSpeed = 0.1f;
    public const float upgradeHeroHealth = 10;

    private void Start()
    {
        soundsPlaying = 0;
        InvokeRepeating("CleanupAudioSources", 1, 3f);
    }

    public static void InitHero(ref PhysicalAttributes refAttributes)
    {
        refAttributes.maxHealth = 500;
        refAttributes.armor = 3;
        refAttributes.attackPower = 50;
        refAttributes.attackSpeed = 1;
        refAttributes.minRange = 1;
        refAttributes.maxRange = 1;
        refAttributes.destroyGoldValue = 50;
        refAttributes.spawnGoldCosts = 100;
        refAttributes.upgradeGoldCosts = 100;
        refAttributes.movemenetSpeed = 11;
        refAttributes.despawnTime = 3;
    }
    public static void InitMeleeMinion(ref PhysicalAttributes refAttributes)
    {
        refAttributes.maxHealth = 100;
        refAttributes.armor = 0;
        refAttributes.attackPower = 10;
        refAttributes.attackSpeed = 1;
        refAttributes.minRange = 0;
        refAttributes.maxRange = 1.5f;
        refAttributes.destroyGoldValue = 50;
        refAttributes.spawnGoldCosts = 10;
        refAttributes.upgradeGoldCosts = 100;
        refAttributes.movemenetSpeed = 11;
        refAttributes.despawnTime = 3;
    }

    public static void InitRangeMinion(ref PhysicalAttributes refAttributes)
    {
        refAttributes.maxHealth = 100;
        refAttributes.armor = 0;
        refAttributes.attackPower = 10;
        refAttributes.attackSpeed = 1;
        refAttributes.minRange = 4;
        refAttributes.maxRange = 8;
        refAttributes.destroyGoldValue = 50;
        refAttributes.spawnGoldCosts = 20;
        refAttributes.upgradeGoldCosts = 100;
        refAttributes.movemenetSpeed = 11;
        refAttributes.despawnTime = 3;
    }

    public static void InitBase(ref PhysicalAttributes refAttributes)
    {
        refAttributes.maxHealth = 10000;
        refAttributes.armor = 0;
        refAttributes.attackPower = 10;
        refAttributes.attackSpeed = 0.5f;
        refAttributes.minRange = 0;
        refAttributes.maxRange = 15;
        refAttributes.destroyGoldValue = 50;
        refAttributes.spawnGoldCosts = 100;
        refAttributes.upgradeGoldCosts = 100;
        refAttributes.movemenetSpeed = 11;
        refAttributes.despawnTime = 0;
    }
    public static void InitRangeTower(ref PhysicalAttributes refAttributes)
    {
        refAttributes.maxHealth = 100;
        refAttributes.armor = 0;
        refAttributes.attackPower = 15;
        refAttributes.attackSpeed = 0.5f;
        refAttributes.minRange = 20;
        refAttributes.maxRange = 50;
        refAttributes.destroyGoldValue = 50;
        refAttributes.spawnGoldCosts = 100;
        refAttributes.upgradeGoldCosts = 100;
        refAttributes.movemenetSpeed = 11;
        refAttributes.despawnTime = 3;
    }

    public static void InitMeleeTower(ref PhysicalAttributes refAttributes)
    {
        refAttributes.maxHealth = 150;
        refAttributes.armor = 0;
        refAttributes.attackPower = 8;
        refAttributes.attackSpeed = 1;
        refAttributes.minRange = 0;
        refAttributes.maxRange = 17;
        refAttributes.destroyGoldValue = 50;
        refAttributes.spawnGoldCosts = 100;
        refAttributes.upgradeGoldCosts = 100;
        refAttributes.movemenetSpeed = 11;
        refAttributes.despawnTime = 3;
    }

    private void LoadScenes()
    {
        SceneManager.LoadSceneAsync("HUD", LoadSceneMode.Additive);
    }

    public static AudioSource PlayLowPrioAudioSource(GameObject gameObject, AudioClip clip, float delay, float timePosition)
    {
        AudioSource lowPrioSource = gameObject.AddComponent<AudioSource>();
        soundsPlaying++;
        lowPrioSource.clip = clip;
        lowPrioSource.priority = 256;
        lowPrioSource.volume = 0.05f - (soundsPlaying * 0.001f);
        lowPrioSource.loop = false;
        if (soundsPlaying <= maxSoundsPlaying)
        {
            lowPrioSource.mute = true;
        }
        else
        {
            lowPrioSource.mute = false;
        }
        lowPrioSource.PlayDelayed(delay);
        return lowPrioSource;
    }

    private void CleanupAudioSources()
    {
        foreach (AudioSource audio in GameObject.FindObjectsOfType<AudioSource>())
        {
            if (audio.loop) continue;
            if (audio.isPlaying) continue;

            Destroy(audio);
            soundsPlaying = Mathf.Max(0, soundsPlaying--);
        }
    }
}

