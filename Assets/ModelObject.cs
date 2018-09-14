using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelObject : ISceneObject
{
    public GameObject m_gameObject;

    private Bounds m_Bounds;

    public ModelObject( GameObject go)
    {
        m_gameObject = go;
        
    }

    public ModelObject(Bounds bound)
    {
        m_Bounds = bound;
        m_gameObject = null;
    }
    public Bounds bounds
    {
        get
        {
            return m_gameObject != null ?  m_gameObject.GetComponent<Renderer>().bounds : m_Bounds;
        }
    }

    public void OnHide()
    {
        throw new System.NotImplementedException();
    }

    public bool OnShow(Transform parent)
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
