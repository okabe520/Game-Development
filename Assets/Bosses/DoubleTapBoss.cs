using UnityEngine;

public class DoubleTapBoss : Monster
{
    private const float DOUBLE_ATTACK_CHANCE = 0.3f;

    protected override void Start()
    {
        monsterName = "影武者";
        maxHealth = 350f;
        health = maxHealth;
        bossIndex = 3;
        base.Start();
    }

    public override float DecideAction(Hero hero)
    {
        if (IsFrozen)
        {
            lastAction = "Frozen";
            return 0;
        }

        float damage = Attack(hero);

        if (Random.value < DOUBLE_ATTACK_CHANCE)
        {
            damage += Attack(hero);
            lastAction = "DoubleAttack";
        }
        else
        {
            lastAction = "Attack";
        }

        return damage;
    }
}
