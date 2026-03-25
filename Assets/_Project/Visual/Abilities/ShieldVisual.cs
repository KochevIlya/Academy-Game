using UnityEngine;

namespace _Project.Visual.Abilities
{
    public class ShieldVisual : MonoBehaviour
    {
        [SerializeField] GameObject _shieldVisual;

        public void ChangeShieldVisual(bool isVisible)
        {
            _shieldVisual.SetActive(isVisible);
        }
    
    }
}
