using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchVisualizer : MonoBehaviour
{
    LineRenderer lr;
    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }
    public void ShowRigidBodyTrajectory(Rigidbody[] rb)
    {
        //StartCoroutine(FrameWait(rb));
        StartCoroutine(LineUpdateRoutine(rb));
        //updatelines(rb);
    }

    //add up (rb.mass * rb.velocity) for each body
    //and divide by the total mass
    void GetRigidbodyAverageVelocity(Rigidbody[] rbs)
    {
        float TotalMass = 0;
        Vector3 WeightedVel = Vector3.zero;
        foreach (Rigidbody rigidbody in rbs)
        {
            TotalMass += rigidbody.mass;
            WeightedVel += rigidbody.mass * rigidbody.velocity;
        }
        Vector3 TotalVelocity = WeightedVel / TotalMass;
        print("total velocity is " + TotalVelocity);
        Vector3[] points = Trajectory.UpdateTraj(TotalVelocity, TotalMass/rbs.Length, transform.position);
        lr.positionCount = points.Length;
        print("points from trajectory is " + points.Length);
        print("start trajectory is " + points[0]);
        print("end trajectory is " + points[points.Length - 1]);
        lr.SetPositions(points);

    }

    IEnumerator LineUpdateRoutine(Rigidbody[] rbs)
    {

        GetRigidbodyAverageVelocity(rbs);
        yield return null;
        GetRigidbodyAverageVelocity(rbs);
        yield return null;
        GetRigidbodyAverageVelocity(rbs);
        yield return null;
        GetRigidbodyAverageVelocity(rbs);
        yield return null;
        GetRigidbodyAverageVelocity(rbs);
        yield return null;
    }

    IEnumerator FrameWait(Rigidbody rb)
    {
        updatelines(rb);
        //yield return new WaitForEndOfFrame();
        yield return null;
        updatelines(rb);
        yield return null;
        updatelines(rb);
        yield return null;
        updatelines(rb);
    }


    void updatelines(Rigidbody rb)
    {
        print("rb velocity is " + rb.velocity);
        Vector3[] points = Trajectory.UpdateTraj(rb.velocity, rb, transform.position);
        lr.positionCount = points.Length;
        print("points from trajectory is " + points.Length);
        print("start trajectory is " + points[0]);
        print("end trajectory is " + points[points.Length - 1]);
        lr.SetPositions(points);
    }
}
