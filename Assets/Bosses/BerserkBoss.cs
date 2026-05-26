using UnityEngine;

public class BerserkBoss : Monster
{
    private int turnCount = 0;
    private const float INITIAL_ATTACK_INCREASE_RATE = 0.1f; // 初始每回合增加10%攻击力
    private const float ENRAGED_ATTACK_INCREASE_RATE = 0.2f; // 狂暴后每回合增加20%攻击力
    private float currentAttackIncreaseRate = INITIAL_ATTACK_INCREASE_RATE;
    private bool hasUsedDeathRoar = false;
    private const float DEATH_ROAR_THRESHOLD = 0.3f; // 30%血量阈值

    protected override void Start()
    {
        monsterName = "狂暴龙王";
        maxHealth = 350f;
        health = maxHealth;
        bossIndex = 2;
        base.Start();
    }

    public override float DecideAction(Hero hero)
    {
        if (IsFrozen)
        {
            lastAction = "Frozen";
            return 0;
        }

        turnCount++;

        // 检查是否需要使用死亡怒吼
        if (!hasUsedDeathRoar && health <= maxHealth * DEATH_ROAR_THRESHOLD)
        {
            string deathRoarMessage = UseDeathRoar();
            lastActionDescription = deathRoarMessage; // 存储描述以供 BattleManager 使用
            return 0; // 使用技能后结束回合
        }

        attackPower *= (1 + currentAttackIncreaseRate);

        float damage = Attack(hero);
        lastAction = "Attack";
        return damage;
    }

    private string UseDeathRoar()
    {
        hasUsedDeathRoar = true;
        currentAttackIncreaseRate = ENRAGED_ATTACK_INCREASE_RATE;
        lastAction = "DeathRoar";
        
        // 添加额外效果
        health += maxHealth * 0.1f; // 恢复10%的最大生命值
        defense *= 1.2f; // 增加20%的防御力
        
        return "狂暴龙王释放了死亡怒吼！攻击力增长率提升到20%！同时恢复了10%的生命值并增加了20%的防御力！";
    }

    public override void ResetState()
    {
        base.ResetState();
        turnCount = 0;
        attackPower = 15f; // 重置为初始攻击力
        hasUsedDeathRoar = false;
        currentAttackIncreaseRate = INITIAL_ATTACK_INCREASE_RATE;
    }
}