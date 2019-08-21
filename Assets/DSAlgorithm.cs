using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
TODO:
 */
public class DSAlgorithm : MonoBehaviour
{
    // must be 2^n + 1;
    public int maxVertics = 65;
    public float maxHeight = 10.0f;
    public float roughness = 0.4f;

    public struct coor  
    {
        public int x;
        public int y;
        public coor(int a, int b) {
            x = a;
            y = b;
        }
    }

    // Start is called before the first frame update
    float[,] calHeights()
    {
        // float[,] heights = new float[maxVertics,maxVertics];
        // coor[] heights = new coor[maxVertics];

        // initialize
        // heights[0,0] = topL;
        // heights[0,maxVertics-1] = topR;5
        // heights[maxVertics-1,0] = botL;
        // heights[maxVertics-1,maxVertics-1] = botR;
        // System.Random rand = new System.Random();

        float d = (float)Random.Range(0, maxHeight);
        int radius = maxVertics-1;
        Stack<coor> dimond = new Stack<coor>();
        List<coor> square = new List<coor>() {
            new coor(0, 0)
        };
        float[,] heights = new float[maxVertics, maxVertics];

        // initialize
        heights[0,0] = (float)Random.Range(0, maxHeight);
        heights[0,radius] = (float)Random.Range(0, maxHeight);
        heights[radius,0] = (float)Random.Range(0, maxHeight);
        heights[radius,radius] = (float)Random.Range(0, maxHeight);

        // perform dimond/square step
        // halve the radius each step
        while (radius > 0) 
        {

            if (dimond.Count == 0) 
            {
                // square step
                List<coor> tempList = new List<coor>(square);
                foreach (var i in tempList) 
                {
                    // find top left corner only
                    coor topR_c = new coor(i.x, i.y+radius);
                    coor botL_c = new coor(i.x+radius, i.y);
                    coor botR_c = new coor(i.x+radius, i.y+radius);

                    // only continue if 4 points create a valid square.
                    if (!isValidSquare(i, botR_c, botL_c, topR_c))
                        continue;

                    // halve the radius to get the middle point
                    coor middle_c = new coor (i.x+radius/2, i.y+radius/2);

                    // record new square points
                    // do I need to sort? maybe no
                    // if (!square.Contains(topR_c))
                        square.Add(topR_c);
                    // if (!square.Contains(botL_c))
                        square.Add(botL_c);
                    // if (!square.Contains(botR_c))
                        square.Add(botR_c);
                    // if (!square.Contains(middle_c))
                        square.Add(middle_c);

                    // debug
                    // Debug.Log(square.Count);
                    // foreach (var m in square)
                    // {
                    //     Debug.Log(""+ m.x + ","+m.y);
                    // }
                    // Debug.Log("=================");

                    // record dimond points
                    dimond.Push(middle_c);

                    // record dimond new height
                    heights[middle_c.x,middle_c.y] = (heights[i.x,i.y] + 
                                                        heights[topR_c.x,topR_c.y] +
                                                        heights[botL_c.x,botL_c.y] + 
                                                        heights[botR_c.x,botR_c.y]) / 4 + (float)Random.Range(-d, d);
                }
                
                // halve the radius
                radius /= 2;
            } 
            else // dimond step 
            {
                while (dimond.Count != 0)
                {
                    coor pt = dimond.Pop();
                    coor top = new coor(pt.x-radius, pt.y);
                    coor bot = new coor(pt.x+radius, pt.y);
                    coor left = new coor(pt.x, pt.y-radius);
                    coor right = new coor (pt.x, pt.y+radius);

                    // record new square points;
                    // if (!square.Contains(top))
                        square.Add(top);
                    // if (!square.Contains(bot))
                        square.Add(bot);
                    // if (!square.Contains(left))
                        square.Add(left);
                    // if (!square.Contains(right))
                        square.Add(right);
                    
                    // debug
                    // Debug.Log(square.Count);
                    // foreach (var m in square)
                    // {
                    //     Debug.Log(""+ m.x + ","+m.y);
                    // }
                    // Debug.Log("++++++++++++++++++++++");
                    
                    // calculate new heights
                    coor topR_c = new coor(pt.x+radius, pt.y-radius);
                    coor botL_c = new coor(pt.x-radius, pt.y+radius);
                    coor botR_c = new coor(pt.x+radius, pt.y+radius);
                    coor topL_c = new coor (pt.x-radius, pt.y-radius);
                    coor temp;

                    // left, use top left and bot left points, plus an extra point which is 2*radius away at left
                    if (pt.x - 2*radius < 0)
                    {
                        temp = new coor(pt.x, pt.y);
                    } else {
                        temp = new coor(pt.x-radius*2, pt.y);
                    }
                    heights[left.x,left.y] = (heights[pt.x,pt.y] + 
                                              heights[temp.x,temp.y] +
                                              heights[topL_c.x,topL_c.y] + 
                                              heights[botL_c.x,botL_c.y]) / 4 + (float)Random.Range(-d, d);
                    // right 
                    if (pt.x + 2*radius >= maxVertics)
                    {
                        temp = new coor(pt.x, pt.y);
                    } else {
                        temp = new coor(pt.x+radius*2, pt.y);
                    }
                    heights[right.x,right.y] = (heights[pt.x,pt.y] + 
                                                        heights[temp.x,temp.y] +
                                                        heights[topR_c.x,topR_c.y] + 
                                                        heights[botR_c.x,botR_c.y]) / 4 + (float)Random.Range(-d, d);
                    // top
                    if (pt.y - 2*radius < 0)
                    {
                        temp = new coor(pt.x, pt.y);
                    } else {
                        temp = new coor(pt.x, pt.y-radius*2);
                    }
                    heights[top.x,top.y] = (heights[pt.x,pt.y] + 
                                            heights[temp.x,temp.y] +
                                            heights[topL_c.x,topL_c.y] + 
                                            heights[topR_c.x,topR_c.y]) / 4 + (float)Random.Range(-d, d);
                    // bot
                    if (pt.y + 2*radius >= maxVertics)
                    {
                        temp = new coor(pt.x, pt.y);
                    } else {
                        temp = new coor(pt.x, pt.y+radius*2);
                    }
                    heights[bot.x,bot.y] = (heights[pt.x,pt.y] + 
                                            heights[temp.x,temp.y] +
                                            heights[botL_c.x,botL_c.y] + 
                                            heights[botR_c.x,botR_c.y]) / 4 + (float)Random.Range(-d, d);
                }
            }

            // reduce d
            d = (float)System.Math.Pow(d, -roughness);
        }

        // debug
        // Debug.Log(square.Count);
        // foreach (var i in square)
        // {
        //     Debug.Log(""+ i.x + ","+i.y);
        // }

        return heights;
    }

