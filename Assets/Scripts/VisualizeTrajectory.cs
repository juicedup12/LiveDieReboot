using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class VisualizeTrajectory : MonoBehaviour
{
    protected Rigidbody rb;
    protected LineRenderer lr;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
    }


    public void ApplyForce(Vector3 Velocity)
    {
        //ForceDir = Target.position - transform.position;
        Vector3[] points = Trajectory.UpdateTraj(Velocity , rb, transform.position);
        print("points from trajectory is " + points.Length);
        lr.positionCount = points.Length;
        lr.SetPositions(points);
        print("start trajectory is " + points[0]);
        print("end trajectory is " + points[points.Length - 1]);
        rb.AddForce(Velocity, ForceMode.Force);
    }
}
