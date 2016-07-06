using UnityEngine;

namespace GuestSong
{
    public class AutoRotate : MonoBehaviour
    {
        public Vector3 direction = Vector3.back;
        public float Speed = 50f;

        void Update()
        {
            transform.Rotate(direction * Time.deltaTime * Speed);
        }
    }
}