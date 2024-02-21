using System;

namespace TriggerTactics.Gameplay
{
    public interface IShotgunManager
    {
        void GenerateAmmon();
        void InitializeShotgun();
        bool Shoot();
        event Action MagazineEmpty;
    }
}

