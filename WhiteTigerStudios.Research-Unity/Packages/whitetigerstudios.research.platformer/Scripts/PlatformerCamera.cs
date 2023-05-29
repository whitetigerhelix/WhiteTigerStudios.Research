// Copyright (c) 2023 - WhitetigerStudios
using UnityEngine;

namespace WhitetigerStudios.Research.Platformer
{
    /// <summary>
    /// Side-scroller camera
    /// </summary>
    public class PlatformerCamera : MonoBehaviour
    {
        [SerializeField] private PlatformerPlayer player;
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 25f;
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float easingAmount = 0.1f;

        [Header("Camera")]
        [SerializeField] private Vector3 offsetFromPlayer = new(5f, 3f, 0f);
        [SerializeField] private bool doZoomForSpeed = false;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            // Follow target
            Vector3 targetPos = player.transform.position + offsetFromPlayer;
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

            // Zoom based on target's speed
            if (doZoomForSpeed)
            {
                float zoomLevel = Mathf.Lerp(maxZoom, minZoom, player.RigidBody.velocity.magnitude / zoomSpeed);
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, zoomLevel, easingAmount);
            }
        }
    }
}
