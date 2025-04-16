using UnityEngine;
using System;
using Newtonsoft.Json;

namespace PuzzleData.Save
{
    [Serializable]
    public class Vector3S
    {
        private float _x, _y, _z;

        public float X => _x;
        public float Y => _y;
        public float Z => _z;

        public Vector3S(Vector3 vector3)
        {
            _x = vector3.x;
            _y = vector3.y;
            _z = vector3.z;
        }

        [JsonConstructor]
        public Vector3S(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(_x, _y, _z);
        }
    }
}