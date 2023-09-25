// Copyright (c) 2023 - WhitetigerStudios
using UnityEngine;
using UnityEngine.Events;

namespace WhitetigerStudios.Research.Platformer
{
    /// <summary>
    /// The various player states
    /// </summary>
    public enum PlayerState
    {
        Locomoting, // Idle/Walking/Running on ground
        Wall,       // Temporarily interacting with the wall
        Hang,       // Hanging down from something
        Falling,    // Jumping/Falling in the air subject to gravity
        Flying,     // Moving in the air without gravity
        Dead        // Player is immobilized
    }

    /// <summary>
    /// Manages the player state - animations, etc
    /// </summary>
    public class PlatformerPlayerController : MonoBehaviour
    {
        [SerializeField] private PlatformerPlayer player;
        public PlatformerPlayer Player => player;
        public Collider PlayerCollider => Player.PlayerCollider;
        public Rigidbody PlayerRigidBody => Player.RigidBody;

        public PlayerState State { get; private set; } = PlayerState.Locomoting;

        public float InteractionRange = 0.5f;
        public float FallingSpeed = 0.5f;

        [Header("Events")]

        public UnityEvent<PlayerState> PlayerStateChanged;
        public UnityEvent IsLocomoting;
        public UnityEvent IsWall;
        public UnityEvent IsHang;
        public UnityEvent IsFalling;
        public UnityEvent IsFlying;
        public UnityEvent IsDead;

        [Header("Debug")]

        public bool EnableDebugLogs = false;
        public bool EnableDebugVisualization = false;

        private void Update()
        {
            UpdatePlayerState();
        }

        private void UpdatePlayerState()
        {
//TODO: All of these are false
            RaycastHit hit;
            Vector3 pos = PlayerCollider.transform.position;
            Transform xform = PlayerCollider.transform;
            Vector3 extents = PlayerCollider.bounds.extents;
            bool isGrounded = DoRaycast(pos, -xform.up, out hit, extents.y + InteractionRange, showGizmos: EnableDebugVisualization, rayColor: Color.green);
            bool isTouchingWall = DoRaycast(pos, xform.forward, out hit, extents.x + InteractionRange, showGizmos: EnableDebugVisualization, rayColor: Color.red) ||
                                  DoRaycast(pos, -xform.forward, out hit, extents.x + InteractionRange, showGizmos: EnableDebugVisualization, rayColor: Color.red) ||
                                  DoRaycast(pos, xform.right, out hit, extents.x + InteractionRange, showGizmos: EnableDebugVisualization, rayColor: Color.red) ||
                                  DoRaycast(pos, -xform.right, out hit, extents.x + InteractionRange, showGizmos: EnableDebugVisualization, rayColor: Color.red);
            bool isTouchingCeiling = DoRaycast(pos, xform.up, out hit, extents.y + InteractionRange, showGizmos: EnableDebugVisualization, rayColor: Color.blue);
            bool isFalling = !isGrounded && !isTouchingWall && !isTouchingCeiling && PlayerRigidBody.velocity.y < -FallingSpeed;
            bool isDead = false;    //TODO: Handle death - how to set?  update this function and use in property

            PlayerState newState = State switch
            {
                _ when isGrounded => PlayerState.Locomoting,
                _ when isTouchingWall => PlayerState.Wall,
                _ when isTouchingCeiling => PlayerState.Hang,
                _ when isFalling => PlayerState.Falling,
                _ when isDead => PlayerState.Dead,
                _ => State,
            };

            // Verbose logs
            if (EnableDebugLogs)
            {
                Debug.Log($"PlatformerPlayerController.UpdatePlayerState - isGrounded: {isGrounded}, isTouchingWall: {isTouchingWall}, isTouchingCeiling: {isTouchingCeiling}, isFalling: {isFalling} | State: {State}, newState: {newState}");
            }

            // Update new state
            if (State != newState)
            {
                Debug.Log($"PlatformerPlayerController.UpdatePlayerState - Changed: {newState} (previous: {State})");

                State = newState;
                PlayerStateChanged?.Invoke(State);

                switch (State)
                {
                    case PlayerState.Locomoting:
                        IsLocomoting?.Invoke();
                        break;
                    case PlayerState.Wall:
                        IsWall?.Invoke();
                        break;
                    case PlayerState.Hang:
                        IsHang?.Invoke();
                        break;
                    case PlayerState.Falling:
                        IsFalling?.Invoke();
                        break;
                    case PlayerState.Flying:
                        IsFlying?.Invoke();
                        break;
                    case PlayerState.Dead:
                        IsDead?.Invoke();
                        break;
                }
            }
        }

        public static bool DoRaycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, bool showGizmos = false, Color? rayColor = null, float hitSphereRadius = 0.1f)
        {
            bool wasHit = Physics.Raycast(origin, direction, out hitInfo, maxDistance);

//TODO: This needs to be in the OnDrawGizmos functions
            /*if (showGizmos)
            {
                if (rayColor == null)
                {
                    rayColor = Color.red;
                }
                Debug.DrawRay(origin, hitInfo.point - origin, (Color)rayColor);
                Gizmos.DrawSphere(hitInfo.point, hitSphereRadius);  //TODO: Set color
            }*/

            return wasHit;
        }
    }
}
