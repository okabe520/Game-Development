using UnityEngine;

public class HealingBoss : Monster
{
    private int turnCount = 0;
    private const int HEAL_INTERVAL = 6;
    private const float HEAL_AMOUNT = 50f;

    private bool isBloodSacrificeActive = false;
    private float bloodSacrificeDamage = 0f;

    private float VAMPIRIC_DAGGER_DAMAGE;
    private float BLOOD_DAGGER_DAMAGE;

    private bool isSecondPhase = false;
    private bool hasUsedDeathContract = false;
    private int deathContractChargeCount = 0;

    protected override void Start()
    {
        monsterName = "嗜血狼人";
        maxHealth = 400f;
        health = maxHealth;
        bossIndex = 5;
        base.Start();

        // 初始化依赖于 attackPower 的常量
        VAMPIRIC_DAGGER_DAMAGE = attackPower * 1.5f;
        BLOOD_DAGGER_DAMAGE = attackPower * 2.5f;
    }

    public override float DecideAction(Hero hero)
    {
        if (IsFrozen)
        {
            SetLastAction("Frozen");
            return 0;
        }

        turnCount++;
        float damage = 0;

        if (isSecondPhase && health <= maxHealth * 0.3f && !hasUsedDeathContract)
        {
            if (deathContractChargeCount < 3)
            {
                ChargeDeathContract();
                SetLastAction("ChargeDeathContract");
                return 0;
            }
            else
            {
                damage = ReleaseDeathContract(hero);
                SetLastAction("ReleaseDeathContract");
                hasUsedDeathContract = true;
                return damage;
            }
        }

        if (isBloodSacrificeActive)
        {
            float baseDamage = attackPower; // 使用攻击力而不是调用 Attack 方法
            float totalDamage = baseDamage + bloodSacrificeDamage;
            float actualDamage = Mathf.Max(totalDamage - hero.defense, 0); // 计算实际伤害
            hero.TakeDamage(totalDamage); // 只调用一次 TakeDamage，传入总伤害
            SetLastAction("BloodSacrificeAttack");
            lastActionDescription = $"嗜血狼人使用了强化攻击，造成了 {actualDamage:F1} 点伤害！";
            isBloodSacrificeActive = false;
            bloodSacrificeDamage = 0f;
            return actualDamage;
        }
        else if (!isSecondPhase && turnCount % HEAL_INTERVAL == 0)
        {
            Heal();
            SetLastAction("Heal");
        }
        else
        {
            float random = Random.value;
            if (random < 0.5f) // 50% 的概率使用普通攻击
            {
                damage = Attack(hero);
                SetLastAction("Attack");
            }
            else if (random < 0.7f) // 20% 的概率使用猛击或全力一击
            {
                damage = isSecondPhase ? FullPowerStrike(hero) : HeavyStrike(hero);
                SetLastAction(isSecondPhase ? "FullPowerStrike" : "HeavyStrike");
            }
            else if (random < 0.85f) // 15% 的概率使用鲜血献祭
            {
                if (PerformBloodSacrifice())
                {
                    SetLastAction("BloodSacrifice");
                }
                else
                {
                    // 如果无法执行鲜血献祭，则执行普通攻击
                    damage = Attack(hero);
                    SetLastAction("Attack");
                }
            }
            else // 15% 的概率使用吸血匕首或附血匕首
            {
                damage = isSecondPhase ? UseBloodDagger(hero) : UseVampiricDagger(hero);
                SetLastAction(isSecondPhase ? "BloodDagger" : "VampiricDagger");
            }
        }

        return damage;
    }

    private float UseVampiricDagger(Hero hero)
    {
        float actualDamage = Mathf.Max(VAMPIRIC_DAGGER_DAMAGE - hero.defense, 0);
        hero.TakeDamage(VAMPIRIC_DAGGER_DAMAGE);
        health += actualDamage;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        lastActionDescription = $"嗜血狼人使用了吸血匕首，对英雄造成 {actualDamage:F1} 点伤害，并恢复了 {actualDamage:F1} 点生命值！";
        return VAMPIRIC_DAGGER_DAMAGE;
    }

