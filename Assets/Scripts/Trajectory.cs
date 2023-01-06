using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Trajectory
{

    static int LineSegmentCount = 20;

    public static Vector3[] UpdateTraj(Vector3 forcevector, float Mass, Vector3 StartPos)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 velocity = (forcevector / Mass) * Time.fixedDeltaTime;
        //float flightdur = (2 * velocity.y) / Physics.gravity.y;
        float flightdur = -10;
        float stepTime = flightdur / LineSegmentCount;

        for (int i = 0; i < LineSegmentCount; i++)
        {
            float steptimepassed = stepTime * i;
            Vector3 MoveVector = new Vector3(
                velocity.x * steptimepassed,
                velocity.y * steptimepassed - .5f * Physics.gravity.y * steptimepassed * steptimepassed,
                velocity.z * steptimepassed
                );
            points.Add(-MoveVector + StartPos);
        }
        //give line renderer array length
        //set line renderer positions

        return points.ToArray();
    }

    public static Vector3[] UpdateTraj(Vector3 forcevector, Rigidbody rb, Vector3 startpos)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 velocity = (forcevector / rb.mass) * Time.fixedDeltaTime;
        //float flightdur = (2 * velocity.y) / Physics.gravity.y;
        float flightdur = -10;
        float stepTime = flightdur / LineSegmentCount;

        for (int i = 0; i < LineSegmentCount; i++)
        {
            float steptimepassed = stepTime * i;
            Vector3 MoveVector = new Vector3(
                velocity.x * steptimepassed,
                velocity.y * steptimepassed - .5f * Physics.gravity.y * steptimepassed * steptimepassed,
                velocity.z * steptimepassed
                ) ;
            points.Add(-MoveVector + startpos);
        }
        //give line renderer array length
        //set line renderer positions

        return points.ToArray();
    }
}
