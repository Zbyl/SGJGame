using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPropagator : MonoBehaviour {

    private RobinHoodController robinHoodController;

	// Use this for initialization
	void Start () {
        robinHoodController = gameObject.GetComponentInParent<RobinHoodController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Shoot()
    {
        //Debug.Log("Propagating shot.");
        robinHoodController.Shoot();
    }
}
