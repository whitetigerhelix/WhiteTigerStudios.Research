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
        private Transform _playerTransform;
        private Rigidbody _playerRigidbody;

        private Vector3 _currentVelocity;

        private void Awake()
        {
            _camera = Camera.main;
            _playerTransform = player.transform;
            _playerRigidbody = player.RigidBody;
        }

        private void LateUpdate()
        {
            // Follow target
            Vector3 targetPos = _playerTransform.position + offsetFromPlayer;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _currentVelocity, followSpeed * Time.deltaTime);

            // Zoom based on target's speed
            if (doZoomForSpeed)
            {
                float zoomLevel = Mathf.Lerp(maxZoom, minZoom, _playerRigidbody.velocity.magnitude / zoomSpeed);
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, zoomLevel, easingAmount);
            }
        }
    }
}
