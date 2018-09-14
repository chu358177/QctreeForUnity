using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ISceneObject
{
    /// <summary>
    /// 该物体的包围盒
    /// </summary>
    Bounds bounds { get; }

    /// <summary>
    /// 该物体进入显示区域时调用（在这里处理物体的加载或显示）
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    bool OnShow(Transform parent);

    /// <summary>
    /// 该物体离开显示区域时调用（在这里处理物体的卸载或隐藏）
    /// </summary>
    void OnHide();


}
public class QctreeNode<T>  where T : ISceneObject
{
    const int QT_NODE_CAPACITY = 4;
    const int maxLevel = 8;
    int level;
    
    public Bounds bounds;
    public List<T> gameObjectsList;
    public QctreeNode<T>[] childNodes;

    public QctreeNode(Bounds b)
    {
        bounds = b;
        gameObjectsList = new List<T>();
        // childNodes = new List<QctreeNode>();
        level = 0;
    }
    public QctreeNode(Bounds b, int lev)
    {
        bounds = b;
        gameObjectsList = new List<T>();
        //childNodes = null;
        level = lev;
    }
    public bool Insert(T go)
    {
        if (!bounds.Contains(go.bounds.center))
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

            childNodes = new QctreeNode<T>[8];
            float halfSizeX = bounds.size.x / 4;
            float halfSizeY = bounds.size.y / 4;
            float halfSizeZ = bounds.size.z / 4;
            Vector3 centerNew = new Vector3(bounds.center.x - halfSizeX, bounds.center.y + halfSizeY, bounds.center.z + halfSizeZ);
            Bounds boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[0] = new QctreeNode<T>(boundsNew,level +1);//up left back 

            centerNew = new Vector3(bounds.center.x - halfSizeX, bounds.center.y + halfSizeY, bounds.center.z - halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[1] = new QctreeNode<T>(boundsNew, level + 1);//up left front 

            centerNew = new Vector3(bounds.center.x + halfSizeX, bounds.center.y + halfSizeY, bounds.center.z - halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[2] = new QctreeNode<T>(boundsNew, level + 1);//up right front

            centerNew = new Vector3(bounds.center.x + halfSizeX, bounds.center.y + halfSizeY, bounds.center.z + halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[3] = new QctreeNode<T>(boundsNew, level + 1);//up right back

            centerNew = new Vector3(bounds.center.x - halfSizeX, bounds.center.y - halfSizeY, bounds.center.z + halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[4] = new QctreeNode<T>(boundsNew, level + 1);//bottom left back 

            centerNew = new Vector3(bounds.center.x - halfSizeX, bounds.center.y - halfSizeY, bounds.center.z - halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[5] = new QctreeNode<T>(boundsNew, level + 1);//bottom left front 

            centerNew = new Vector3(bounds.center.x + halfSizeX, bounds.center.y - halfSizeY, bounds.center.z - halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[6] = new QctreeNode<T>(boundsNew, level + 1);//bottom right front

            centerNew = new Vector3(bounds.center.x + halfSizeX, bounds.center.y - halfSizeY, bounds.center.z + halfSizeZ);
            boundsNew = new Bounds(centerNew, bounds.size / 2);
            childNodes[7] = new QctreeNode<T>(boundsNew, level + 1);//bottom right back
        }
    }

    public List<T> preOrder(QctreeNode<T> node)
    {
        if (node == null) return null;
        List<T> orderGoList = new List<T>();

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

    public List<T> QueryInBounds(Bounds bds)
    {
        List<T> queryGoList = new List<T>();
        if (!bounds.Intersects(bds)) return queryGoList;// empty list

        for (int g = 0; g < gameObjectsList.Count; g++)
        {
            //if(bds.Intersects(gameObjectsList[g].GetComponent<Renderer>().bounds))
            if (bds.Contains(gameObjectsList[g].bounds.center))          
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
