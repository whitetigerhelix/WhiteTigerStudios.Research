// Copyright (c) 2023 - WhitetigerStudios
using System.Collections;
using UnityEngine;

namespace WhitetigerStudios.Research.Platformer
{
    /// <summary>
    /// Platform disappears after a configured amount of time if a player walks on it for long enough.
    /// </summary>
    public class DisappearingPlatform : PlatformerWorldObject
    {
        [Header("Disappearing Platform")]

        [SerializeField]
        private float disappearAfterTime = 0f;

        [SerializeField]
        private float disappearTime = 2f;

        [SerializeField]
        private bool reappear = true;

        [SerializeField]
        private Renderer myRenderer;

        [SerializeField]
        private Collider myCollider;

        private float alpha = 1f;
        public float Alpha
        {
            get => alpha;
            private set
            {
                alpha = value;

                Color color = myRenderer.material.color;
                color.a = alpha;
                myRenderer.material.color = color;
            }
        }

        private bool playerIsOnPlatform = false;
        public bool PlayerIsOnPlatform
        {
            get => playerIsOnPlatform;
            private set
            {
                if (playerIsOnPlatform != value)
                {
                    playerIsOnPlatform = value;

                    if (playerIsOnPlatform)
                    {
                        StartWaitToDisappearCoroutine();
                    }
                    else
                    {
                        StopWaitToDisappearCoroutine();
                    }
                }
            }
        }

        public Transform PlayerAnchorTransform => PlatformerPlayerController.Instance.Player.PlayerAnchor;

        private Coroutine waitToDisappearCoroutine;
        private bool isDisappearing;

        private void Update()
        {
            HandleDisappearing();
        }

        public void StartWaitToDisappearCoroutine()
        {
            if (waitToDisappearCoroutine == null)
            {
                waitToDisappearCoroutine = StartCoroutine(WaitToDisappearCoroutine());
            }
        }

        public void StopWaitToDisappearCoroutine()
        {
            if (waitToDisappearCoroutine != null)
            {
                StopCoroutine(waitToDisappearCoroutine);
                waitToDisappearCoroutine = null;
            }
        }

        private IEnumerator WaitToDisappearCoroutine()
        {
            yield return new WaitForSeconds(disappearAfterTime);

            isDisappearing = true;
            waitToDisappearCoroutine = null;
        }

        private void HandleDisappearing()
        {
            float delta = disappearTime * Time.deltaTime;

            if (isDisappearing)
            {
                // Keep disappearing...
                if (Alpha > 0f)
                {
                    float newAlpha = Alpha - delta;
                    if (newAlpha <= 0f)
                    {
                        // Reached the end - stop disappearing, disable collider
                        Alpha = 0f;
                        myCollider.enabled = false;
                        isDisappearing = false;
                    }
                    else
                    {
                        Alpha = newAlpha;
                    }
                }
            }

            // If the player is not on the platform, we're no longer disappearing, try to reappear
//TODO: Timer before reappearing
            if (!PlayerIsOnPlatform && !isDisappearing && reappear)
            {
                if (Alpha < 1f)
                {
                    Alpha = Mathf.Clamp(Alpha + delta, 0f, 1f);
                }

                myCollider.enabled = Alpha > float.Epsilon;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == PlayerAnchorTransform)
            {
                PlayerIsOnPlatform = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform == PlayerAnchorTransform)
            {
                PlayerIsOnPlatform = false;
            }
        }
    }
}
