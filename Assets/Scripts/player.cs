using UnityEngine;
using System.Collections;

public class player 
{
    Transform transform;
    string username;
    int direction;

    public player(string username,Transform transform, int direction)
    {
        this.transform = transform;
        this.username = username;
        this.direction = direction;
    }

    public Transform getTransform()
    {
       return transform;
    }

    public float getPositionX()
    {
        return transform.position.x;
    }

    public float getPositionZ()
    {
        return transform.position.z;
    }

    public void setPositionX(float input)
    {
        transform.position = new Vector3 (input,transform.position.y,transform.position.z);
    }

    public void setPositionZ(float input)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, input);
    }

    public void setPosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public string getUsername()
    {
        return username;
    }

    public int getDirection()
    {
        return direction;
    }

    public void setDirection(int input)
    {
        direction = input;
    }
}
