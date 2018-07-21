using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPropagator : MonoBehaviour {

    private SoldierController soldierController;

	// Use this for initialization
	void Start () {
        soldierController = gameObject.GetComponentInParent<SoldierController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Shoot()
    {
        //Debug.Log("Propagating shot.");
        soldierController.Shoot();
    }

    void Finished()
    {
        //Debug.Log("Propagating shot.");
        soldierController.ShootFinished();
    }
}
