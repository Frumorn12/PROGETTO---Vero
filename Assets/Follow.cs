using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target; // Il transform del personaggio che la telecamera deve seguire
    public Vector3 offset; // L'offset dalla posizione del personaggio

    void Update()
    {
        if (target != null)
        {
            // Imposta la posizione della telecamera sulla posizione del personaggio con l'offset
            transform.position = target.position + offset;

            // Fai sì che la telecamera guardi sempre verso il personaggio
            transform.LookAt(target);
        }
    }
}
