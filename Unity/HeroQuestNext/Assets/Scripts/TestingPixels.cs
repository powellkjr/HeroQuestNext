using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingPixels : MonoBehaviour
{
    private Mesh mMesh;
    [SerializeField] private Texture2D tFurnitureTiles;
    [SerializeField] private Texture2D tFurnitureTilesMask;

    private Texture2D tHeroBasePixels;

    [SerializeField] private Texture2D tHeroMapMainSpriteSheet;
    //[SerializeField] private Texture2D tHeroMapMainSpriteSheetCheck;
    [SerializeField] private Material mHeroMapVisualMaterial;
    // Start is called before the first frame update
    void Start()
    {
        mMesh = new Mesh { };
        GetComponent<MeshFilter>().mesh = mMesh;
        //tHeroBasePixels = GetComponent<Texture2D>();


        //tHeroMapMainSpriteSheet = new Texture2D(iNumFurnitureTiles * vBaseTileReference.x, iBaseTile * iNumPages, TextureFormat.RGBA32, true);
        //Color[] cClear = tHeroMapMainSpriteSheet.GetPixels(0, 0, tHeroBasePixels.width, tHeroBasePixels.height);
        //for(int i =0; i < cClear.Length; i++)
        //{
        //    cClear[i] = Color.clear;
        //}
        //tHeroMapMainSpriteSheet.SetPixels(0, 0, tHeroMapMainSpriteSheet.width, tHeroMapMainSpriteSheet.height, cClear);

        tHeroMapMainSpriteSheet = new Texture2D(8, 1, TextureFormat.RGBA32, true);
        Color[] arrClearPixels = new Color[8];
        for (int i = 0; i < arrClearPixels.Length; i++)
        {
            arrClearPixels[i] = Color.clear;
        }
        tHeroMapMainSpriteSheet.SetPixels(0, 0, 8, 1, arrClearPixels);
        tHeroMapMainSpriteSheet.Apply();
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;



        //copy
        Color[] arrSrcPixels = tFurnitureTiles.GetPixels(0,0,8,1);
        Color[] arrMaskPixels = tFurnitureTilesMask.GetPixels(0, 0, 8, 1);
        Color[] arrTintPixels = new Color[] { BlackBocks.GetRandomColor(new Color(.5f, .5f, 0, 1), new Color(.9f, .9f, 0, 1)) };

        //color
        //BlackBocks.TintColorArray(arrSrcPixels, inTint[aSrcSpriteIndexAndSrcRect.iSrcSpriteIndex % inTint.Length]);
        BlackBocks.TintColorArrayInsideMask(arrSrcPixels, arrTintPixels[0], arrMaskPixels);
        //locate
        RectInt rMainUVRect = new RectInt(0,0,8,1);
        //paste
        //tHeroMapMainSpriteSheet.SetPixels(rMainUVRect.x, rMainUVRect.y, rMainUVRect.width, rMainUVRect.height, arrSrcPixels);
        Color[] arrBasePixels = tHeroMapMainSpriteSheet.GetPixels(rMainUVRect.x, rMainUVRect.y, rMainUVRect.width, rMainUVRect.height);
        //BlackBocks.MergeColorArray(arrBasePixels, arrMaskPixels);
        BlackBocks.MergeColorArray(arrBasePixels, arrSrcPixels);
        tHeroMapMainSpriteSheet.SetPixels(rMainUVRect.x, rMainUVRect.y, rMainUVRect.width, rMainUVRect.height, arrBasePixels);
        tHeroMapMainSpriteSheet.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
