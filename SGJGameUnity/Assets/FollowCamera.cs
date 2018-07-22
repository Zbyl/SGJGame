using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
    public static FollowCamera instance;

    public Transform target;
    public float springFactor = 0.7f;
    public float craneLength = 10.0f;
    public float craneSpringFactor = 0.7f;
    public float maxCraneSpeed = 3.0f;

    private Vector3 noCranePosition;
    private Vector3 cranePosition = Vector3.zero;

    void Awake()
    {
        if (FollowCamera.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        FollowCamera.instance = this;
    }

    // Use this for initialization
    void Start () {
        noCranePosition = transform.position;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (target == null)
        {
            return;
        }

        var distance = target.position - noCranePosition;
        if (distance.magnitude < 0.01f)
        {
            distance = Vector3.zero;
        }

        {
            var dir = distance.normalized;
            var correction = dir * distance.magnitude * springFactor;
            correction = new Vector3(correction.x, correction.y, 0.0f);
            noCranePosition += correction;
        }

        var craneDistance = target.right * craneLength - cranePosition;
        if (craneDistance.magnitude < 0.01f)
        {
            craneDistance = Vector3.zero;
        }

        {
            var dir = craneDistance.normalized;
            var correction = dir * craneDistance.magnitude * craneSpringFactor;
            if (correction.magnitude > maxCraneSpeed)
            {
                correction = correction.normalized * maxCraneSpeed;
            }
            correction = new Vector3(correction.x, correction.y, 0.0f);
            cranePosition += correction;
        }

        transform.position = noCranePosition + cranePosition;
    }
}
