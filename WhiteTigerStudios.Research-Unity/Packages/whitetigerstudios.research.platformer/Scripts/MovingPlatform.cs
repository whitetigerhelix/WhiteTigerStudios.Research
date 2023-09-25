// Copyright (c) 2023 - WhitetigerStudios
using UnityEngine;
using UnityEngine.Events;

namespace WhitetigerStudios.Research.Platformer
{
    /// <summary>
    /// Object that moves back and forth between two points.
    /// </summary>
    public class MovingPlatform : PlatformerWorldObject
    {
        [Header("Moving Platform")]
        
        public Transform LeftEdge;
        public Transform RightEdge;

        public Transform LeftTarget;
        public Transform RightTarget;

        public float Speed = 1f;

        public UnityEvent OnDestinationReached;

        private float time;
        private float halfWidth;
        private const float EPSILON = 0.001f;
        
        private void Start()
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                // Assuming the local scale affects the x-axis as it does.
                halfWidth = (boxCollider.size.z * transform.localScale.z) / 2f;
            }
        }

        private void Update()
        {
            time += Time.deltaTime * Speed;
            float easedT = 0.5f * (1f - Mathf.Cos(Mathf.PI * Mathf.PingPong(time, 1f)));

            Vector3 fixedLeftTarget = LeftTarget.position + new Vector3(0f, 0f, halfWidth);
            Vector3 fixedRightTarget = RightTarget.position - new Vector3(0f, 0f, halfWidth);

            Vector3 target = Vector3.Lerp(fixedRightTarget, fixedLeftTarget, easedT);
            transform.position = target;

            //Debug.Log($"MovingPlatform - halfWidth: {halfWidth}, fixedLeftTarget: {fixedLeftTarget}, fixedRightTarget: {fixedRightTarget}, target: {target}, LeftEdge: {LeftEdge.position}, RightEdge: {RightEdge.position}");

            // Check if the edge reached either point, considering a small epsilon.
            if (Mathf.Abs(LeftEdge.position.z - fixedLeftTarget.z) <= EPSILON || 
                Mathf.Abs(RightEdge.position.z - fixedRightTarget.z) <= EPSILON)
            {
                OnDestinationReached?.Invoke();
            }
        }
    }
}
