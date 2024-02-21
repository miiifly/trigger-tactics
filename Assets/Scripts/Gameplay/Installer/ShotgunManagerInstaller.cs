using UnityEngine;
using Zenject;

namespace TriggerTactics.Gameplay
{
    public class ShotgunManagerInstaller : MonoInstaller
    {
        [SerializeField]
        private ShotgunManager _shotgunManager;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<IShotgunManager>().FromInstance(_shotgunManager).AsSingle().NonLazy();
        }
    }
}
