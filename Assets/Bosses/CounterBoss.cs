using UnityEngine;

public class CounterBoss : Monster
{
    private const float COUNTER_CHANCE = 0.3f; // 30%几率进行反击
    private bool canCounter = false;
    private bool isStoneWillActive = false;
    private float storedDamage = 0f;
    private bool mustCounterNextTurn = false;

    protected override void Start()
    {
        monsterName = "石先锋";
        maxHealth = 500f;
        health = maxHealth;
        attackPower = 15f;
        bossIndex = 4;
        base.Start();
    }

    public override float DecideAction(Hero hero)
    {
        if (IsFrozen)
        {
            SetLastAction("Frozen");
            return 0;
        }

        float damage = 0;

        if (mustCounterNextTurn)
        {
            damage = StoneWillCounter(hero);
            mustCounterNextTurn = false;
            SetLastAction("StoneWillCounter");
            return damage;
        }

        if (health <= maxHealth * 0.3f && !isStoneWillActive)
        {
            UseStoneWill();
            return 0;
        }

        float random = Random.value;
        if (random < 0.4f) // 40% 概率普通攻击
        {
            damage = Attack(hero);
            SetLastAction("Attack");
        }
        else if (random < 0.7f) // 30% 概率使用碎石
        {
            damage = UseStoneShards(hero);
            SetLastAction("StoneShards");
        }
        else // 30% 概率使用砸地
        {
            damage = UseGroundSmash(hero);
            SetLastAction("GroundSmash");
        }

        canCounter = true; // 允许下次受到攻击时反击
        return damage;
    }

    public override float TakeDamage(float damage, bool ignoreDef = false)
    {
        if (isStoneWillActive)
        {
            storedDamage = damage;
            LogMessage("石先锋的石之意志使攻击无效！");
            isStoneWillActive = false;
            mustCounterNextTurn = true;
            return 0;
        }

        if (Random.value < 0.2f) // 20% 概率无效
        {
            LogMessage("石先锋的坚硬躯壳使攻击无效！");
            return 0;
        }

        float actualDefense = (health > maxHealth * 0.5f) ? 8f : 0f;
        float modifiedDamage = Mathf.Max(damage - actualDefense, 0);

        // 调用基类的 TakeDamage 方法来应用基本的受伤害规则
        float actualDamage = base.TakeDamage(modifiedDamage, ignoreDef);

        if (health <= 0)
        {
            health = 0;
            LogMessage("石先锋被击败了！");
        }
        else if (canCounter && !IsFrozen && Random.value < COUNTER_CHANCE)
        {
            if (hero != null)
            {
                float counterDamage = attackPower * 2f; // 反击造成200%的攻击力伤害
                hero.TakeDamage(counterDamage);
                LogMessage($"石先锋进行反击，对英雄造成{Mathf.Max(counterDamage-hero.defense, 0)}点伤害");
                BattleManager.Instance.UpdateHeroStatus(); // 更新英雄状态
            }
        }

        canCounter = false; // 重置反击状态
        return actualDamage;
    }

    private float UseStoneShards(Hero hero)
    {
        float damage = attackPower*1.5f;
        hero.TakeDamage(damage);
        lastActionDescription = $"石先锋使用了碎石，对英雄造成 {Mathf.Max(damage-hero.defense, 0)} 点伤害！";
        return damage;
    }

    private float UseGroundSmash(Hero hero)
    {
        float damage = attackPower*2f;
        hero.TakeDamage(damage);
        TakeDamage(damage);
        lastActionDescription = $"石先锋使用了砸地，牺牲自己的生命值，对英雄造成 {Mathf.Max(damage-hero.defense, 0)} 点伤害！";
        return damage;
    }

    private void UseStoneWill()
    {
        isStoneWillActive = true;
        LogMessage("石先锋使用了石之意志，下一次受到的攻击将无效！");
        SetLastAction("StoneWill");
    }

    private float StoneWillCounter(Hero hero)
    {
        float damage = storedDamage * 3;
        hero.TakeDamage(damage);
        lastActionDescription = $"石先锋使用石之意志的力量进行反击，对英雄造成 {Mathf.Max(damage - hero.defense, 0):F1} 点伤害！";
        LogMessage(lastActionDescription);
        isStoneWillActive = false;
        storedDamage = 0;
        return damage;
    }

    public override float Attack(Hero hero)
    {
        float damage = base.Attack(hero);
        lastActionDescription = $"石先锋进行普通攻击，对英雄造成 {Mathf.Max(damage - hero.defense, 0):F1} 点伤害！";
        return damage;
    }

    public override void ResetState()
    {
        base.ResetState();
        canCounter = false;
        isStoneWillActive = false;
        storedDamage = 0f;
        mustCounterNextTurn = false;
    }
}