using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Testing");
        Mesh mMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mMesh;

        Vector3[] vVertices = new Vector3[4];
        Vector2[] vUV = new Vector2[4];
        int[] iTriangels = new int[6];

        vVertices[0] = new Vector3(0, 0);
        vVertices[1] = new Vector3(0, 100);
        vVertices[2] = new Vector3(100, 100);
        vVertices[3] = new Vector3(100, 0);

        iTriangels[0] = 0;
        iTriangels[1] = 1;
        iTriangels[2] = 2;

        iTriangels[3] = 0;
        iTriangels[4] = 2;
        iTriangels[5] = 3;
        

        vUV[0] = new Vector2(0, 0);
        vUV[1] = new Vector2(0, 1);
        vUV[2] = new Vector2(1, 1);
        vUV[3] = new Vector2(1, 0);


        mMesh.vertices = vVertices;
        mMesh.uv = vUV;
        mMesh.triangles = iTriangels;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
