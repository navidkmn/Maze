using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.IO;
using LitJson;

public class move : MonoBehaviour {
 
    /*
     * 0 north
     * 1 east
     * 2 south
     * 3 west
     */
  
    static int[,] maze = new int[10, 10] {{0,0,0,0,0,0,1,0,0,0},
                                          {0,1,0,1,1,0,1,0,1,0},
                                          {0,1,0,0,1,0,0,0,1,0},
                                          {0,1,1,0,1,1,1,1,1,1},
                                          {0,1,0,0,1,0,0,0,0,0},
                                          {0,1,0,1,1,0,0,1,0,0},
                                          {0,1,0,0,1,0,0,0,0,0},
                                          {0,1,1,0,1,1,1,1,0,1}, 
                                          {0,1,0,0,1,0,0,1,0,0},
                                          {0,0,0,1,1,0,1,0,0,0},  
    };

    const int row = 10;
    const int colomn = 10;

    const int radius = 0;

    static float xPos = 0;
    static float zPos = 0;

    public static List<int> direction = new List<int>();
    public static List<player> players = new List<player>();
    public static List<Vector2> touchPosition = new List<Vector2>();

    Transform tr;
    GameObject wall;

    Camera cam;
    Rigidbody r;

    static int angelFromNorth = 90;
    static int geometricalDirecion = 1;

    int time =0;

    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    string Host = "192.168.139.63";
    int Port = 8585;
    static string user;

	// Use this for initialization
    void Start()
    {
        user = menuscript.userName;
        
        Application.targetFrameRate = 30;
        
        connectToSever();

        direction.Add(1);
        direction.Add(-1);
        
        r = transform.gameObject.AddComponent<Rigidbody>();
        r.useGravity = false;

        transform.position = new Vector3(xPos,radius,zPos);
        transform.localScale = new Vector3(2,2,2);
        transform.rotation = Quaternion.Euler(0,0,0);

        for (int i = 0; i < row; i++) {
            for (int j = 0; j < colomn; j++)
            {
                int [] coordinate = new int[2];
                coordinate[0] = i;
                coordinate[1] = j;

                if (maze[i, j] == 1)
                {
                    wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.position = new Vector3(i * 10 + 10/2, 2, j * 10 + 10/2);
                    wall.transform.localScale = new Vector3(10, 4, 10);

                }
            }
         }
    }

    public void connectToSever()
    {
        mySocket = new TcpClient(Host, Port);
        theStream = mySocket.GetStream();
        theWriter = new StreamWriter(theStream);
        theReader = new StreamReader(theStream);

        theWriter.Write("{\"name\":\""+user+"\"}");      //my name 
        theWriter.Flush();

        while (!(theStream.DataAvailable))
        {
            System.Threading.Thread.Sleep(100);
        }

        setPlayers();
    }

