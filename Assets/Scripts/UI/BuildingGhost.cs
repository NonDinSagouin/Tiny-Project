using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyProject.UI
{
    public class BuildingGhost : MonoBehaviour
    {
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;

            foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            {
                Color c = sr.color;
                c.a = 0.5f;
                sr.color = c;
            }
        }

        private void Update()
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
            worldPos.z = 0f;
            transform.position = worldPos;

            if (Keyboard.current.escapeKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                BuildSlots.ClearGhost();
            }
        }

        private void OnDestroy()
        {
            BuildSlots.ClearGhostReference();
        }
    }
}
