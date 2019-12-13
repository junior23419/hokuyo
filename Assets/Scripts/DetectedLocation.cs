using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urg
{
    public class DetectedLocation : ICloneable
    {
        public int index;
        public float angle;
        public float distance;
        public float degree;

        public DetectedLocation(int index, float angle, float distance)
        {
            this.index = index;
            this.angle = angle;
            this.distance = distance;
            this.degree = (angle * 57.2958f)+90;
        }

        public Vector2 ToScreenSpace(float xSize,float ySize,float screenWidth, float screenHeight,float yOffset,float xOffset,bool flipX,bool flipY)
        {
            Vector2 actual = ToActualSpace();
            float x = (((actual.x - xOffset) / (xSize / 2)) * (screenWidth / 2));
            float y = (((actual.y - yOffset) / ySize) * screenHeight) - (screenHeight / 2);
            return new Vector2(flipX?-x:x,flipY?-y:y);
        }
        
        public Vector2 ToActualSpace()
        {
            float x = distance * Mathf.Cos(degree / 57.2958f);
            float y = distance * Mathf.Sin(degree / 57.2958f);
            return new Vector2(x, y);
        }

        public Vector2 ToPosition2D()
        {
            var pos3d = ToPosition();
            return new Vector2(pos3d.x, pos3d.z);
        }

        public Vector3 ToPosition()
        {
            return ToPosition(Vector3.right, Vector3.up);
        }

        public Vector3 ToPosition(Vector3 forward, Vector3 normal)
        {
            return distance * (Quaternion.AngleAxis(-angle, normal) * forward);
        }

        public object Clone()
        {
            return new DetectedLocation(this.index, this.angle, this.distance);
        }
    }
}
