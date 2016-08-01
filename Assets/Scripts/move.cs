using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class move : MonoBehaviour {

    static int[,] maze = new int[10, 10] {{0,0,0,0,0,0,0,0,0,0},
                                          {0,0,0,0,0,0,0,0,0,0},
                                          {0,0,0,0,0,0,0,0,0,0},
                                          {1,1,1,0,0,1,1,0,0,1},
                                          {0,0,0,0,0,0,0,0,0,0},
                                          {1,1,0,1,1,0,0,1,0,0},
                                          {0,0,0,0,0,0,0,0,0,0},
                                          {1,1,1,0,1,1,1,1,0,1}, 
                                          {0,0,0,0,0,0,0,1,0,0},
                                          {0,0,0,0,1,0,1,0,0,0},  
    };
    const int row = 10;
    const int colomn = 10;

    const int radius = 2;

    static int xMaze = 0;
    static int zMaze = 0;

    static float xPos = 5;
    static float zPos = 5;

    static int endX;
    static int endZ;

    static int distanceFromCenter = 10;

    static bool changedir = false;

    public static List<string> dir = new List<string>();
    public static List<int[]> walls = new List<int[]>();
    public static List<int[]> paths = new List<int[]>();

    Vector3 velocity = new Vector3();

    Transform tr;
    GameObject wall;

    Rigidbody r;

    static int angelFromNorth =0;
    static string GeometricalDirecion ="east";  

	// Use this for initialization
    void Start()
    {
        dir.Add("east");
        dir.Add("");

        r = GameObject.Find("Sphere").AddComponent<Rigidbody>();

        Application.targetFrameRate = 30;
        transform.position = new Vector3(xPos,radius,zPos);

        for (int i = 0; i < row; i++) {
            for (int j = 0; j < colomn; j++)
            {
                int [] coordinate = new int[2];
                coordinate[0] = i;
                coordinate[1] = j;

                if (maze[i, j] == 1)
                {
                    walls.Add(coordinate);
                    wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.position = new Vector3(i * 10 + 10/2, 2, j * 10 + 10/2);
                    wall.transform.localScale = new Vector3(10, 4, 10);

                }
                else
                    paths.Add(coordinate);
            }
         }
    }

    string recommandDir(int angel, string arrow)
    {
        if ((angel == 0 && arrow == "right") || (angel == 180 && arrow == "left"))
            return "east";
        else
            if ((angel == 90 && arrow == "right") || (angel == 270 && arrow == "left"))
                return "south";
            else
                if ((angel == 180 && arrow == "right") || (angel == 0 && arrow == "left"))
                    return "west";

                else
                    return "north";
    }

    bool canGo(int number,string dir)
    {
        int nextX;
        int nextZ;

        if (number == 0)
        {
            if (dir == "north" && xPos > radius)
            {
                nextX = (int)((xPos - (radius + 1)) / 10);
                nextZ = (int)((zPos) / 10);

                if (maze[nextX, nextZ] == 0)
                {
                    angelFromNorth = 0;
                    return true;
                }
                else
                    return false;
            }

            if (dir == "south" && xPos < row * 10 - (radius + 1))
            {
                nextX = (int)((xPos + (radius + 1)) / 10);
                nextZ = (int)((zPos) / 10);

                if (maze[nextX, nextZ] == 0)
                {
                    angelFromNorth = 180;
                    return true;
                }
                else
                    return false;
            }

            if (dir == "east" && zPos < (colomn * 10) - (radius + 1))
            {
                nextX = (int)((xPos) / 10);
                nextZ = (int)((zPos + (radius + 1)) / 10);

                if (maze[nextX, nextZ] == 0)
                {
                    angelFromNorth = 90;
                    return true;
                }
                else
                    return false;
            }

            if (dir == "west" && zPos > radius)
            {
                nextX = (int)(xPos / 10);
                nextZ = (int)((zPos - (radius + 1)) / 10);

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

        else // number==1
        {
            if (dir == "north" && xPos > 10)
            {
                nextX = (int)((xPos - (distanceFromCenter)) / 10);
                nextZ = (int)((zPos) / 10);
             //   print(nextX + "     " + nextZ);
                if (maze[nextX, nextZ] == 0)
                {
        //            if (!changedir)
       //             {
                        endX = nextX * 10 + 10 / 2;
                        endZ = nextZ * 10 + 10 / 2;
      //              }
//                    endX = nextX * 10 + 10 / 2;
 //                   endZ = nextZ * 10 + 10 / 2;

                   angelFromNorth = 0;
                   changedir = true;
                    return true;
                }
                else
                    return false;
            }

            if (dir == "south" && xPos < row * 10 - (10))
            {
           //     if (!changedir)
           //     {
                    nextX = (int)((xPos + (10)) / 10);
                    nextZ = (int)((zPos) / 10);
           //     }

                if (maze[nextX, nextZ] == 0)
                {

           //         if (!changedir)
           //         {
                        endX = nextX * 10 + 10 / 2;
                        endZ = nextZ * 10 + 10 / 2;
          //          }
                    angelFromNorth = 180;
                    changedir = true;
                    return true;
                }
                else
                    return false;
            }

            if (dir == "east" && zPos < (colomn * 10) - (10))
            {
                nextX = (int)((xPos) / 10);
                nextZ = (int)((zPos + (10)) / 10);

                if (maze[nextX, nextZ] == 0)
                {

                 //   endX = nextX * 10 + 10 / 2;
                  // endZ = nextZ * 10 + 10 / 2;

            //        if (!changedir)
            //        {
                        endX = nextX * 10 + 10 / 2;
                        endZ = nextZ * 10 + 10 / 2;
            //        }
                    angelFromNorth = 90;
                    changedir = true;
                    return true;
                }
                else
                    return false;
            }

            if (dir == "west" && zPos > 10)
            {
                nextX = (int)(xPos / 10);
                nextZ = (int)((zPos - (10)) / 10);

                if (maze[nextX, nextZ] == 0)
                {
      //              if (!changedir)
     //               {
                                          endX = nextX * 10 + 10 / 2;
                                        endZ = nextZ * 10 + 10 / 2;
   //                 }
                    angelFromNorth = 270;
                    changedir = true;
                    return true;
                }
                else
                    return false;
            }

            else
                return false;
        }
    }

    void moveStraight()
    {

        if (GeometricalDirecion == "north")
        {
            xPos -= 0.5f;
            transform.position = new Vector3(xPos, radius, zPos);
        }

        if (GeometricalDirecion == "south")
        {
            xPos += 0.5f;
            transform.position = new Vector3(xPos, radius, zPos);
        }

        if (GeometricalDirecion == "east")
        {
            zPos += 0.5f;
            transform.position = new Vector3(xPos, 2, zPos);
        }

        if (GeometricalDirecion == "west")
        {
            zPos -= 0.5f;
            transform.position = new Vector3(xPos, radius, zPos);
        }
    }
    
    Vector3 calcRejectVector(Vector3 me , Vector3 goal)
    {
        Vector3 v = me - goal;
        float f = Mathf.Max(v.magnitude* 0.005f ,0.5f);
        v = v.normalized * f;
        return v;
    }

    Vector3 calcAttractiveVector(Vector3 me , Vector3 goal)
    {
        Vector3 v = goal - me;
        float f = Mathf.Max(v.magnitude*0.5f,20);
        v = v.normalized * f;
        return v; 
    }

    void changeDirection()
    {
      //  int count = 0;
        velocity = calcAttractiveVector(new Vector3(xPos,radius,zPos),new Vector3(endX,radius,endZ));

        foreach (int[] wall in walls)
          velocity += calcRejectVector(new Vector3(xPos,radius,zPos),new Vector3(wall[0]*10+10/2,radius,wall[1]*10+10/2));

        r.velocity = velocity;
  //      distanceFromCenter--;
 //       print(distanceFromCenter);
    }
        
	// Update is called once per frame
    void FixedUpdate ()
    {
   //     print(changedir);

        if (!changedir)
        {
            velocity = new Vector3(0,0,0);

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
              //  distanceFromCenter = 10;
                dir.RemoveAt(1);
                dir.Insert(1, recommandDir(angelFromNorth, "right"));
             //   changedir = false;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
             //   distanceFromCenter = 10;
                dir.RemoveAt(1);
                dir.Insert(1, recommandDir(angelFromNorth, "left"));
             //   changedir = false;
            }


            if (canGo(1, dir[dir.Count - 1]))
            {
                GeometricalDirecion = dir[dir.Count - 1];
                if(distanceFromCenter>0)
                changeDirection();

//                if (xPos == endX && zPos == endZ)
         //       {
        //            dir.RemoveAt(0);
       //             dir.Insert(1, "");
      //              changedir = false;
     //           }
            }
            else
                if (canGo(0, dir[dir.Count - 2]))
                {
                    GeometricalDirecion = dir[dir.Count - 2];
                    moveStraight();
                }
                else
                    GeometricalDirecion = "";

            if (GeometricalDirecion == "")
                transform.position = new Vector3(xPos, radius, zPos);
        }
        else //changedir
        {
            xPos = (int)transform.position.x;
            zPos = (int)transform.position.z;
            changeDirection();
            if (xPos == endX && zPos == endZ)
            {
                changedir = false;
                dir.RemoveAt(0);
                dir.Add("");
            }
      }
    }
}
