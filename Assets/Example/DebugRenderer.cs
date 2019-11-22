using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Urg
{
    public class DebugRenderer : MonoBehaviour
    {
        public UrgSensor urg;
        const float maxDis = 2.8f;
        private float[] distances;
        private List<DetectedLocation> locations = new List<DetectedLocation>();
        private AffineConverter affineConverter;
        private List<GameObject> debugObjects;
        private Object syncLock = new Object();

        public Canvas canvas;
        public float xSize;
        public float ySize;
        public float yOffset;

        public GameObject playerPrefab;
        List<GameObject> players;
        float screenWidth;
        float screenHeight;
        Vector2 hidePlayerPos = new Vector2(-100, -100);

        int touchCount = 0;
        void Awake()
        {
            players = new List<GameObject>();
            // delegate method to receive raw distance data from sensor.
            urg.OnDistanceReceived += Urg_OnDistanceReceived;

            // delegate method to receive filtered detected locations.
            urg.OnLocationDetected += Urg_OnLocationDetected;

            urg.AddFilter(new SpatialMedianFilter(3));
            urg.AddFilter(new ClusteringFilter(0.15f));

            var cam = Camera.main;
            var plane = new Plane(Vector3.up, Vector3.zero);

            var sensorCorners = new Vector2[4];
            sensorCorners[0] = new Vector2(1.5f, 0.5f);
            sensorCorners[1] = new Vector2(1.5f, -0.5f);
            sensorCorners[2] = new Vector2(0.5f, -0.5f);
            sensorCorners[3] = new Vector2(0.5f, 0.5f);

            var worldCorners = new Vector3[4];
            worldCorners[0] = Screen2WorldPosition(new Vector2(0, Screen.height), cam, plane);
            worldCorners[1] = Screen2WorldPosition(new Vector2(Screen.width, Screen.height), cam, plane);
            worldCorners[2] = Screen2WorldPosition(new Vector2(Screen.width, 0), cam, plane);
            worldCorners[3] = Screen2WorldPosition(new Vector2(0, 0), cam, plane);
            affineConverter = new AffineConverter(sensorCorners, worldCorners);

            debugObjects = new List<GameObject>();
            for (var i = 0; i < 100; i++)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.parent = transform;
                obj.transform.localScale = 0.3f * Vector3.one;
                debugObjects.Add(obj);
            }
           
            
        }

        private void Start()
        {
            screenHeight = canvas.GetComponent<RectTransform>().rect.height;
            screenWidth = canvas.GetComponent<RectTransform>().rect.width;
        }

        void Update()
        {
            if (urg == null)
            {
                return;
            }


            if (distances != null && distances.Length > 0)
            {
                bool isLastOneRendered = false;
                List<int> set = new List<int>();
                for (int i = 0; i < distances.Length; i++)
                {
                    float distance = distances[i];
                    float angle = urg.StepAngleRadians * i + urg.OffsetRadians;
                    float degree = angle * 57.2958f;
                    var cos = Mathf.Cos(angle);
                    var sin = Mathf.Sin(angle);
                    var dir = new Vector3(cos, 0, sin);
                    var pos = distance * dir;
                    if (distances[i] < maxDis && degree >= -90 && degree <= 90 )
                    {
                        //Debug.Log("angle: " + angle);
                        set.Add(i);
                        //if(!isLastOneRendered)
                        //    Debug.DrawRay(urg.transform.position, pos, Color.blue);
                        //isLastOneRendered = true;
                    }
                    else
                    {
                        if(set.Count > 1)
                        {
                            //find middle of touch
                            int median = Mathf.RoundToInt((set[0] + set[set.Count - 1]) / 2);

                            float distanceRen = distances[median];
                            float angleRen = urg.StepAngleRadians * median + urg.OffsetRadians;
                            var cosRen = Mathf.Cos(angleRen);
                            var sinRen = Mathf.Sin(angleRen);
                            var dirRen = new Vector3(cosRen, 0, sinRen);
                            var posRen = distance * dirRen;

                            DetectedLocation location = new DetectedLocation(median, angleRen, distanceRen);

                            touchCount++;
                            //Debug.Log("xy : " + location.ToScreenSpace(xSize, ySize, screenWidth, screenHeight, yOffset) + "degree:" + location.degree);
                            if(players.Count < touchCount)
                            {
                                GameObject player = Instantiate(playerPrefab);
                                players.Add(player);
                                player.transform.SetParent(canvas.transform);
                            }
                            players[touchCount-1].GetComponent<RectTransform>().position = location.ToScreenSpace(xSize, ySize, screenWidth, screenHeight, yOffset);
                                

                                
                            Debug.DrawRay(urg.transform.position, posRen, Color.blue);
                        }
                        //isLastOneRendered = false;
                        set.Clear();
                    }
                        
                        
                }
                if(touchCount < players.Count)
                {
                    while(touchCount < players.Count)
                    {
                        players[touchCount].GetComponent<RectTransform>().position = hidePlayerPos;

                        touchCount++;
                    }
                }
                touchCount = 0;
            }

            var locs = this.locations;
            int index = 0;
            foreach (var loc in locs)
            {
                Vector3 worldPos = new Vector3(0, 0, 0);
                var inRegion = affineConverter.Sensor2WorldPosition(loc.ToPosition2D(), out worldPos);
                if (inRegion && index < debugObjects.Count)
                {
                    //Gizmos.DrawCube(worldPos, new Vector3(0.1f, 0.1f, 0.1f));
                    //debugObjects[index].transform.position = worldPos;
                    index++;
                }
            }

            for (var i = index; i < debugObjects.Count; i++)
            {
                debugObjects[i].transform.position = new Vector3(100, 100, 100);
            }
        }

        void Urg_OnDistanceReceived(float[] rawDistances)
        {
            this.distances = rawDistances;
        }

        void Urg_OnLocationDetected(List<DetectedLocation> locations)
        {
            // this is called outside main thread.
            this.locations = locations;
        }

        private static Vector3 Screen2WorldPosition(Vector2 screenPosition, Camera camera, Plane basePlane)
        {
            var ray = camera.ScreenPointToRay(screenPosition);
            var distance = 0f;

            if (basePlane.Raycast(ray, out distance))
            {
                var p = ray.GetPoint(distance);
                return p;
            }
            return Vector3.negativeInfinity;
        }
    }
}
