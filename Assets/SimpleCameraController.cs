using System;
using UnityEngine;

namespace Scenes.TurretTestScene
{
    public class SimpleCameraController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float moveSpeed;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update() {
            var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * Time.smoothDeltaTime;
            transform.Rotate(Vector3.up, mouseDelta.x * rotationSpeed, Space.World);
            transform.Rotate(Vector3.right, -mouseDelta.y * rotationSpeed, Space.Self);

            var moveFactorZ = Input.GetKey(KeyCode.W) ? 1
                :Input.GetKey(KeyCode.S)  ? -1 : 0;
            
            var moveFactorX = Input.GetKey(KeyCode.A) ? -1
                : Input.GetKey(KeyCode.D) ? 1 : 0;
            transform.Translate(Vector3.forward * (Time.deltaTime * moveSpeed * moveFactorZ));

            var xMoveVector = transform.right;
            xMoveVector.y = 0;
            transform.Translate(xMoveVector * (Time.deltaTime * moveSpeed * moveFactorX), Space.World);
        }
    }
} 
