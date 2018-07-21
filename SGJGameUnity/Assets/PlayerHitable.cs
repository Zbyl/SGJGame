using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitable : Hitable
{
    Animator feetAnimator;
    Animator bodyAnimator;

    // Use this for initialization
    void Start () {
        feetAnimator = transform.Find("Feet").GetComponent<Animator>();
        bodyAnimator = transform.Find("Body").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    protected override void onDead()
    {
        gameObject.GetComponent<PlayerSoldierController>().enabled = false;
        //gameObject.GetComponent<Collider2D>().enabled = false;
        bodyAnimator.Play("DeadAnimation");
        feetAnimator.Play("FeetIdleAnimation");
    }
}
