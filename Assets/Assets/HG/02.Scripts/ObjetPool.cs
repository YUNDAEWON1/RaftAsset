using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetPool : MonoBehaviour
{
    public static ObjetPool instance;

    //public GameObject plank = null;
    //public GameObject leaf = null;
    //public GameObject plastic = null;

    public GameObject[] objects;

    public GameObject spawns;
    public Queue<GameObject> m_queue = new Queue<GameObject>();

    private void Awake()
    {
        instance = this;
        spawns = GameObject.Find("Spawn_Object");
    }

    public void  CreateQueue()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject plank = PhotonNetwork.InstantiateSceneObject("Plank", spawns.transform.position, spawns.transform.rotation, 0, null);
                plank.SetActive(false);
                m_queue.Enqueue(plank);

                GameObject leaf = PhotonNetwork.InstantiateSceneObject("Leaf", spawns.transform.position, spawns.transform.rotation, 0, null);
                leaf.SetActive(false);
                m_queue.Enqueue(leaf);

                GameObject plastic = PhotonNetwork.InstantiateSceneObject("Plastic", spawns.transform.position, spawns.transform.rotation, 0, null);
                plastic.SetActive(false);
                m_queue.Enqueue(plastic);
            }
        }
    }

    public void InsertQueue(GameObject p_object)
    {
        m_queue.Enqueue(p_object);
        p_object.SetActive(false);
    }

    public GameObject GetQueue()
    {
        GameObject t_object = m_queue.Dequeue();
        t_object.SetActive(true);
        return t_object;
    }
}
