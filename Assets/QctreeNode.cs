using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QctreeNode
{
    const int QT_NODE_CAPACITY = 4;
    const int maxLevel = 8;
    int level;
    
    public Bounds bounds;
    public List<GameObject> gameObjectsList;
    public QctreeNode[] childNodes;

    public QctreeNode(Bounds b)
    {
        bounds = b;
        gameObjectsList = new List<GameObject>();
        // childNodes = new List<QctreeNode>();
        level = 0;
    }
    public QctreeNode(Bounds b, int lev)
    {
        bounds = b;
        gameObjectsList = new List<GameObject>();
        //childNodes = null;
        level = lev;
    }
    public bool Insert(GameObject go)
    {
        if (!bounds.Contains(go.transform.position))
        {
            return false;
        }

        if (gameObjectsList.Count < QT_NODE_CAPACITY)
        {
            gameObjectsList.Add(go);
            return true;
        }

        if (childNodes == null)
            SubDivide();

        //foreach(QctreeNode node in childNodes)
        //{
        //    if (node.Insert(go)) return true;
        //}

        if (childNodes == null) { Debug.Log("Out of Capacity"); return false; }
        for (int i = 0; i < 8; i++)
        {
            if (childNodes[i].Insert(go)) return true;
        }

        return false;
    }

    void SubDivide()
    {
       
        if (childNodes == null && level < maxLevel)
        //if (childNodes == null)
        {
            Debug.Log("SubDivide");

            childNodes = new QctreeNode[8];
            float halfSizeX = bounds.size.x / 4;
            float halfSizeY = bounds.size.y / 4;
            float halfSizeZ = bounds.size.z / 4;
            Vector3 centerNew = new Vector3(bounds.center.x - halfSizeX, bounds.center.y + halfSizeY, bounds.center.z + halfSizeZ);
            Bounds boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[0] = new QctreeNode(boundsNew,level +1);//up left back 

            centerNew = new Vector3(bounds.center.x - halfSizeX, bounds.center.y + halfSizeY, bounds.center.z - halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[1] = new QctreeNode(boundsNew, level + 1);//up left front 

            centerNew = new Vector3(bounds.center.x + halfSizeX, bounds.center.y + halfSizeY, bounds.center.z - halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[2] = new QctreeNode(boundsNew, level + 1);//up right front

            centerNew = new Vector3(bounds.center.x + halfSizeX, bounds.center.y + halfSizeY, bounds.center.z + halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[3] = new QctreeNode(boundsNew, level + 1);//up right back

            centerNew = new Vector3(bounds.center.x - halfSizeX, bounds.center.y - halfSizeY, bounds.center.z + halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[4] = new QctreeNode(boundsNew, level + 1);//bottom left back 

            centerNew = new Vector3(bounds.center.x - halfSizeX, bounds.center.y - halfSizeY, bounds.center.z - halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[5] = new QctreeNode(boundsNew, level + 1);//bottom left front 

            centerNew = new Vector3(bounds.center.x + halfSizeX, bounds.center.y - halfSizeY, bounds.center.z - halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[6] = new QctreeNode(boundsNew, level + 1);//bottom right front

            centerNew = new Vector3(bounds.center.x + halfSizeX, bounds.center.y - halfSizeY, bounds.center.z + halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[7] = new QctreeNode(boundsNew, level + 1);//bottom right back
        }
    }

    public List<GameObject> preOrder(QctreeNode node)
    {
        if (node == null) return null;
        List<GameObject> orderGoList = new List<GameObject>();

        for (int g = 0; g < node.gameObjectsList.Count; g++)
        {
                orderGoList.Add(node.gameObjectsList[g]);
        }

        if (childNodes == null) return orderGoList;

        for (int i = 0; i < 8; i++)
        {
            orderGoList.AddRange(childNodes[i].preOrder(childNodes[i]));
            //Debug.Log("for queryGoList: " + queryGoList.Count);
        }

        return orderGoList;
    }

    public List<GameObject> QueryInBounds(Bounds bds)
    {
        List<GameObject> queryGoList = new List<GameObject>();
        if (!bounds.Intersects(bds)) return queryGoList;// empty list

        for (int g = 0; g < gameObjectsList.Count; g++)
        {
            //if(bds.Intersects(gameObjectsList[g].GetComponent<Renderer>().bounds))
            if (bds.Contains(gameObjectsList[g].transform.position))          
            {
                
                queryGoList.Add(gameObjectsList[g]);
            }
        }

        if (childNodes == null) return queryGoList;
        //Debug.Log("queryGoList: " + queryGoList.Count);
        for (int i = 0; i < 8; i++)
        {
            queryGoList.AddRange(childNodes[i].QueryInBounds(bds));
            //Debug.Log("for queryGoList: " + queryGoList.Count);
        }

        return queryGoList;
    }

}