    private float UseBloodDagger(Hero hero)
    {
        float selfDamage = Mathf.Min(20f, health - 1);
        TakeDamage(selfDamage);
        float actualDamage = Mathf.Max(BLOOD_DAGGER_DAMAGE - hero.defense, 0);
        hero.TakeDamage(BLOOD_DAGGER_DAMAGE);
        lastActionDescription = $"嗜血狼人使用了附血匕首，消耗 {selfDamage:F1} 点生命值，对英雄造成 {actualDamage:F1} 点伤害！";
        return BLOOD_DAGGER_DAMAGE;
    }

    private void Heal()
    {
        health += HEAL_AMOUNT;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        lastActionDescription = $"嗜血狼人恢复了 {HEAL_AMOUNT} 点生命值";
    }

    private bool PerformBloodSacrifice()
    {
        float minHealth = isSecondPhase ? 30f : 20f;
        if (health <= minHealth)
        {
            // 血量太低，无法执行鲜血献祭
            return false;
        }

        float minDamage = isSecondPhase ? 30f : 20f;
        float maxDamage = isSecondPhase ? 60f : 50f;
        bloodSacrificeDamage = Random.Range(minDamage, maxDamage);
        float actualDamage = Mathf.Min(bloodSacrificeDamage, health - 1);
        TakeDamage(actualDamage);
        isBloodSacrificeActive = true;
        lastActionDescription = $"嗜血狼人使用了鲜血献祭，下一次攻击将会更强！";
        return true;
    }

    private float FullPowerStrike(Hero hero)
    {
        float damage = attackPower * 2f;
        hero.TakeDamage(damage);
        lastActionDescription = $"嗜血狼人使用了全力一击，对英雄造成 {Mathf.Max(damage - hero.defense, 0):F1} 点伤害！";
        return damage;
    }

    private void ChargeDeathContract()
    {
        deathContractChargeCount++;
        lastActionDescription = $"嗜血狼人正在签订死神契约！（{deathContractChargeCount}/4）";
        SetLastAction("ChargeDeathContract");
    }

    private float ReleaseDeathContract(Hero hero)
    {
        float damage = hero.health + hero.defense - 1;
        hero.TakeDamage(damage);
        lastActionDescription = $"嗜血狼人签订了死神契约，将英雄的生命值降至 1 点！";
        SetLastAction("ReleaseDeathContract");
        return damage;
    }

    public override void ResetState()
    {
        base.ResetState();
        turnCount = 0;
        isBloodSacrificeActive = false;
        bloodSacrificeDamage = 0f;
        isSecondPhase = false;
        hasUsedDeathContract = false;
        deathContractChargeCount = 0;
    }

    public override float Attack(Hero hero)
    {
        return base.Attack(hero);
    }

    public override float TakeDamage(float damage, bool ignoreDef = false)
    {
        float actualDamage = base.TakeDamage(damage, ignoreDef);

        if (health <= 0 && !isSecondPhase)
        {
            EnterSecondPhase();
            // 不需要在这里再次调用 LogMessage，因为 EnterSecondPhase 已经处理了
        }

        return actualDamage;
    }

    private void EnterSecondPhase()
    {
        isSecondPhase = true;
        health = maxHealth;
        isBloodSacrificeActive = false; // 重置鲜血献祭强化
        bloodSacrificeDamage = 0f;
        lastActionDescription = "嗜血狼人解放了鲜血王朝的封印！";
        LogMessage(lastActionDescription); // 直接输出消息
    }

    public bool IsInSecondPhase()
    {
        return isSecondPhase;
    }

    // 添加 HeavyStrike 方法
    new private float HeavyStrike(Hero hero)
    {
        float damage = attackPower * 1.5f;
        hero.TakeDamage(damage);
        lastActionDescription = $"嗜血狼人使用了猛击，对英雄造成 {Mathf.Max(damage - hero.defense, 0):F1} 点伤害！";
        return damage;
    }
}