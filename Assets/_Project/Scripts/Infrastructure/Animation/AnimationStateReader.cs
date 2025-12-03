using System;
using _Project.Scripts.Utils.Extensions;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Animation
{
    public sealed class AnimationStateReader : StateMachineBehaviour
    {
        [SerializeField] private float _eventTime;

        private bool _isFindReader;
        private bool _isSendEvent;

        private IAnimationStateReader[] _stateReaders = Array.Empty<IAnimationStateReader>();

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (_isFindReader == false)
            {
                FindReaders(animator);
            }
     
            _stateReaders.Foreach(stateReader => stateReader.EnteredState(stateInfo.shortNameHash));
            
            _isSendEvent = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            float time = stateInfo.normalizedTime;

            if (_isFindReader && _isSendEvent == false && time > _eventTime)
            {
                _stateReaders.Foreach(stateReader => stateReader.UpdateState(stateInfo.shortNameHash));
                
                _isSendEvent = true;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            if (_isFindReader == false)
            {
                FindReaders(animator);
            }
            
            _stateReaders.Foreach(stateReader => stateReader.ExitedState(stateInfo.shortNameHash));
        }

        private void FindReaders(Animator animator)
        {
            _stateReaders = animator.gameObject.GetComponentsInChildren<IAnimationStateReader>();

            _isFindReader = _stateReaders.Length > 0;
        }
    }
}