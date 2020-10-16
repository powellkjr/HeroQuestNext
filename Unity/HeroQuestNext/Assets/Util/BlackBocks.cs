using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBocks 
{
    public static readonly Vector2Int vNorth =     new Vector2Int( 0,  1);
    public static readonly Vector2Int vNorthEast = new Vector2Int( 1,  1);
    public static readonly Vector2Int vEast =      new Vector2Int( 1,  0);
    public static readonly Vector2Int vSouthEast = new Vector2Int( 1, -1);
    public static readonly Vector2Int vSouth =     new Vector2Int( 0, -1);
    public static readonly Vector2Int vSouthWest = new Vector2Int(-1, -1);
    public static readonly Vector2Int vWest =      new Vector2Int(-1,  0);
    public static readonly Vector2Int vNorthWest = new Vector2Int(-1,  1);

    public static TextMesh CreateWorldText(
        string text = "Text",
        Transform parent = null,
        Vector3 localPosition = default(Vector3),
        int fontSize = 40,
        Color? color = null,
        TextAnchor textAnchor = TextAnchor.MiddleCenter,
        TextAlignment textAlignment = TextAlignment.Center,
        int sortingOrder = 5000)

    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    public static TextMesh CreateWorldText(
        Transform parent,
        string text ,
        Vector3 localPosition,
        int fontSize,
        Color color,
        TextAnchor textAnchor,
        TextAlignment textAlignment,
        int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent,false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    public static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public static Vector3 GetMouseWorldPositionWithZ(
        Camera inWorldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition,inWorldCamera);
    }
    public static Vector3 GetMouseWorldPositionWithZ(
        Vector3 inScreenPostion,
        Camera inWorldCamera)
    {
        Vector3 vWorldPostion = inWorldCamera.ScreenToWorldPoint(inScreenPostion);
        return vWorldPostion;
    }

    public static void CreateEmptyMeshArrays(
        int inQuadCount,
        out Vector3[] vVertices,
        out Vector2[] vUVs,
        out int[] iTriangles)
    {
        vVertices = new Vector3[4 * inQuadCount];
        vUVs = new Vector2[4 * inQuadCount];
        iTriangles = new int[6 * inQuadCount];
    }

    public static void AddToMeshArrays(
        Vector3[] vVertices,
        Vector2[] vUVs,
        int[] iTriangles,
        int index,
        Vector3 vPos,
        float fRot,
        Vector3 vBaseSize,
        Vector2 vUV00,
        Vector2 vUV11
        )
    {
        //Relocate Verticies
        int vIndex = index * 4;
        int vIndex0 = vIndex + 0;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        vBaseSize *= .5f;

        bool bSkewed = vBaseSize.x != vBaseSize.y;
        if (bSkewed)
        {
            vVertices[vIndex0] = vPos + GetQuaternionEuler(fRot) * new Vector3(-vBaseSize.x,  vBaseSize.y);
            vVertices[vIndex1] = vPos + GetQuaternionEuler(fRot) * new Vector3(-vBaseSize.x, -vBaseSize.y);
            vVertices[vIndex2] = vPos + GetQuaternionEuler(fRot) * new Vector3( vBaseSize.x, -vBaseSize.y);
            vVertices[vIndex3] = vPos + GetQuaternionEuler(fRot) * new Vector3( vBaseSize.x,  vBaseSize.y);
        }
        else
        {
            vVertices[vIndex0] = vPos + GetQuaternionEuler(fRot - 270) * vBaseSize;
            vVertices[vIndex1] = vPos + GetQuaternionEuler(fRot - 180) * vBaseSize;
            vVertices[vIndex2] = vPos + GetQuaternionEuler(fRot - 090) * vBaseSize;
            vVertices[vIndex3] = vPos + GetQuaternionEuler(fRot - 000) * vBaseSize;
        }

        //Relocate UVs
        vUVs[vIndex0] = new Vector2(vUV00.x, vUV11.y);
        vUVs[vIndex1] = new Vector2(vUV00.x, vUV00.y);
        vUVs[vIndex2] = new Vector2(vUV11.x, vUV00.y);
        vUVs[vIndex3] = new Vector2(vUV11.x, vUV11.y);

        //Create triangles
        int iThisTriangle = index * 6;
        iTriangles[iThisTriangle + 0] = vIndex0;
        iTriangles[iThisTriangle + 1] = vIndex3;
        iTriangles[iThisTriangle + 2] = vIndex1;

        iTriangles[iThisTriangle + 3] = vIndex1;
        iTriangles[iThisTriangle + 4] = vIndex3;
        iTriangles[iThisTriangle + 5] = vIndex2;

    }



    private static Quaternion[] arrCachedQuaternionEuler;

    private static void CacheQuaternionEuler()
    {
        if (arrCachedQuaternionEuler != null) return;
      
        arrCachedQuaternionEuler = new Quaternion[360];
        for (int i = 0; i <360; i++)
        {
            arrCachedQuaternionEuler[i] = Quaternion.Euler(0, 0, i);
        }
        
    }
    private static Quaternion GetQuaternionEuler (float fRotation)
    {
        int iRotation = Mathf.RoundToInt(fRotation);
        iRotation %= 360;
        if (iRotation < 0) iRotation += 360;
        if (arrCachedQuaternionEuler == null) CacheQuaternionEuler();
        return arrCachedQuaternionEuler[iRotation];
    }

    public static Color GetRandomColor(Color inColor00, Color inColor11)
    {
        return new Color(Random.Range(inColor00.r, inColor11.r), Random.Range(inColor00.g, inColor11.g), Random.Range(inColor00.b, inColor11.b));
    }

    public static void TintColorArray(Color[] inBaseArray, Color inTint)
    {
        for (int i = 0; i < inBaseArray.Length; i++)
        {
            inBaseArray[i].r *= inTint.r/255f;
            inBaseArray[i].g *= inTint.g/255f;
            inBaseArray[i].b *= inTint.b/255f;
        }
    }
}

