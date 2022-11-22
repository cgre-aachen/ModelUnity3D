using UnityEngine;
using System;
using PlasticPipe;


namespace Gempy
{
    public class SurfacePoint: MonoBehaviour, IComparable<SurfacePoint>
    {
        // DataAssembler _surfaceId;
        // public int Category;
        public int SurfaceId;
        public int Series;
        private Vector3 spawnPos;
        private Vector3 _lastPos;
        private Quaternion _lastRot;
        private bool isMoving = false;
        private bool hasstopped = false;
        public DataAssembler dataAssembler;
        public int interval = 250;
        private long lasttimechecked = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
        
        public int CompareTo(SurfacePoint other)
        {
            int compareTo = SurfaceId.CompareTo(other.SurfaceId);
            return compareTo;
        }

        void Start()
        {
            _lastPos = gameObject.transform.position;
            _lastRot = gameObject.transform.rotation;
            isMoving = false;
        }

        void Update()
        {
            var currenttime = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            
            if (currenttime <= lasttimechecked + interval)
            {
                return;
            }

            lasttimechecked = currenttime;
            
            var currentpos = gameObject.transform.position;
            var currentrot = gameObject.transform.rotation;
            if (isMoving)
            {
                if (currentpos == _lastPos & currentrot == _lastRot)
                {
                    hasstopped = true;
                    isMoving = false;
                }
            }

            if (currentpos != _lastPos ^ currentrot != _lastRot)
            {
                isMoving = true;
            }

            _lastPos = currentpos;
            _lastRot = currentrot;
            switch ((isMoving, hasstopped))
            {
                case (isMoving: true, hasstopped: false):
                    break;
                case (isMoving: false, hasstopped: true):
                    dataAssembler.UpdateInputdata = true;
                    hasstopped = false;
                    break;
            }
        }
    }
}