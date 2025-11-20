using UnityEngine;
using Zenject;
using Network.API;
using System.ComponentModel;

public class MenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IMessageBox>()
     .To<MessageBoxHandler>()
     .FromComponentInHierarchy()
     .AsSingle();

        Container.BindInterfacesAndSelfTo<APIHandler>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<GameEvents>().FromComponentInHierarchy().AsSingle();
    }
}