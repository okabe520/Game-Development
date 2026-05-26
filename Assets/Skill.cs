using UnityEngine;

[System.Serializable]
public class Skill
{
    public string name;
    public float manaCost;
    public float damage;
    public bool isUltimate;
    public bool isUnlocked;

    public Skill(string name, float manaCost, float damage, bool isUltimate = false, bool isUnlocked = false)
    {
        this.name = name;
        this.manaCost = manaCost;
        this.damage = damage;
        this.isUltimate = isUltimate;
        this.isUnlocked = isUnlocked;
    }

    public void Execute(Hero hero, Monster monster, Animator heroAnimator, System.Action<string> log)
    {
        float actualDamage;
        switch (name)
        {
            case "火球术":
                actualDamage = Mathf.Max(damage - monster.defense, 0);
                monster.TakeDamage(actualDamage);
                if (heroAnimator != null) heroAnimator.SetTrigger("Attack");
                bool hadBurnBefore = monster.BurnTurnsRemaining > 0;
                if (Random.value < 0.4f || hadBurnBefore)
                {
                    monster.ApplyBurn();
                    log(hadBurnBefore
                        ? $"火球术重置了{monster.monsterName}的燃烧状态，持续3回合"
                        : $"火球术使{monster.monsterName}陷入燃烧状态，持续3回合");
                }
                break;

            case "冰锥":
                actualDamage = Mathf.Max(damage - monster.defense, 0);
                monster.TakeDamage(actualDamage);
                if (heroAnimator != null) heroAnimator.SetTrigger("Attack");
                if (Random.value < 0.5f)
                {
                    monster.ApplyFreeze();
                    log($"冰锥使{monster.monsterName}陷入冰冻状态！");
                }
                break;

            case "雷击":
                actualDamage = Mathf.Max(damage - monster.defense, 0);
                monster.TakeDamage(actualDamage);
                if (heroAnimator != null) heroAnimator.SetTrigger("Attack");
                bool wasElectrified = monster.IsElectrified;
                if (Random.value < 0.3f || wasElectrified)
                {
                    monster.ApplyElectrified();
                    log(wasElectrified
                        ? $"{monster.monsterName}的触电状态被重置，持续3回合"
                        : $"{monster.monsterName}陷入触电状态，持续3回合");
                }
                break;

            case "虚弱":
                bool wasWeakenedBefore = monster.IsWeakened;
                monster.ApplyWeaken(3);
                log(wasWeakenedBefore
                    ? $"虚弱技能重置了{monster.monsterName}的虚弱状态，怪物攻击力降低25%，受到伤害增加20%，持续3回合"
                    : $"虚弱技能使{monster.monsterName}陷入虚弱状态，怪物攻击力降低25%，受到伤害增加20%，持续3回合");
                break;

            case "灵魂激流":
                actualDamage = Mathf.Max(hero.attackPower * 5 - monster.defense, 0);
                monster.TakeDamage(actualDamage);
                if (heroAnimator != null) heroAnimator.SetTrigger("Attack");
                hero.ResetRage();
                break;

            case "治疗术":
                float healAmount = 60f;
                hero.health = Mathf.Min(hero.health + healAmount, hero.maxHealth);
                log($"英雄使用了治疗术，恢复了 {healAmount} 点生命值！");
                break;
        }
    }
}
