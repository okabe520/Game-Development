using UnityEngine;
using System.Collections.Generic;

public class Monster : MonoBehaviour
{
    private BattleLog battleLog;

    // 定义 BurnInfo 结构
    public struct BurnInfo
    {
        public float damage;
        public int turnsRemaining;
    }

    // 初始属性值常量
    private const float INITIAL_HEALTH = 400f;

    // 怪物属性
    public float health;
    public float maxHealth;
    public float attackPower = 15f;
    public float defense = 3f;
    public float speed = 4f;
    public string monsterName = "骷髅战士";
    public int bossIndex = 0;  // 0=普通怪, 1-5=Boss编号

    [SerializeField] private Animator monsterAnimator;

    [HideInInspector] public Hero hero; 

    // 技能相关
    private bool isChargingPower = false;
    private float chargedAttackMultiplier = 2.5f;

    // 新增属性来跟踪虚弱效果
    private int weakenTurnsRemaining = 0;
    private float originalAttackPower;
    private bool isWeakened = false;
    private bool weakenJustEnded = false;

    // 新增属性来跟踪燃烧效果
    private const int BURN_DURATION = 3;
    private const float BURN_DAMAGE = 15f;
    private int burnTurnsRemaining = 0;
    private bool burnJustEnded = false;

    // 添加私有属性来跟踪冰冻状态
    private bool isFrozen = false;

    // 添加公共属性来获取冰冻状态
    public bool IsFrozen
    {
        get { return isFrozen; }
        set { isFrozen = value; }
    }

    // 新增应用冰冻效果的方法
    public void ApplyFreeze()
    {
        if (!isFrozen)
        {
            isFrozen = true;
        }
    }

    // 修改 lastAction 属性
    public string lastAction { get; protected set; }
    public string lastActionDescription { get; protected set; } // 新添加的属性

    protected virtual void Start()
    {
        if (maxHealth == 0)
        {
            // 如果子类没有设置 maxHealth，使用默认值
            maxHealth = INITIAL_HEALTH;
        }
        if (health == 0)
        {
            health = maxHealth;
        }
    }

    // 修改 DecideAction 方法
    public virtual float DecideAction(Hero hero)
    {
        if (isFrozen)
        {
            lastAction = "Frozen";
            return 0;
        }

        float damage = 0;

        if (health <= maxHealth * 0.5f)
        {
            // 血量低于50%时的行动逻辑
            if (!isChargingPower)
            {
                float random = Random.value;
                if (random < 0.2f) // 20%的概率使用蓄力
                {
                    ChargePower();
                    lastAction = "ChargePower";
                }
                else if (random < 0.4f) // 20%的概率使用猛击
                {
                    damage = HeavyStrike(hero);
                    lastAction = "HeavyStrike";
                }
                else if (random < 0.6f) // 20%的概率使用五月雨斩
                {
                    damage = MayRainSlash(hero);
                    lastAction = "MayRainSlash";
                }
                else // 40%的概率使用普通攻击
                {
                    damage = Attack(hero);
                    lastAction = "Attack";
                }
            }
            else
            {
                // 如果处于蓄力状态，必定使用普通攻击释放蓄力
                damage = Attack(hero);
                lastAction = "Attack";
            }
        }
        else
        {
            // 血量高于50%时的行动逻辑
            if (isChargingPower)
            {
                damage = Attack(hero);
                lastAction = "Attack";
            }
            else
            {
                float random = Random.value;
                if (random < 0.2f) // 20%的概率使用猛击
                {
                    damage = HeavyStrike(hero);
                    lastAction = "HeavyStrike";
                }
                else if (random < 0.4f) // 20%的概率使用五月雨斩
                {
                    damage = MayRainSlash(hero);
                    lastAction = "MayRainSlash";
                }
                else // 60%的概率使用普通攻击
                {
                    damage = Attack(hero);
                    lastAction = "Attack";
                }
            }
        }

        return damage;
    }

    // 修改攻击方法
    public virtual float Attack(Hero hero)
    {
        float damage = attackPower;
        if (isChargingPower)
        {
            damage *= chargedAttackMultiplier;
            isChargingPower = false; // 重置蓄力状态
            chargePowerJustEnded = true; // 设置标志
        }
        hero.TakeDamage(damage);

        return damage;
    }

    // 修改猛击技能
    public float HeavyStrike(Hero hero)
    {
        float damage = attackPower * 1.5f;
        hero.TakeDamage(damage);
        return damage;
    }

    // 修改五月雨斩技能
    public float MayRainSlash(Hero hero)
    {
        int hitCount = DecideHitCount();
        float baseDamage = attackPower * 0.4f; // 基础伤害为攻击力的40%
        float totalDamage = hitCount * baseDamage;
        hero.TakeDamage(totalDamage);
        return totalDamage;
    }

