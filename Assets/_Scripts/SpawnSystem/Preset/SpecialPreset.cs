using System.Collections;
using System.Collections.Generic;
using TriggerTactics.Gameplay.Spawn;
using UnityEngine;


namespace TriggerTactics.Gameplay.Spawn
{
    [CreateAssetMenu(fileName = "SpecialPreset", menuName = "TriggerTactics/Special/SpecialPreset")]
    public class SpecialPreset : ScriptableObject
    {
        [SerializeField]
        private List<BaseSpecial> _specials = new List<BaseSpecial>();
        public IEnumerable<BaseSpecial> Specials => _specials;
    }
}

