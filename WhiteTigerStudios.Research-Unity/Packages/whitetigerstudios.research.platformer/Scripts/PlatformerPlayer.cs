// Copyright (c) 2023 - WhitetigerStudios
using UnityEngine;

namespace WhitetigerStudios.Research.Platformer
{
    /// <summary>
    /// Player movement, etc
    /// </summary>
    public class PlatformerPlayer : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpImpulse = 10f;
        [SerializeField] private Rigidbody rigidBody;

        private void FixedUpdate()
        {
            // Walk
            float moveAmount = Input.GetAxisRaw("Horizontal");
            Vector3 movement = (Vector3.forward * moveAmount).normalized * moveSpeed * Time.fixedDeltaTime;
            rigidBody.MovePosition(rigidBody.position + movement);

            // Jump
            if (Input.GetKeyDown(KeyCode.W))
            {
                Vector3 jumpMovement = Vector3.up * jumpImpulse;
                rigidBody.AddForce(jumpMovement, ForceMode.Impulse);
            }
        }
    }
}