    // 修改 TakeDamage 方法
    public virtual float TakeDamage(float damage, bool ignoreDef = false)
    {
        float actualDamage = damage;

        if (!ignoreDef)
        {
            actualDamage = Mathf.Max(damage, 0);
            if (isWeakened)
            {
                actualDamage *= 1.2f; // 受到的伤害提高20%
            }
            if (isElectrified)
            {
                actualDamage *= 1.5f; // 触电状态下受到的伤害提高50%
            }
        }

        health -= actualDamage;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (monsterAnimator != null) monsterAnimator.SetTrigger("Get Hit");
        LogMessage($"{monsterName}受到{actualDamage:F1}点伤害");
        return actualDamage;
    }

    // 修改 StartTurn 方法
    public BurnInfo StartTurn()
    {
        BurnInfo burnInfo = new BurnInfo();
        burnJustEnded = false;

        if (burnTurnsRemaining > 0)
        {
            burnInfo.damage = ApplyBurnDamage();
            burnTurnsRemaining--; // 先减少回合数
            burnInfo.turnsRemaining = burnTurnsRemaining; // 然后记录剩余回合数

            if (burnTurnsRemaining == 0)
            {
                burnJustEnded = true;
            }
        }
        else
        {
            burnInfo.damage = 0;
            burnInfo.turnsRemaining = 0;
        }

        return burnInfo;
    }

    // 修改 ApplyBurnDamage 方法
    private float ApplyBurnDamage()
    {
        return TakeDamage(BURN_DAMAGE, true);
    }

    // 修改 EndTurn 方法
    public void EndTurn()
    {
        if (isWeakened)
        {
            weakenTurnsRemaining--;
            
            if (weakenTurnsRemaining <= 0)
            {
                attackPower = originalAttackPower;
                isWeakened = false;
                weakenJustEnded = true;
            }
        }

        if (electrifiedTurnsRemaining > 0)
        {
            electrifiedTurnsRemaining--;
            if (electrifiedTurnsRemaining == 0)
            {
                isElectrified = false;
                electrifiedJustEnded = true;
            }
        }
    }
        // 修改 ApplyBurn 方法
    public void ApplyBurn()
    {
        burnTurnsRemaining = BURN_DURATION;
        burnJustEnded = false; // 确保重置这个标志
    }

    // 新增应用冰冻效果的方法
    public void HandleFrozenState()
    {
        IsFrozen = false;
    }

    // 修改虚弱效果应用方法
    public void ApplyWeaken(int turns)
    {
        weakenTurnsRemaining = turns;
        if (!isWeakened)
        {
            originalAttackPower = attackPower;
            isWeakened = true;
        }
        attackPower = originalAttackPower * 0.75f; // 每次应用时都重新计算攻击力
        weakenJustEnded = false; // 重置这个标志
    }

    // 修改重置状态方法
    public virtual void ResetState()
    {
        health = maxHealth;
        isChargingPower = false;
    }

    // 修改蓄力技能
    public void ChargePower()
    {
        isChargingPower = true;
    }

    // 决定五月雨斩的攻击次数
    private int DecideHitCount()
    {
        float random = Random.value;
        if (random < 0.3f) return 2;
        if (random < 0.6f) return 3;
        if (random < 0.9f) return 4;
        return 5;
    }

    public bool IsWeakened { get; private set; }
    public int BurnTurnsRemaining { get; private set; }

    public void SetBattleLog(BattleLog log)
    {
        battleLog = log;
    }

    protected void LogMessage(string message)
    {
        if (battleLog != null)
        {
            battleLog.AddMessage(message);
        }
        else
        {
            Debug.LogWarning("BattleLog is null in Monster, cannot log message: " + message);
        }
    }

    public bool HasBurnJustEnded()
    {
        return burnJustEnded;
    }

    public bool HasWeakenJustEnded()
    {
        if (weakenJustEnded)
        {
            weakenJustEnded = false;
            return true;
        }
        return false;
    }

    private bool chargePowerJustEnded = false;

    public bool HasChargePowerJustEnded()
    {
        if (chargePowerJustEnded)
        {
            chargePowerJustEnded = false;
            return true;
        }
        return false;
    }

    // 新增属性来跟踪触电效果
    private int electrifiedTurnsRemaining = 0;
    private bool isElectrified = false;
    private bool electrifiedJustEnded = false;

    // 新增应用触电效果的方法
    public void ApplyElectrified(int duration = 4)
    {
        electrifiedTurnsRemaining = duration;
        isElectrified = true;
        electrifiedJustEnded = false; // 重置这个标志
    }

    // 新增方法来检查触电状态是否刚刚结束
    public bool HasElectrifiedJustEnded()
    {
        if (electrifiedJustEnded)
        {
            electrifiedJustEnded = false;
            return true;
        }
        return false;
    }

    // 新增方法来获取触电状态
    public bool IsElectrified => isElectrified;

    protected void SetLastAction(string action)
   {
    lastAction = action;
   }
}

