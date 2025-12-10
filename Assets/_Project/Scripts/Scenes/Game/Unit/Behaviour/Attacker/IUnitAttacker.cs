namespace _Project.Scripts.Scenes.Game.Unit.Attacker
{
  public interface IUnitAttacker
  {
    void Shoot(GameUnit unit);
    void OnShootCast(GameUnit unit);
    void AbilityUse(GameUnit unit);
  }
}