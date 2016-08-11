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
                                          {0,1,0,0,0,0,0,1,0,0},
                                          {0,0,0,1,1,0,0,0,0,0},  
    };

    const int row = 10;
    const int colomn = 10;

    public static List<int> direction = new List<int>();
    public static List<player> players = new List<player>();
    public static List<Vector2> touchPosition = new List<Vector2>();

    Transform tr;
    GameObject wall;
    Rigidbody r;

    static int angelFromNorth;
    static int geometricalDirecion;

    int time = 0;
    int tryToConnect = 0;

    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;

    string host;
    int playersNo;
    int Port = 8585;
    static string user;

	// Use this for initialization
    void Start()
    {
        user = menuscript.userName;
        host = menuscript.host;
        playersNo = (int)Convert.ToInt32(menuscript.player);

        Application.targetFrameRate = 30;
        
        connectToSever();
        //transform.position = new Vector3(5,0,5);
       // transform.rotation = Quaternion.Euler(0, 0, 0);
        direction.Add(1);
        direction.Add(-1);
        
        r = transform.gameObject.AddComponent<Rigidbody>();
        r.useGravity = false;

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
                    wall.GetComponent<Renderer>().material.color = Color.red;
                }
            }
         }
    }

    public void connectToSever()
    {
        mySocket = new TcpClient(host, Port);
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

        for (int i = 0; i < playersNo; i++)
        {
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
                print(dir + "   doshman");
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
                print(dir + "   me");
                transform.position = new Vector3(x,0,z);
                geometricalDirecion = dir;
                if (dir == 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    angelFromNorth = 0;
                }
                if (dir == 1)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    angelFromNorth = 90;
                }
                if (dir == 2)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    angelFromNorth = 180;
                }
                if (dir == 3)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    angelFromNorth = 270;
                }
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
                if (dir == 0 && transform.position.x > 5)
                {
                    nextX = (int)((transform.position.x - 5.2f) / 10);
                    nextZ = (int)((transform.position.z) / 10);

                    if (maze[nextX, nextZ] == 0)
                    {
                        angelFromNorth = 0;
                        return true;
                    }
                    else
                        return false;
                }

                if (dir == 2 && transform.position.x < row * 10 - (5))
                {
                    nextX = (int)((transform.position.x + (5)) / 10);
                    nextZ = (int)((transform.position.z) / 10);

                    if (maze[nextX, nextZ] == 0)
                    {
                        angelFromNorth = 180;
                        return true;
                    }
                    else
                        return false;
                }

                if (dir == 1 && transform.position.z < (colomn * 10) - (5))
                {
                    nextX = (int)((transform.position.x) / 10);
                    nextZ = (int)((transform.position.z + (5)) / 10);

                    if (maze[nextX, nextZ] == 0)
                    {
                        angelFromNorth = 90;
                        return true;
                    }
                    else
                        return false;
                }

                if (dir == 3 && transform.position.z > 5)
                {
                    nextX = (int)(transform.position.x / 10);
                    nextZ = (int)((transform.position.z - (5.2f)) / 10);

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
            if (dir == 0 && transform.position.x > 10)
            {
                nextX = (int)((transform.position.x - (10)) / 10);
                nextZ = (int)((transform.position.z) / 10);

                if (maze[nextX, nextZ] == 0 && Mathf.Abs(transform.position.x - ((nextX + 1) * 10 + 10 / 2)) <= 0.2f && Mathf.Abs(transform.position.z - ((nextZ) * 10 + 10 / 2)) <= 0.2f)
                {
                    transform.position = new Vector3((nextX + 1) * 10 + 10 / 2, 0, (nextZ) * 10 + 10 / 2);
                   angelFromNorth = 0;
                    return true;
                }
                else
                    return false;
            }

            if (dir == 2 && transform.position.x < row * 10 - (10))
            {
                nextX = (int)((transform.position.x + (10)) / 10);
                nextZ = (int)((transform.position.z) / 10);

                if (maze[nextX, nextZ] == 0 && Mathf.Abs(transform.position.x - ((nextX - 1) * 10 + 10 / 2)) <= 0.2f && Mathf.Abs(transform.position.z - ((nextZ) * 10 + 10 / 2)) <= 0.2f)
                {
                    transform.position = new Vector3((nextX - 1) * 10 + 10 / 2, 0, (nextZ) * 10 + 10 / 2);
                    angelFromNorth = 180;
                    return true;
                }
                else
                    return false;
            }

            if (dir == 1 && transform.position.z < (colomn * 10) - (10))
            {
                nextX = (int)((transform.position.x) / 10);
                nextZ = (int)((transform.position.z + (10)) / 10);

                if (maze[nextX, nextZ] == 0 && Mathf.Abs(transform.position.x - ((nextX) * 10 + 10 / 2)) <= 0.2f && Mathf.Abs(transform.position.z - ((nextZ - 1) * 10 + 10 / 2)) <= 0.2f)
                {
                    transform.position = new Vector3((nextX) * 10 + 10 / 2, 0, (nextZ - 1) * 10 + 10 / 2);
                    angelFromNorth = 90;
                    return true;
                }
                else
                    return false;
            }

            if (dir == 3 && transform.position.z > 10)
            {
                nextX = (int)(transform.position.x / 10);
                nextZ = (int)((transform.position.z - (10)) / 10);

                if (maze[nextX, nextZ] == 0 && Mathf.Abs(transform.position.x - ((nextX) * 10 + 10 / 2)) <= 0.2f && Mathf.Abs(transform.position.z - ((nextZ + 1) * 10 + 10 / 2)) <= 0.2f)
                {
                    transform.position = new Vector3((nextX) * 10 + 10 / 2, 0, (nextZ + 1) * 10 + 10 / 2);
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
              transform.position = new Vector3(transform.position.x - (0.2f),0,transform.position.z);

            if (geometricalDirecion == 2)    
                transform.position = new Vector3(transform.position.x + (0.2f), 0, transform.position.z);
            
            if (geometricalDirecion == 1)
                transform.position = new Vector3(transform.position.x,0,transform.position.z + (0.2f));                

            if (geometricalDirecion == 3)
                transform.position = new Vector3(transform.position.x,0,transform.position.z - (0.2f));                
            }
        
        else
        {
            if (players[player].getDirection() == 0)
                players[player].setPositionX(players[player].getTransform().position.x - 0.2f);
            
            if (players[player].getDirection() == 2)
                players[player].setPositionX(players[player].getTransform().position.x + 0.2f);
            
            if (players[player].getDirection() == 1)
                players[player].setPositionZ(players[player].getTransform().position.z + 0.2f);
        
            if (players[player].getDirection() == 3)
                players[player].setPositionZ(players[player].getTransform().position.z - 0.2f);
        }
    }

    public void sendToSever()
    {
            Dictionary<string,string> data = new Dictionary<string, string>();

            data["name"] = user;
            data["x"] = transform.position.x.ToString();
            data["z"] = transform.position.z.ToString();
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
          }     
     }
}

     void checkOtherConncting()
     {
         if (theStream.DataAvailable)
         {
             string m = theReader.ReadLine();
             print(m);
             JsonData jsonvale = JsonMapper.ToObject(m);

             ICollection keys = ((IDictionary)jsonvale).Keys;

             foreach (string key in keys)
                 if (key.ToString().Equals("disconnect"))
                     print(jsonvale[key].ToString() + " disconnected");
         }
     }

     void checkMyConnecting()
     {
             if (Time.time % 1 == 0 && tryToConnect<400)
             {
                 print("back");
                 mySocket = new TcpClient(host, Port);
                 theStream = mySocket.GetStream();
                 theWriter = new StreamWriter(theStream);
                 theReader = new StreamReader(theStream);

                 theWriter.Write("{\"name\":\"" + user + "\"}");      //my name 
                 theWriter.Flush();
             }
     }
              
     // Update is called once per frame
     void FixedUpdate()
     {

        time++;
        tryToConnect++;

        if (!mySocket.Connected)
            checkMyConnecting();

        if (mySocket.Connected)
        {
            recieveFromServer();
            tryToConnect = 0;
        }

         if (mySocket.Connected && Time.time % 0.5 == 0)
             sendToSever();

         for (int i = 0; i < players.Count; i++)
        {
            if (canGo(0, i, players[i].getDirection()))
                moveStraight(i);
        }

         /*
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
                   touchPosition.Clear();
             }
         }*/
               
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
         
         
        if (canGo(1,4, direction[direction.Count - 1]))
        {
            if (time < 100)
            {
                geometricalDirecion = direction[direction.Count - 1];
                moveStraight(4);
                transform.rotation = Quaternion.Euler(0, angelFromNorth - 90, 0);
                
                if(mySocket.Connected)
                sendToSever();
                
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

         if(mySocket.Connected)
              checkOtherConncting();
   }
}
