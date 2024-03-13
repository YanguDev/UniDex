using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UniDex.Animations
{
    public class RotatingObject : MonoBehaviour
    {
        public enum RotationAxis
        {
            X,
            Y,
            Z
        }

        [SerializeField]
        private int degreesPerSecond = 30;
        [SerializeField]
        private RotationAxis rotationAxis;

        private void Update()
        {
            float rotation = degreesPerSecond * Time.deltaTime;
            Vector3 eulerAngles = rotationAxis switch
            {
                RotationAxis.X => new Vector3(rotation, 0, 0),
                RotationAxis.Y => new Vector3(0, rotation, 0),
                RotationAxis.Z => new Vector3(0, 0, rotation),
                _ => Vector3.zero
            };

            transform.Rotate(eulerAngles);
        }
    }
}
