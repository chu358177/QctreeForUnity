using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour {
    public GameObject go;
    public   int GoNum = 25;
    [Tooltip("显示八叉树分割空间")]
    public bool isShowQctreeBox;
    List<GameObject> listGo = new List<GameObject>();

    QctreeNode root;

    Bounds queryBound;
    Vector3 queryCenter;
    List<GameObject> listOrder;
    // Use this for initialization
    void Start () {
        
        Spawns();

        root = new QctreeNode(new Bounds(Vector3.zero, new Vector3(100, 100, 100)));

        foreach (GameObject g in listGo)
        {
            root.Insert(g);
        }

        //queryCenter = new Vector3(100, 100, 100);
        queryBound = new Bounds(queryCenter, new Vector3(30, 30, 30));


        int nnn = root.QueryInBounds(queryBound).Count;
        Debug.Log("Count: " + nnn);

        listOrder = root.preOrder(root);
        Debug.Log(listOrder.Count);
    }

    void Spawns()
    {
        for (int i = 0; i < GoNum; i++)
        {
            Vector3 p = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));
            GameObject tmpGo = Instantiate(go, p, Quaternion.identity);
            tmpGo.name += i.ToString();
            listGo.Add(tmpGo);        
        }

    }

    List<GameObject> goList;
    void unLoadNotInside(Bounds bds)
    {
        if (goList == null) return;
        foreach(GameObject g in goList)
        {
            if(!bds.Contains(g.transform.position))
            {
                g.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }
  void loadGo()
    {
        foreach (GameObject g in listGo)
        {

        }
    }

    // Update is called once per frame
    void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if(h!=0 || v !=0)
        {
            queryCenter += new Vector3( h*2, queryCenter.y, v * 2);

         
        }

        //if(Input.GetKeyDown(KeyCode.Q))
        {
            queryBound = new Bounds(queryCenter, new Vector3(30, 30, 50));

            unLoadNotInside(queryBound);

           
           goList = root.QueryInBounds(queryBound);
            
            foreach(GameObject g in goList)
            {
                g.GetComponent<MeshRenderer>().material.color = Color.red;
            }

            
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (GameObject g in root.gameObjectsList)
            {
                Debug.Log(g.name);
            }

        }


    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawWireCube(Vector3.zero, new Vector3(100, 100, 100));
        if (root != null)
        {
            Gizmos.DrawWireCube(root.bounds.center, root.bounds.size);

            //DrawDebug();
            //Gizmos.DrawWireCube(root.childNodes[0].bounds.center, root.childNodes[0].bounds.size);
            // Gizmos.DrawWireCube(root.childNodes[1].bounds.center, root.childNodes[1].bounds.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(queryCenter, queryBound.size);
        }
        if(listOrder != null)
        {
            Gizmos.color = Color.blue;
            foreach (GameObject g in listOrder)
            {
                Gizmos.DrawWireCube(g.transform.position, g.GetComponent<Renderer>().bounds.size);
            }
        }

        if(isShowQctreeBox) DrawDebug(root);
    }

    void DrawDebug(QctreeNode nodeTree)
    {
        if (nodeTree == null) return;
        Gizmos.color = Color.cyan;
        if (nodeTree.childNodes == null) return;
        foreach (QctreeNode node in nodeTree.childNodes)
        {
            //Debug.Log(node.bounds.center);
            Gizmos.DrawWireCube(node.bounds.center, node.bounds.size);

            if (node.childNodes != null)
            {
                foreach (QctreeNode chileNode in node.childNodes)
                {
                    DrawDebug(chileNode);
                }
            }
        }
    }

   
}
