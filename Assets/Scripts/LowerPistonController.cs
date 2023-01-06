using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerPistonController : MonoBehaviour
{
    [SerializeField] ObjectPushPiston objectPush;
    [SerializeField] float DestroyTimeDelay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !objectPush.IsPistonActive)
        {
            objectPush.InitialisePlatformRaise();
            print(other + " enetered piston trigger");
            StartCoroutine(DestroyObjDelay(other.transform.root.gameObject));
        }
    }

    IEnumerator DestroyObjDelay(GameObject DestroyTarget)
    {
        yield return new WaitForSeconds(DestroyTimeDelay);
        print("destroyed " + DestroyTarget);
        Destroy(DestroyTarget);
    }
}
