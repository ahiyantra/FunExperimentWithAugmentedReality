using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class ARLessonsScript : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text StateText;

    [SerializeField] TMPro.TMP_Text PlaneText;

    [SerializeField] ARPlaneManager mArPlaneManager;

    [SerializeField] ARPointCloudManager mArPointCloudManager;

    ARRaycastManager arRaycastManager;

    [SerializeField] GameObject robotPrefab;

    private GameObject go = null;

    [SerializeField] ARCameraManager aRCameraManager;

    [SerializeField] Light mLight;

    [SerializeField] Image image;

    [SerializeField] GameObject ballPrefab;

    int numPlanesAdded = 0, numPlanesUpdated = 0, numPlanesRemoved = 0, numPointsUpdated = 0;

    // code for tutorial start

    [SerializeField] Canvas infoCanvas;

    // code for tutorial end

    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        mArPlaneManager.planesChanged += onPlanesChanged;

        mArPointCloudManager.pointCloudsChanged += onPointCloudsChanged;

        ARSession.stateChanged += Change;

        aRCameraManager.frameReceived += onCameraFrameChanged;

        // code for tutorial start

        GameObject tempObject = GameObject.Find("Canvas_(1)");

        infoCanvas = tempObject.GetComponent<Canvas>();

        // code for tutorial end

    }

    void onCameraFrameChanged(ARCameraFrameEventArgs args)
    {
        if(args.lightEstimation.averageBrightness.HasValue)
        {
            mLight.intensity = args.lightEstimation.averageBrightness.Value;
        }

        if(args.lightEstimation.averageColorTemperature.HasValue)
        {
            mLight.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
        }

        if(args.lightEstimation.colorCorrection.HasValue)
        {
            image.color = args.lightEstimation.colorCorrection.Value;
            mLight.color = args.lightEstimation.colorCorrection.Value;
        }
    }

    void Change(ARSessionStateChangedEventArgs e)
    {
        print("state changed to : " + e.state);
        StateText.SetText(e.state.ToString());
    }

    void onPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach(var planesAdded in args.added)
        {
            numPlanesAdded += 1;
        }

        foreach(var planesUpdated in args.updated)
        {
            numPlanesUpdated += 1;
        }

        foreach(var planesRemoved in args.removed)
        {
            numPlanesRemoved += 1;
        }
    }

    void onPointCloudsChanged(ARPointCloudChangedEventArgs args)
    {
        foreach(var pointCloudUpdated in args.updated)
        {
            numPointsUpdated += 1;
        }
    }

    private void Update()
    {
        //UpdateTextPlanes();

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            var hits = new List<ARRaycastHit>();

            arRaycastManager.Raycast(ray, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

            // code for blocking raycasts start

            var touchPosition = Input.GetTouch(0).position;

            bool isOverUI = touchPosition.IsPointOverUIObject();

            if(!isOverUI && arRaycastManager.Raycast(ray, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes)) // touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon
            {
                ///*
                if(hits.Count > 0)
                {
                    var pos = hits[0].pose.position;

                    if(go != null)
                    {
                        go.gameObject.transform.position = pos;
                    }
                    else
                    {
                         go = Instantiate(robotPrefab, pos, Quaternion.identity) as GameObject;

                        Rigidbody rigidbody = go.GetComponent<Rigidbody>();

                        rigidbody.isKinematic = false; // true
                        rigidbody.velocity = Vector3.zero;
                        rigidbody.angularVelocity = Vector3.zero;
                    }
                }
                //*/
            }

            // code for blocking raycasts end

            /*
            if(hits.Count > 0)
            {
                var pos = hits[0].pose.position;

                if(go != null)
                {
                    go.gameObject.transform.position = pos;
                }
                else
                {
                     go = Instantiate(robotPrefab, pos, Quaternion.identity) as GameObject;

                    Rigidbody rigidbody = go.GetComponent<Rigidbody>();

                    rigidbody.isKinematic = false; // true
                    rigidbody.velocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                }
            }
            */
            
        }
    }

    // Function to Shoot the Ball
    public void ShootBall()
    {
        GameObject newBall = Instantiate<GameObject>(ballPrefab);
        newBall.transform.position = Camera.main.transform.position;
        Rigidbody rb = newBall.GetComponent<Rigidbody>();
        rb.AddForce(5000 * Camera.main.transform.forward);
    }

    // Function for the Tutorial
    public void Tutorial()
    {
        
        infoCanvas.enabled = !infoCanvas.enabled;
        
    }

    // Function to update the Plane Text
    private void UpdateTextPlanes()
    {
        PlaneText.SetText("Planes Added = " + numPlanesAdded + " Planes Updated = " + numPlanesUpdated + " Planes Removed = " + numPlanesRemoved + " Point Clouds Updated = " + numPointsUpdated);
    }
}
