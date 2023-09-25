// Copyright (c) 2023 - WhitetigerStudios
using UnityEngine;

namespace WhitetigerStudios.Research.Platformer
{
    /// <summary>
    /// The world surface that the platformer player can interact with
    /// </summary>
    public class PlatformerWorldObject : MonoBehaviour
    {
        [Header("Platformer World Object Definition")]

        [SerializeField, Tooltip("A human readable name for this world object")]
        private string surfaceName = "DefaultPlatformerWorldObject";
        public string SurfaceName => surfaceName;
    }
}
