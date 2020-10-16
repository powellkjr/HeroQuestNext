using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Serialization;
using UnityEngine;

public class HeatMapVisual : MonoBehaviour
{
    private BlackBocksGrid<HeatMapGridObject> arrGrid;
    private Mesh mMesh;
    private bool bUpdateMesh;
    [SerializeField] public bool bDebugEnabled;
    private void Awake()
    {
        mMesh = new Mesh { };
        GetComponent<MeshFilter>().mesh = mMesh;


    }
    // Start is called before the first frame update
    void Start()
    {
        //arrGrid = new Grid<int>(20, 20, 10f, Vector3.zero);
        
    }

    public void SetGrid(BlackBocksGrid<HeatMapGridObject> inGrid)
    {
        this.arrGrid = inGrid;

        inGrid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, BlackBocksGrid<HeatMapGridObject>.OnGridValueChangedEventArgs e)
    {
        //UpdateHeatMapVisuals();
        bUpdateMesh = true;
    }

    private void LateUpdate()
    {
        if (bUpdateMesh)
        {
            UpdateHeatMapVisuals();
            bUpdateMesh = false;
        }
    }
    public void UpdateHeatMapVisuals()
    {
        BlackBocks.CreateEmptyMeshArrays(arrGrid.GetWidth() * arrGrid.GetHeight(), out Vector3[] vVertices, out Vector2[] vUVs, out int[] iTriangles);
        Vector3 vQuadSize = arrGrid.GetCellSize() * new Vector3(1, 1);
        for( int x = 0; x < arrGrid.GetWidth(); x++)
        {
            for (int y = 0; y < arrGrid.GetHeight(); y++)
            {
                int i = x * arrGrid.GetHeight() + y;
                Debug.Log(i + ":" + x + "," + y);
                
                Vector2 vGridUV = new Vector2(arrGrid.GetGridObject(x, y).GetValueNormalized(),0f);
                BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i, arrGrid.GetWorldPosition(x, y) + vQuadSize * .5f, 0, vQuadSize, vGridUV, vGridUV);
            }
        }

        mMesh.vertices = vVertices;
        mMesh.uv = vUVs;
        mMesh.triangles = iTriangles;
    }
    // Update is called once per frame
    void Update()
    {
        //UpdateHeatMapVisuals();
    }
}
