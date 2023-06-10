using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Location", menuName = "Location")]
    public class Location : ScriptableObject
    {
        [Header("General Settings")]
        [Tooltip("Name of the location (make sure you have a gameObject instead of your location object with the same name).")]
        public new string name;
        [Tooltip("Skybox of the location.")]
        public Material skybox;
        [Header("Fog Settings")]
        [Tooltip("Should the location have fog?")]
        public bool fog = false;
        [Tooltip("Color of the fog.")]
        public Color fogColor;
        [Tooltip("Density of the fog.")]
        public float fogDensity;
        [Header("Character Settings")]
        
        [Tooltip("Can the characters move?")]
        public bool charactersCanMove = true;
        
        [Tooltip("Can the characters look around?")]
        public bool charactersCanLook = true;
        
        [Tooltip("Should the characters be placed in specific positions?")]
        public bool characterPositionsEnabled;
        
        [Tooltip("The positions of the characters in the scene.")]
        public Vector3[] characterPositions;
        
        [Tooltip("The container that holds the characters. Leave null for the characters to be top level")]
        public string characterContainter;
    }
}
