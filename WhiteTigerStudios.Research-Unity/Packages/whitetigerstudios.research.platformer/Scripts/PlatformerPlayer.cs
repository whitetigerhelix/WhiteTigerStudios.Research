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
        //[SerializeField] private float smoothness = 1f;     // Smooth movement
        [SerializeField] private Rigidbody rigidBody;
        public Rigidbody RigidBody => rigidBody;

        private void FixedUpdate()
        {
            // Walk
            float moveAmount = Input.GetAxisRaw("Horizontal");
            //float smoothedMoveAmount = Mathf.Lerp(currentMoveAmount, moveAmount, smoothness * Time.fixedDeltaTime);
            //Vector3 movement = (Vector3.forward * smoothedMoveAmount).normalized * moveSpeed * Time.fixedDeltaTime;
            Vector3 movement = (Vector3.forward * moveAmount).normalized * moveSpeed * Time.fixedDeltaTime;
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
