using TriggerTactics.Gameplay.Special;
using UnityEngine;

namespace TriggerTactics.Gameplay.Spawn
{
    public class BaseSpecial : MonoBehaviour, ISpecialComponent
    {
        [SerializeField]
        private SpecialType _specialType;
        public SpecialType SpecialType => _specialType;

        GameObject IBaseSpawnable.GameObject => this.gameObject;

        int IBaseSpawnable.SpawnableTypeID => _specialType.GetHashCode();
    }
}