    public void setPlayers()
    {

        for (int i = 0; i < 5; i++)
        {
            bool a = true;
            GameObject o;
            string name = "";
            int dir = 0;
            float x = 0;
            float z = 0;

            string m = theReader.ReadLine().ToString();

            JsonData jsonvale = JsonMapper.ToObject(m);
            ICollection keys = ((IDictionary)jsonvale).Keys;

            foreach (string key in keys)
            {
                if (key.ToString() == "name")
                    name = jsonvale[key].ToString();

                if (key.ToString() == "x")
                    x = (float)(Convert.ToDouble(jsonvale[key].ToString()));
 
                if (key.ToString() == "z")
                    z = (float)(Convert.ToDouble(jsonvale[key].ToString()));
                
                if (key.ToString() == "direction")
                    dir = (int)Convert.ToInt32(jsonvale[key].ToString());
            }

            if (name != user)
            {
                o = (GameObject)Instantiate(Resources.Load("Human"));
                o.transform.position = new Vector3(x, 0, z);
                
                if(dir==0)
                    o.transform.rotation = Quaternion.Euler(0, -90, 0);
                if (dir == 1)
                    o.transform.rotation = Quaternion.Euler(0, 0, 0);
                if (dir == 2)
                    o.transform.rotation = Quaternion.Euler(0, 90, 0);
                if (dir == 3)
                    o.transform.rotation = Quaternion.Euler(0, 180, 0);

                players.Add(new player(name, o.transform, dir));
             }

            else
            {
                xPos = x;
                zPos = z;
                geometricalDirecion = dir;
                if (dir == 0)
                    transform.rotation = Quaternion.Euler(0, -90, 0);
                if (dir == 1)
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                if (dir == 2)
                    transform.rotation = Quaternion.Euler(0, 90, 0);
                if (dir == 3)
                    transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
     
  }

    public int recommandDir(int angel, string arrow)
    {
        if ((angel == 0 && arrow == "right") || (angel == 180 && arrow == "left") || (angel == 270 && arrow == "down"))
            return 1;
        else
            if ((angel == 90 && arrow == "right") || (angel == 270 && arrow == "left") || (angel == 0 && arrow == "down"))
                return 2;
            else
                if ((angel == 180 && arrow == "right") || (angel == 0 && arrow == "left") || (angel == 90 && arrow == "down"))
                    return 3;

                else
                    return 0;
    }

    bool canGo(int number,int player,int dir)
    {
        int nextX;
        int nextZ;

        if (number == 0)
        {
            if (player == 4)
            {
                if (dir == 0 && xPos > 5)
                {
                    nextX = (int)((xPos - 5.2f) / 10);
                    nextZ = (int)((zPos) / 10);

                    if (maze[nextX, nextZ] == 0)
                    {
                        angelFromNorth = 0;
                        return true;
                    }
                    else
                        return false;
                }

                if (dir == 2 && xPos < row * 10 - (5))
                {
                    nextX = (int)((xPos + (5)) / 10);
                    nextZ = (int)((zPos) / 10);

                    if (maze[nextX, nextZ] == 0)
                    {
                        angelFromNorth = 180;
                        return true;
                    }
                    else
                        return false;
                }

                if (dir == 1 && zPos < (colomn * 10) - (5))
                {
                    nextX = (int)((xPos) / 10);
                    nextZ = (int)((zPos + (5)) / 10);

                    if (maze[nextX, nextZ] == 0)
                    {
                        angelFromNorth = 90;
                        return true;
                    }
                    else
                        return false;
                }

                if (dir == 3 && zPos > 5)
                {
                    nextX = (int)(xPos / 10);
                    nextZ = (int)((zPos - (5.2f)) / 10);

                    if (maze[nextX, nextZ] == 0)
                    {
                        angelFromNorth = 270;
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else // player
            {
                if (dir == 0 && players[player].getPositionX() > 5)
                {
                    nextX = (int)((players[player].getPositionX() - 5.2f) / 10);
                    nextZ = (int)((players[player].getPositionZ()) / 10);

                    if (maze[nextX, nextZ] == 0)
                        return true;
                    else
                        return false;
                }

                if (dir == 2 && players[player].getPositionX() < row * 10 - (5))
                {
                    nextX = (int)((players[player].getPositionX() + (5)) / 10);
                    nextZ = (int)((players[player].getPositionZ()) / 10);

                    if (maze[nextX, nextZ] == 0)
                        return true;
        
                    else
                        return false;
                }

                if (dir == 1 && players[player].getPositionZ() < (colomn * 10) - (5))
                {
                    nextX = (int)((players[player].getPositionX()) / 10);
                    nextZ = (int)((players[player].getPositionZ() + (5)) / 10);

                    if (maze[nextX, nextZ] == 0)
                        return true;
                    else
                        return false;
                }

                if (dir == 3 && players[player].getPositionZ() > 5)
                {
                    nextX = (int)(players[player].getPositionX() / 10);
                    nextZ = (int)((players[player].getPositionZ() - (5.2f)) / 10);

                    if (maze[nextX, nextZ] == 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        else // number==1
        {
            if (dir == 0 && xPos > 10)
            {
                nextX = (int)((xPos - (10)) / 10);
                nextZ = (int)((zPos) / 10);

                if (maze[nextX, nextZ] == 0 && Mathf.Abs(xPos-((nextX+1)*10+10/2))<=0.2f && Mathf.Abs(zPos-((nextZ)*10+10/2))<=0.2f)
                {
                    xPos = (nextX + 1) * 10 + 10 / 2;
                    zPos = (nextZ) * 10 + 10 / 2;
                   angelFromNorth = 0;
                    return true;
                }
                else
                    return false;
            }

            if (dir == 2 && xPos < row * 10 - (10))
            {
                    nextX = (int)((xPos + (10)) / 10);
                    nextZ = (int)((zPos) / 10);
                    
                if (maze[nextX, nextZ] == 0 && Mathf.Abs(xPos-((nextX-1)*10+10/2))<=0.2f && Mathf.Abs(zPos-((nextZ)*10+10/2))<=0.2f)
                {
                    xPos = (nextX - 1) * 10 + 10 / 2;
                    zPos = (nextZ) * 10 + 10 / 2;
                    angelFromNorth = 180;
                    return true;
                }
                else
                    return false;
            }

            if (dir == 1 && zPos < (colomn * 10) - (10))
            {
                nextX = (int)((xPos) / 10);
                nextZ = (int)((zPos + (10)) / 10);

                if (maze[nextX, nextZ] == 0 && Mathf.Abs(xPos-((nextX)*10+10/2))<=0.2f && Mathf.Abs(zPos-((nextZ-1)*10+10/2))<=0.2f)
                {
                    xPos = (nextX) * 10 + 10 / 2;
                    zPos = (nextZ-1) * 10 + 10 / 2;
                    angelFromNorth = 90;
                    return true;
                }
                else
                    return false;
            }

            if (dir == 3 && zPos > 10)
            {
                nextX = (int)(xPos / 10);
                nextZ = (int)((zPos - (10)) / 10);

                if (maze[nextX, nextZ] == 0 && Mathf.Abs(xPos-((nextX)*10+10/2))<=0.2f && Mathf.Abs(zPos-((nextZ+1)*10+10/2))<=0.2f)
                {
                    xPos = (nextX) * 10 + 10 / 2;
                    zPos = (nextZ+1) * 10 + 10 / 2;
                    angelFromNorth = 270;
                    return true;
                }
                else
                    return false;
            }

            else
                return false;
        }
    }

    void moveStraight(int player)
    {
        if (player == 4)
        {
            if (geometricalDirecion == 0)
            {
                xPos -= 0.2f;
                transform.position = new Vector3(xPos, radius, zPos);
            }

            if (geometricalDirecion == 2)
            {
                xPos += 0.2f;
                transform.position = new Vector3(xPos, radius, zPos);
            }

            if (geometricalDirecion == 1)
            {
                zPos += 0.2f;
                transform.position = new Vector3(xPos, radius, zPos);                
            }

            if (geometricalDirecion == 3)
            {
                zPos -= 0.2f;
                transform.position = new Vector3(xPos, radius, zPos);
            }
        }

        else
        {

            if (players[player].getDirection() == 0)
            {
                players[player].setPositionX(players[player].getTransform().position.x - 0.2f);
            }

            if (players[player].getDirection() == 2)
            {
                players[player].setPositionX(players[player].getTransform().position.x + 0.2f);
            }

            if (players[player].getDirection() == 1)
            {
                players[player].setPositionZ(players[player].getTransform().position.z + 0.2f);
            }

            if (players[player].getDirection() == 3)
            {
                players[player].setPositionZ(players[player].getTransform().position.z - 0.2f);
            }

  //          if (players[player].getDirection() == -1)
 //           {
  //              players[player].setPosition();
   //         }
        }
    }

    public void sendToSever()
    {
        Dictionary<string,string> data = new Dictionary<string, string>();

        data["name"] = user;
        data["x"] = xPos.ToString();
        data["z"] = zPos.ToString();
        data["direction"] = geometricalDirecion.ToString();

        JsonData jd = JsonMapper.ToJson(data);

        theWriter.Write(jd);  
        theWriter.Flush();
     }

     public void recieveFromServer()
     {
         if (theStream.DataAvailable)
         {
             string m = theReader.ReadLine().ToString();
             print(m);
             JsonData jsonvale = JsonMapper.ToObject(m);
          
             ICollection keys = ((IDictionary)jsonvale).Keys;
              int who=0;

                foreach ( string key in keys )
              {
                    if(key.ToString()=="name"){
                        for (int j = 0; j < players.Count; j++)
                            if (players[j].getUsername() == jsonvale[key].ToString())
                                who = j;
                    }
                    if(key.ToString()=="x"){
                        players[who].setPositionX((float)Convert.ToDouble(jsonvale[key].ToString()));
                    }
                     if(key.ToString()=="z"){
                         players[who].setPositionZ((float)Convert.ToDouble(jsonvale[key].ToString()));
                    }
                     if(key.ToString()=="direction"){
                         int dir = (int)Convert.ToDouble(jsonvale[key].ToString());
                         players[who].setDirection(dir);
                         if (dir == 0)
                             players[who].getTransform().rotation = Quaternion.Euler(0,-90,0);
                         if (dir == 1)
                             players[who].getTransform().rotation = Quaternion.Euler(0, 0, 0);
                         if (dir == 2)
                             players[who].getTransform().rotation = Quaternion.Euler(0, 90, 0);
                         if (dir == 3)
                             players[who].getTransform().rotation = Quaternion.Euler(0, 180, 0);


                     }
        //      }
          }     
     }
}
              
     // Update is called once per frame
     void FixedUpdate()
     {
            

        time++;

        recieveFromServer();

         foreach (Touch touch in Input.touches)
         {
             if (touch.phase == TouchPhase.Began)
                 touchPosition.Add(touch.position);

             if (touch.phase == TouchPhase.Moved)
                 touchPosition.Add(touch.position);

             if (touch.phase == TouchPhase.Ended)
             {
                 if (touchPosition[0].x < touchPosition[touchPosition.Count - 1].x && Mathf.Abs(touchPosition[0].x - touchPosition[touchPosition.Count - 1].x) > Mathf.Abs(touchPosition[0].y - touchPosition[touchPosition.Count - 1].y))
                 {
                     time=0;         
                     direction.RemoveAt(1);
                     direction.Add(recommandDir(angelFromNorth, "right"));
                 }
                 if (touchPosition[0].x > touchPosition[touchPosition.Count - 1].x && Mathf.Abs(touchPosition[0].x - touchPosition[touchPosition.Count - 1].x) > Mathf.Abs(touchPosition[0].y - touchPosition[touchPosition.Count - 1].y))
                 {
                     time=0;
                     direction.RemoveAt(1);
                     direction.Add(recommandDir(angelFromNorth, "left"));
                 }
                 if (touchPosition[0].y > touchPosition[touchPosition.Count - 1].y && Mathf.Abs(touchPosition[0].x - touchPosition[touchPosition.Count - 1].x) < Mathf.Abs(touchPosition[0].y - touchPosition[touchPosition.Count - 1].y))
                {
                     time=0;
                     direction.RemoveAt(1);
                     direction.Add(recommandDir(angelFromNorth, "down")); 
                }
             }
         }


    /*    for (int i = 0; i < players.Count; i++)
        {
            if (canGo(0, i, players[i].getDirection()))
                moveStraight(i);
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            time=0;
            direction.RemoveAt(1);
            direction.Add(recommandDir(angelFromNorth, "right"));
        }
        
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            time =0;
            direction.RemoveAt(1);
            direction.Add(recommandDir(angelFromNorth, "left"));
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            time = 0;
            direction.RemoveAt(1);
            direction.Add(recommandDir(angelFromNorth, "down"));
        }
        */

        if (canGo(1,4, direction[direction.Count - 1]))
        {
            if (time < 100)
            {
                geometricalDirecion = direction[direction.Count - 1];
                sendToSever();
                moveStraight(4);
                transform.rotation = Quaternion.Euler(0, angelFromNorth - 90, 0);
                direction.RemoveAt(0);
                direction.Add(-1);
                time = 0;
            }
            else
            {
                direction.RemoveAt(1);
                direction.Add(-1);
                geometricalDirecion = direction[direction.Count - 2];
                moveStraight(4);    
            }
        }

        else
            if (canGo(0,4, direction[direction.Count - 2]))
            {
                geometricalDirecion = direction[direction.Count - 2];
                moveStraight(4);
            }
         //   else
         //   {
           //     geometricalDirecion = -1;
           //     transform.position = new Vector3(xPos, radius, zPos);

                //sendToSever();
         //   }

   }
}
