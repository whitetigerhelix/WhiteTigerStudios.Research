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
        [SerializeField] private Rigidbody rigidBody;
        public Rigidbody RigidBody => rigidBody;

        private Vector3 previousMovement = Vector3.zero;

        private void FixedUpdate()
        {
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
                Vector3 jumpMovement = Vector3.up * jumpImpulse;
                rigidBody.AddForce(jumpMovement, ForceMode.Impulse);
            }
        }
    }
}