    Mesh createLandScape()
    {
        List<Vector3> pts = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();

        float[,] heights = calHeights();
        for (int i=0; i<maxVertics; i++)
        {
            for (int j=0; j<maxVertics; j++)
            {
                Vector3 pt = new Vector3(i, heights[i, j], j);
                pts.Add(pt);
                colors.Add(Color.black);
            }
        }

        for (int i=0; i<maxVertics-1; i++)
        {
            for (int j=0; j<maxVertics-1; j++)
            {
                triangles.Add(i*maxVertics+j);
                triangles.Add(i*maxVertics+j+1);
                triangles.Add((i+1)*maxVertics+j+1);

                triangles.Add(i*maxVertics+j);
                triangles.Add((i+1)*maxVertics+j+1);
                triangles.Add((i+1)*maxVertics+j);
            }
        }

        Mesh m = new Mesh();
        m.name = "landscape";
		m.vertices = pts.ToArray();
		m.colors = colors.ToArray();
		m.triangles = triangles.ToArray();

        m.RecalculateBounds();
        m.RecalculateNormals();

        return m;
    }

    // void OnDrawGizmos()
    // {
    //     for (int i=0; i<maxVertics; i++)
    //     {
    //         for (int j=0; j<maxVertics; j++)
    //         {
    //             Vector3 pt = new Vector3(i, debugHeights[i, j], j);
    //             Gizmos.color = Color.black;
    //             Gizmos.DrawSphere(pt, 0.1f);
    //         }
    //     }
    // }

    void Start()
    {
        MeshFilter cubeMesh = this.gameObject.AddComponent<MeshFilter>();
        cubeMesh.mesh = this.createLandScape();

        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material.shader = Shader.Find("Unlit/VertexColorShader");
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    bool isValidPt(coor pt)
    {
        return pt.x >= 0 && pt.x < maxVertics && pt.y >=0 && pt.y < maxVertics;
    }

    bool isValidSquare(coor tl, coor tr, coor bl, coor br)
    {
        return isValidPt(tl) && isValidPt(tr) && isValidPt(bl) && isValidPt(br);
    }
}
