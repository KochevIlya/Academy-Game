using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Scenes.Game.Posts
{
    public class Post : MonoBehaviour
    {
        [SerializeField] private GameObject _plane;
        public bool _isActive = true;
        void Start()
        {
            _plane.gameObject.SetActive(_isActive);
        }

        public void Hide()
        {
            _isActive = false;
            _plane.gameObject.SetActive(false);
        }
    }
}

