using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class TestTrajectory : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Transform Target;
    [SerializeField] Vector3 vel;
    Vector3 ForceDir;
    [SerializeField] float ForceMultiplier;
    LineRenderer lineRenderer;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            ApplyForce();
            //rb.velocity = vel;
        }
    }

    public void ApplyForce()
    {
        //ForceDir = Target.position - transform.position;
        Vector3[] points = Trajectory.UpdateTraj(vel * ForceMultiplier, rb, transform.position);
        print("points from trajectory is " + points.Length);
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        print("start trajectory is " + points[0]);
        print("end trajectory is " + points[points.Length - 1]);
        rb.AddForce(vel * ForceMultiplier);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Damage damage))
        {
            print("damage hit " + gameObject);
            ForceDir = other.transform.forward;
            Vector3[] points = Trajectory.UpdateTraj(ForceDir * ForceMultiplier, rb, transform.position);
            print("points from trajectory is " + points.Length);
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
            print("start trajectory is " + points[0]);
            print("end trajectory is " + points[points.Length - 1]);
            rb.AddForce(ForceDir * ForceMultiplier);
        }
    }

}

    
