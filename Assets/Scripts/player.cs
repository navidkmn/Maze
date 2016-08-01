using UnityEngine;
using System.Collections;

public class player : MonoBehaviour
{

    Transform transform;
    string username;
    int direction;

    public player(Transform transform, string username, int direction)
    {
        this.transform = transform;
        this.username = username;
        this.direction = direction;
    }

}
