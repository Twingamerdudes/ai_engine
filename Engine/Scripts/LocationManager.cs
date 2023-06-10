using Cinemachine;
using ScriptableObjects;
using UnityEngine;

namespace Engine.Scripts
{
    public class LocationManager : MonoBehaviour
    {
        public static void GetBackgroundInfo(Location location, GameObject locationObj, Manager.Character[] characters, Transform vcam)
        {
            foreach (Transform child in locationObj.transform)
            {
                child.gameObject.SetActive(child.name == location.name);
            }
            RenderSettings.skybox = location.skybox;
            RenderSettings.fog = location.fog;
            RenderSettings.fogColor = location.fogColor;
            RenderSettings.fogDensity = location.fogDensity;
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].character.parent = null;
                //check if location length is greater than the current character index
                if (location.characterPositionsEnabled)
                {
                    if (location.characterPositions.Length >= i + 1)
                    {
                        characters[i].character.position = location.characterPositions[i];
                    }
                }
                else
                {
                    characters[i].character.position = Manager.originalPositions[i];
                }

                characters[i].character.LookAt(vcam.transform);
                characters[i].character.GetComponent<Character>().isMovementAllowed = location.charactersCanMove;
                characters[i].character.GetComponent<Character>().isLookingAllowed = location.charactersCanLook;
                if (location.characterContainter != "")
                {
                    characters[i].character.parent = GameObject.Find(location.characterContainter).transform;
                }
            }
        }
    }
}
