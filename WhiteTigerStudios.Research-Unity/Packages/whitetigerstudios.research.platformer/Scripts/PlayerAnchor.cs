// Copyright (c) 2023 - WhitetigerStudios
using UnityEngine;

namespace WhitetigerStudios.Research.Platformer
{
    /// <summary>
    /// When the player enters the trigger, the player is parented to this object, 
    /// effectively "anchoring" them to the object, so they move relative to it.
    /// Make sure that this GameObject has a trigger collider on it.  The player 
    /// should also have a trigger collider on it as well.
    /// </summary>
    public class PlayerAnchor : MonoBehaviour
    {
        public Transform PlayerTransform => PlatformerPlayerController.Instance.Player.transform;
        public Transform PlayerParentTransform => PlayerTransform.parent;
        public Transform PlayerAnchorTransform => PlatformerPlayerController.Instance.Player.PlayerAnchor;
        private Transform originalParent;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"PlayerAnchor.OnTriggerEnter - other: {other.name} (PlayerTransform: {PlayerTransform.name}) (PlayerAnchorTransform: {PlayerAnchorTransform.name}) (PlayerParentTransform: {(PlayerParentTransform != null ? PlayerParentTransform.name : "null")})");
            if (other.transform == PlayerAnchorTransform)
            {
                originalParent = PlayerParentTransform;
                PlayerTransform.SetParent(transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log($"PlayerAnchor.OnTriggerExit - other: {other.name} (PlayerTransform: {PlayerTransform.name}) (PlayerAnchorTransform: {PlayerAnchorTransform.name}) (OriginalParent: {(originalParent != null ? originalParent.name : "null")})");
            if (other.transform == PlayerAnchorTransform)
            {
                PlayerTransform.SetParent(originalParent);
            }
        }
    }
}
