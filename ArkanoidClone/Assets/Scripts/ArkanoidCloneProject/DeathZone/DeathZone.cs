using UnityEngine;

namespace ArkanoidCloneProject.Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeathZone : MonoBehaviour
    {
        private BoxCollider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _collider.isTrigger = false;
            gameObject.tag = "DeathZone";
        }
    }
}