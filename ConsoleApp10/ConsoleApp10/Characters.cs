public class Character
{
    public string ClassName { get; set; }
    public int HP { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int SpecialAbilityCooldown { get; set; }


    public virtual void UseSpecialAbility() { }
    public void UpdateCooldown()
    {
        if (SpecialAbilityCooldown > 0)
        {
            SpecialAbilityCooldown--;
        }
    }


    public virtual int GetDamage()
    {
        return 0;
    }

    public virtual void ResetDamage() { }
}

public class Warrior : Character
{
    private int baseDamage = 20;

    public Warrior()
    {
        ClassName = "Воин";
        HP = 120;
        SpecialAbilityCooldown = 0;
    }

    public override void UseSpecialAbility()
    {
        if (SpecialAbilityCooldown == 0)
        {
            Console.WriteLine($"{ClassName} использует специальную атаку!");
            baseDamage += 5;
            SpecialAbilityCooldown = 3;
        }
        else
        {
            Console.WriteLine("Специальная атака на перезарядке!");
        }
    }

    public override int GetDamage()
    {
        return (PositionX + PositionY) % 2 == 0 ? baseDamage + 3 : baseDamage;
    }

    public override void ResetDamage()
    {
        baseDamage = 20;
    }
}

public class Archer : Character
{
    private int baseDamage = 15;

    public Archer()
    {
        ClassName = "Лучник";
        HP = 80;
        SpecialAbilityCooldown = 0;
    }

    public override void UseSpecialAbility()
    {
        if (SpecialAbilityCooldown == 0)
        {
            Console.WriteLine($"{ClassName} использует специальную атаку!");
            baseDamage += 10;
            SpecialAbilityCooldown = 2;
        }
        else
        {
            Console.WriteLine("Специальная атака на перезарядке!");
        }
    }

    public override int GetDamage()
    {
        return (PositionX + PositionY) % 2 == 0 ? baseDamage + 3 : baseDamage;
    }

    public override void ResetDamage()
    {
        baseDamage = 17;
    }
}

public class Mage : Character
{
    public Mage()
    {
        ClassName = "Маг";
        HP = 70;
        SpecialAbilityCooldown = 0;
    }

    public override void UseSpecialAbility()
    {
        if (SpecialAbilityCooldown == 0)
        {
            Console.WriteLine($"{ClassName} использует специальную атаку!");
            HP += 10;
            SpecialAbilityCooldown = 4;
        }
        else
        {
            Console.WriteLine("Специальная атака на перезарядке!");
        }
    }

    public override int GetDamage()
    {
        return (PositionX + PositionY) % 2 == 0 ? 20 + 3 : 20;
    }
}

public class Assassin : Character
{
    private int baseDamage = 18;

    public Assassin()
    {
        ClassName = "Ассасин";
        HP = 90;
        SpecialAbilityCooldown = 0;
    }

    public override void UseSpecialAbility()
    {
        if (SpecialAbilityCooldown == 0)
        {
            Console.WriteLine($"{ClassName} использует специальную атаку!");
            baseDamage *= 2;
            SpecialAbilityCooldown = 5;
        }
        else
        {
            Console.WriteLine("Специальная атака на перезарядке!");
        }
    }

    public override int GetDamage()
    {
        return (PositionX + PositionY) % 2 == 0 ? baseDamage + 3 : baseDamage;
    }

    public override void ResetDamage()
    {
        baseDamage = 18;
    }
}