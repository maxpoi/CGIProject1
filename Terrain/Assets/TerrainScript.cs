using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    // width of the terrain ↓
    public float width = 20.0f;
    // num of vertices per row ↓
    public int nVertices = 65; 
    public float maxPosHeight = 5.0f;
    public Texture waterTexture;

    private Vector3[] vertices;
    private float waterHeight;

    // perform diamond-square algorithm to generate a height map
    // with reference on: https://www.youtube.com/watch?v=1HV8GbFnCik&t=540s
    Mesh CreateLandscape()
    {
        float halfWidth = width / 2;
        int nSteps = nVertices - 1;
        float stepSize = width / nSteps;
        vertices = new Vector3[nVertices * nVertices];
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < nVertices; i++)
        {
            for (int j = 0; j < nVertices; j++)
            {
                vertices[nVertices*i+j] = new Vector3(-halfWidth + (j*stepSize),
                                                      0.0f,
                                                      halfWidth - (i*stepSize));

                if ((i < nSteps) && (j < nSteps))
                {
                    triangles.Add(nVertices * i + j);
                    triangles.Add(nVertices * i + j + 1);
                    triangles.Add(nVertices * (i + 1) + j + 1);

                    triangles.Add(nVertices * i + j);
                    triangles.Add(nVertices * (i + 1) + j + 1);
                    triangles.Add(nVertices * (i + 1) + j);
                }
            }
        }

        // initialise height for corners
        vertices[0].y = Random.Range(-maxPosHeight, maxPosHeight);
        vertices[nSteps].y = Random.Range(-maxPosHeight, maxPosHeight);
        vertices[(nVertices*nVertices)-nVertices].y = Random.Range(-maxPosHeight,
                                                                   maxPosHeight);
        vertices[(nVertices*nVertices)-1].y = Random.Range(-maxPosHeight,
                                                           maxPosHeight);

        // diamond-square algorithm to generate height for each vertices
        int nSquares = 1;
        while (nSteps > 1)
        {
            int[] topLs = TopLs(nSquares, nSteps);
            foreach (int topL_c in topLs)
            {
                DiamondSquare(topL_c, nSteps, maxPosHeight);
            }

            nSquares *= 4;
            nSteps /= 2;
            maxPosHeight *= 0.5f;
        }

        float maxHeight = -maxPosHeight, minHeight = maxPosHeight;
        List<float> allHeights = new List<float>();
        foreach (Vector3 vertex in vertices)
        {
            float height = vertex.y;
            if (height > maxHeight)
                maxHeight = height;
            if (height < minHeight)
                minHeight = height;
            allHeights.Add(height);
        }

        // assign color to each vertex
        float heightRange = maxHeight - minHeight;
        // set water height
        waterHeight = minHeight + heightRange * 0.3f;
        foreach (float height in allHeights)
        {
            // snow on top
            if ((height > (maxHeight - heightRange * 1 / 5)) &&
                (height <= maxHeight))
                colors.Add(Color.white);
            // grass at mid
            else if ((height > (minHeight + heightRange * 1 / 3)) &&
                     (height <= (maxHeight - heightRange * 1 / 5)))
                colors.Add(new Color(0.235f, 0.431f, 0.159f));
            // sand at bottom
            else
                colors.Add(new Color(0.55f, 0.4f, 0.0f));
        }

        Mesh m = new Mesh();
        m.name = "landscape";
        m.vertices = vertices;
        m.triangles = triangles.ToArray();
        m.colors = colors.ToArray();

        m.RecalculateBounds();
        m.RecalculateNormals();

        triangles.Clear();
        colors.Clear();
        allHeights.Clear();

        return m;
    }

    // diamond-square algorithm
    void DiamondSquare(int topL_i, int nSteps, float range)
    {
        // diamond step
        int botL_i = topL_i + nVertices * nSteps;
        int topR_i = topL_i + nSteps;
        int botR_i = botL_i + nSteps;

        int halfSteps = nSteps / 2;
        int mid_i = halfSteps * nVertices + (halfSteps + topL_i);
        vertices[mid_i].y = (vertices[topL_i].y + vertices[botL_i].y +
                             vertices[topR_i].y + vertices[botR_i].y) * 0.25f +
                             Random.Range(-range, range);

        
        // square step
        // top
        int top_i = topL_i + halfSteps;
        if (isOnSide(top_i))
        {
            vertices[top_i].y = sideSquare(topL_i, topR_i, mid_i, range);
        }
        else
        {
            vertices[top_i].y = withinSquare(topL_i, topR_i, mid_i,
                                             top_i-nVertices*(nSteps/2), range);
        }

        // left
        int left_i = mid_i - halfSteps;
        if (isOnSide(left_i))
        {
            vertices[left_i].y = sideSquare(topL_i, botL_i, mid_i, range);
        }
        else
        {
            vertices[left_i].y = withinSquare(topL_i, botL_i, mid_i,
                                              top_i-nSteps/2, range);
        }

        // right
        int right_i = mid_i + halfSteps;
        if (isOnSide(right_i))
        {
            vertices[right_i].y = sideSquare(topR_i, botR_i, mid_i, range);
        }
        else
        {
            vertices[right_i].y = withinSquare(topR_i, botR_i, mid_i,
                                               top_i+nSteps/2, range);
        }

        // bottom
        int bot_i = botL_i + halfSteps;
        if (isOnSide(bot_i))
        {
            vertices[bot_i].y = sideSquare(botL_i, botR_i, mid_i, range);
        }
        else
        {
            vertices[bot_i].y = withinSquare(botL_i, botR_i, mid_i,
                                             top_i+nVertices*(nSteps/2), range);
        }
    }

    // determine topL_c for all squares
    int[] TopLs(int nSquares, int nSteps)
    {
        int[] topLs = new int[nSquares];
        int topLcount = 0;
        int init = 0;
        while ((init + 1) % nVertices != 0)
        {
            topLs[topLcount] = init;
            topLcount++;

            int botL_i = nSteps * nVertices + init;
            while (botL_i < nVertices * (nVertices - 1))
            {
                topLs[topLcount] = botL_i;
                topLcount++;

                botL_i += nSteps * nVertices;
            }
            init += nSteps;
        }

        return topLs;
    }

    // determine whether given vertex is on the side of the height map
    bool isOnSide(int ver_i)
    {
        if ((ver_i < nVertices) || (ver_i >= nVertices * (nVertices - 1)) ||
            (ver_i % nVertices == 0) || ((ver_i + 1) % nVertices == 0))
        {
            return true;
        }

        return false;
    }

    // square step for vertex which on the side of the height map
    // (i.e. average of 3 vertices)
    float sideSquare(int ver1, int ver2, int ver3, float range)
    {
        return (vertices[ver1].y + vertices[ver2].y + vertices[ver3].y) / 3
               + Random.Range(-range, range);
    }

    // square step for vertex which NOT on the side of the height map
    // (i.e. average of 4 vertices)
    float withinSquare(int ver1, int ver2, int ver3, int ver4, float range)
    {
        return (vertices[ver1].y + vertices[ver2].y + vertices[ver3].y +
                vertices[ver4].y) / 4 + Random.Range(-range, range);
    }

    // Start is called before the first frame update
    void Start()
    {
        // create mountaion
        MeshFilter terrainMesh = this.gameObject.AddComponent<MeshFilter>();
        terrainMesh.mesh = this.CreateLandscape();

        // color mountain
        MeshRenderer mountRenderer = this.gameObject.AddComponent<MeshRenderer>();
        mountRenderer.material.shader = Shader.Find("UI/Lit/Bumped");

        // create water plane
        GameObject waterPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterPlane.transform.localPosition = new Vector3(0, waterHeight, 0);
        // scale up to create the correct water plane size ↓
        float scaleUp = width / 10.0f;
        waterPlane.transform.localScale = new Vector3(scaleUp, scaleUp, scaleUp);

        // color water
        MeshRenderer waterRenderer = waterPlane.GetComponent<MeshRenderer>();
        waterRenderer.material.shader = Shader.Find("Unlit/WaveShader");
        waterRenderer.material.mainTexture = waterTexture;
        //waterRenderer.material.color = new Color(0.196f, 0.392f, 0.804f, 1.0f);
    }
}
