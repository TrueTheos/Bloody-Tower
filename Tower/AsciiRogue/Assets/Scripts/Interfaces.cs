public interface ITakeDamage
{
    void TakeDamage(int damage);
}

public interface IDoDamage
{
    void DoDamage(int damage);
}

public interface IPoison
{
    void Poison();
}

/*
 * fire burns organic material and boils liquids (not potions). 
 * There is 20% to destroy scrolls and potions.
 * */
public interface IFire
{
    void Fire();
}


public interface IBleeding
{
    void Bleed();
}



public interface IFireResistance
{
    void FireResistance();
}

public interface IPoisonResistance
{
    void PoisonResistance();
}

public interface IRegeneration
{
    void Regeneration();
}

public interface IFullRestore
{
    void FullRestore();
}

public interface IInvisible
{
    void Invisible();
}
