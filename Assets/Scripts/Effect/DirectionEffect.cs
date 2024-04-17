using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionEffect : MonoBehaviour {

    private float m_LifeTime = 5.0f;
    private float m_ElapsedTime = 0.0f;

    private SpriteRenderer icon;
    private SpriteRenderer BG;

    // Use this for initialization
    void Start () {
        icon = transform.Find("Icon").GetComponent<SpriteRenderer>();
        BG = transform.Find("BG").GetComponent<SpriteRenderer>();

        //if(FindObjectOfType<AsyncClient>() != null)
        //{
        //    var client = FindObjectOfType<AsyncClient>();
        //    if(client.isServer)
        //    {
        //        icon.sortingOrder = 12;
        //        BG.sortingOrder = 12;
        //    }            
        //}
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameObject.activeInHierarchy) return;

        if (m_ElapsedTime > m_LifeTime)
        {
            Destroy(gameObject);
        }
        else
            m_ElapsedTime += Time.deltaTime;

    }
}
