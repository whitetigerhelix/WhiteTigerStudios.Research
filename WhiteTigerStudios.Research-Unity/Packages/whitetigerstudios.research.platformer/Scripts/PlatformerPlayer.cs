// Copyright (c) 2023 - WhitetigerStudios
using UnityEngine;

namespace WhitetigerStudios.Research.Platformer
{
    /// <summary>
    /// Player movement, etc
    /// </summary>
    public class PlatformerPlayer : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 200f;
        [SerializeField] private float jumpImpulse = 25f;
        [SerializeField] private PlatformerPlayerController playerController;
        [SerializeField] private Rigidbody rigidBody;
        public Rigidbody RigidBody => rigidBody;
        [SerializeField] private Collider playerCollider;
        public Collider PlayerCollider => playerCollider;
        public PlayerState PlayerState => playerController.State;

        private Vector3 previousMovement = Vector3.zero;

        private void FixedUpdate()
        {
            UpdatePlayerMovement();
        }

        private void UpdatePlayerMovement()
        {
            if (PlayerState == PlayerState.Dead)
            {
                return;
            }

            // Walk
            float moveAmount = Input.GetAxisRaw("Horizontal");

            Vector3 movement = Vector3.zero;
            if (moveAmount != 0f)
            {
                movement = transform.forward * moveAmount * moveSpeed * Time.fixedDeltaTime;
            }
            else if (previousMovement != Vector3.zero && rigidBody.velocity.sqrMagnitude > 0f)
            {
                float damping = 5f;
                movement = Vector3.Lerp(Vector3.zero, -previousMovement * 0.4f, Time.fixedDeltaTime * damping);
            }
            previousMovement = movement;

            movement.y = rigidBody.velocity.y;
            rigidBody.velocity = movement;

            // Jump
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
//TODO: Check for wall jumping
                if (playerController.State != PlayerState.Falling)
                {
                    Debug.Log($"PlatformerPlayer.FixedUpdate - Jump | PlayerState: {playerController.State}");
                    Vector3 jumpMovement = Vector3.up * jumpImpulse;
                    rigidBody.AddForce(jumpMovement, ForceMode.Impulse);
                }
                else
                {
                    Debug.LogWarning($"PlatformerPlayer.FixedUpdate - Can't jump when falling | PlayerState: {playerController.State}");
                }
            }
        }
    }
}
