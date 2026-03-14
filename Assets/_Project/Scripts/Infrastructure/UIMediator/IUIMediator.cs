using System;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Infrastructure.UIMediator
{
    public interface IUIMediator : IInitializable, IDisposable
    {
        void Initialize();
        
    }
}
