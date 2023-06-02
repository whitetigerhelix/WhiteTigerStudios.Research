// Copyright (c) 2023 - WhitetigerStudios
using System;
using UnityEngine;

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

        public PlayerState State { get; private set; } = PlayerState.Locomoting;

        public event Action<PlayerState> PlayerStateChanged;

        private void Update()
        {
            UpdatePlayerState();
        }

        private void UpdatePlayerState()
        {
            //TODO: Check if player is touching the ground
            //TODO: If not, check if touching the wall
            //TODO: If not, check if touching the ceiling
            //TODO: If not, are we falling?
            //TODO: Or dead?
            //TODO: I'm thinking of handling rigid body collisions and doing an angle check from collision normal

            PlayerState newState = PlayerState.Dead;
            if (State != newState)
            {
                State = newState;
                PlayerStateChanged?.Invoke(State);
            }
        }
    }
}
