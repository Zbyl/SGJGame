using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobinHoodController : MonoBehaviour {

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
    }

    // Require the rocket to be a rigidbody.
    // This way we the user can not assign a prefab without rigidbody
    public Transform arrowStart;
    public Rigidbody2D rocket;
    public float speed = 10f;
    public float topSpeed = 10f;

    Animator animator;

    private Vector3 direction = Vector3.right;
    private Vector3 shootDir;
    private int d = 0;

    private bool allowShoot = true;

    void FireRocket()
    {
        var rot = Quaternion.FromToRotation(-arrowStart.right, shootDir);
        Rigidbody2D rocketClone = (Rigidbody2D)Instantiate(rocket, arrowStart.position, rot);
        rocketClone.velocity = shootDir * speed;

        // You can also access other components / scripts of the clone
        //rocketClone.GetComponent<MyRocketScript>().DoSomething();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            //FireRocket();
            shootDir = direction;
            allowShoot = true;
            Debug.Log("BowLeft");
            animator.Play("BowLeft");
        }
    }

    // Physics in FixedUpdate
    void FixedUpdate()
    {
        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");
        var nd = (transform.right * moveH + transform.up * moveV).normalized;
        if (nd.magnitude > 0.01f)
            direction = nd;
        GetComponent<Rigidbody2D>().velocity = transform.right * moveH * topSpeed + transform.up * moveV * topSpeed;

        animator.SetFloat("Speed", Mathf.Sqrt(moveH * moveH + moveV * moveV));

        var oldD = d;
        if (Mathf.Abs(moveH) > Mathf.Abs(moveV))
            if (moveH > 0)
                d = 0;
            else
                d = 2;
        else
            if (moveV < 0)
                d = 1;
            else
                d = 3;

        if (d != oldD)
        {
            if (d == 0)
                animator.Play("WalkRight");
            else
            if (d == 1)
                animator.Play("WalkDown");
            else
            if (d == 2)
                animator.Play("WalkLeft");
            else
            if (d == 3)
                animator.Play("WalkUp");
        }

        animator.SetInteger("Direction", d);
    }

    public void ShootArrow(string s)
    {
        Debug.Log("PrintEvent: " + s + " called at: " + Time.time);
        if (allowShoot)
        {
            allowShoot = false;
            FireRocket();
        }
    }
}
