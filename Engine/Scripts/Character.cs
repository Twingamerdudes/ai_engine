using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine.Scripts
{
    public class Character : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("The speed at which the character moves.")]
        [SerializeField]
        private float moveSpeed = 3f;
    
        [Tooltip("How long the character will move for.")]
        [SerializeField]
        private float moveDuration = 1f;
    
        [Tooltip("Can the character move?")]
        public bool isMovementAllowed = true;
    
        [Tooltip("Can the character look around?")]
        public bool isLookingAllowed = true;
    
        [HideInInspector]
        public Transform target;

        private Animator _animator;

        private float _timer;

        private bool _isMoving;


        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= 20f && isMovementAllowed)
            {
                _timer = 0f;
                if(Random.Range(0,10)> 4)
                {
                    StartCoroutine(MoveCharacter());
                }
            }
            //look at target
            if (target != null && !_isMoving && isLookingAllowed)
            {
                transform.LookAt(target);
            }
        }

        private IEnumerator MoveCharacter()
        {
            _isMoving = true;
            if (_animator != null)
            {
                // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
                _animator.SetBool("Walking", true);
            }
            float elapsedTime = 0f;
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            Vector3 randomDirection = new Vector3(x, 0f, z).normalized;
 
            while (elapsedTime < moveDuration && isMovementAllowed)
            {
                elapsedTime += Time.deltaTime;
                transform.Translate(randomDirection * (moveSpeed * Time.deltaTime), Space.World);
                // ReSharper disable once Unity.InefficientPropertyAccess
                transform.LookAt(transform.position + randomDirection);
                yield return null;
            }
            if (_animator != null)
            {
                // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
                _animator.SetBool("Walking", false);
            }
            _isMoving = false;
        }
    }
}
