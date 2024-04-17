using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour {

    public GameObject cube;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit = ShootRay();
            if(hit.transform == null)
            {
                Debug.Log("hit is Null");
            }
        }
	}

    public RaycastHit ShootRay()
    {

        Vector3 StartVec = Vector3.forward;
        Vector3 dirVec = Vector3.forward;
        if (Camera.main == null)
        {
            Debug.Log("Camera Main is null");
        }
        if (Camera.main != null)
        {
            //Debug.Log("Camera.main : " + Camera.main.name);
            StartVec = Camera.main.transform.position;
            dirVec = Camera.main.transform.forward;
        }
        //Debug.Log(dirVec);
        Ray ray = new Ray(StartVec, transform.TransformDirection(dirVec));

        //Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(StartVec, dirVec, Color.yellow, Mathf.Infinity);
            Debug.Log(hit.collider.name);
            Debug.Log(hit.point);
            GameObject go = Instantiate(cube, hit.point, Quaternion.identity);
            go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            return hit;
        }

        //if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
        //{
        //    Debug.DrawRay(StartVec, dirVec, Color.yellow, Mathf.Infinity);
        //    return hit;
        //}

        //RaycastHit hit;
        Debug.DrawRay(StartVec, dirVec, Color.red, Mathf.Infinity);

        //if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
        //{
        //    Debug.DrawRay(StartVec, dirVec, Color.yellow, Mathf.Infinity);
        //    return hit;
        //}

        Debug.Log("hit is Null");
        return hit;
    }
}
