using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//need to add particles to closest point
public class Laser : MonoBehaviour
{

    [SerializeField] float MaxRange;
    LineRenderer lr;
    //use this to visualize hit point
    [SerializeField] Transform ParticleTransform;
    [SerializeField] Transform ColliderParent;
    bool HasCollided;
    Collider col;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        //lr.SetPosition(1, transform.position + transform.up * MaxRange);
        UpdateLine(MaxRange);
        ParticleTransform.position = transform.position + transform.up * MaxRange;
    }


    [ContextMenu("update line")]
    public void UpdateLine()
    {
        print("setting line to max range");
        lr.SetPosition(1, transform.position + transform.up * MaxRange);
        ParticleTransform.position = transform.position + transform.up * MaxRange;
        ColliderParent.localScale = new Vector3(ColliderParent.localScale.x, MaxRange, ColliderParent.localScale.z);

    }

    public void UpdateLine(float length)
    {
        print("updating line to " + length);
        lr.SetPosition(1, transform.position + transform.up * length);
        ParticleTransform.position = transform.position + transform.up * length;
        ColliderParent.localScale = new Vector3(ColliderParent.localScale.x, length, ColliderParent.localScale.z);
    }


    // Update is called once per frame
    void Update()
    {
        if(HasCollided)
        {
            if(!col)
            {
                print("col has been removed");
                HasCollided = false;
                UpdateLine();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ragdoll") || other.CompareTag("limb") || other.CompareTag("Player"))
        {
            print("trigger hit" + other.gameObject);
            HasCollided = true;
            col = other;
        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ragdoll") || other.CompareTag("limb"))
        {
            //print("laser hit " + other);
            Vector3 p = other.ClosestPointOnBounds(transform.position);
            float dist = Vector3.Distance(other.transform.position, transform.position);
            //lr.SetPosition(1, transform.position + transform.up * dist);
            UpdateLine(dist);
            HasCollided = true;
            col = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print(other + " exited laser");
        if (other.CompareTag("Ragdoll") || other.CompareTag("limb") || other.CompareTag("Player"))
        {
            //lr.SetPosition(1, transform.position + transform.up * MaxRange);
            UpdateLine();
            if(col == other)
            {
                col = null;
                HasCollided = false;
            }
        }
        UpdateLine();
    }

    private void OnCollisionExit(Collision collision)
    {
        print(collision.gameObject + "exited laser");
        UpdateLine();
        if (col == collision.collider)
        {
            col = null;
            HasCollided = false;
        }
    }
}
