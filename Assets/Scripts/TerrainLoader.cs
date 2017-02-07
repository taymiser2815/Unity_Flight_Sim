using UnityEngine;
using System.Collections;
using System.IO;

public class TerrainLoader : MonoBehaviour
{
    // Private methods.
    //----------------------------------------------------------------------------------------------
    private void Start()
    {
        //m_terrain = ( Terrain ) gameObject.GetComponent( "Terrain" );
        m_resolution = m_offset;
        m_heightValues = new float[ m_offset, m_offset ];
        //LoadHeightmap( m_heightmap, m_terrain.terrainData);
        GenerateTerrainGrid(m_gridSize,m_height, m_offset,m_start_x,m_start_y,m_heightmap);
        //LoadTerrain(m_heightmap,m_terrain.terrainData);
        
    }

    //tms folder is = x
    //tms/x/y_h.png
    void GenerateTerrainGrid(int gridSize, int height, int offset, int start_x, int start_y, string path)
    {
        
        //Initiate vars
        TerrainData[ , ] tileArr = new TerrainData[gridSize,gridSize];
        //Initiate Tiles onto grid
        for(int x = 0; x < gridSize; x++)
        {
            for(int y = 0; y < gridSize; y++)
            {
                tileArr[x,y] = new TerrainData();
                
                Debug.Log("Tile_" + x.ToString() + "_" + y.ToString());
                Vector3 worldSize = new Vector3(offset, height, offset);
                Vector3 tilePosition = new Vector3(x*offset,0,y*offset);
                
                tileArr[x,y].heightmapResolution = offset;
                GameObject go = Terrain.CreateTerrainGameObject(tileArr[x,y]);
                go.transform.position = tilePosition;
                go.name = "Tile_" + x.ToString() + "_" + y.ToString();
                string terr_path = path + "/" + (start_x + x).ToString() + "/" + (start_y + y).ToString()  + "_h";
                Debug.Log(terr_path);
                tileArr[x,y].size = worldSize;
                Debug.Log(tileArr[x,y].size);
                LoadHeightmap(terr_path, tileArr[x,y]);
            }
        }
        /*
        //set the tiles neighbors
        for(int x = 0; x < gridSize; x++)
        {
            for(int y = 0; y < gridSize; y++)
            {
                //if tile is on the top egde
                if(y == gridSize-1)
                {
                    //if tile is top left
                    if(x == 0)
                    {
                        tileArr[x,y].//SetNeighbors(null, null, tileArr[x+1,y],tileArr[x,y-1]);
                    }
                    //if tile is top right
                    else if(x == gridSize-1)
                    {
                        tileArr[x,y].SetNeighbors( tileArr[x-1,y], null, null, tileArr[x,y-1]);
                    }
                    else
                    {
                        tileArr[x,y].SetNeighbors( tileArr[x-1,y], null, tileArr[x+1,y], tileArr[x,y-1]);
                    }

                }
                //if tile is on the bottom egde
                else if(y == 0)
                {
                    //if tile is bot left
                    if(x == 0)
                    {
                        Debug.Log("Tile_" + x.ToString() + "_" + y.ToString());
                        tileArr[x,y].SetNeighbors(tileArr[x-1,y], null, null, tileArr[x,y-1]);
                    }
                    //if tile is bot right
                    else if(x == gridSize-1)
                    {
                        tileArr[x,y].SetNeighbors( tileArr[x-1,y], tileArr[x,y+1], null, null);
                    }
                    else
                    {
                        tileArr[x,y].SetNeighbors( tileArr[x-1,y], tileArr[x,y+1], tileArr[x+1,y], null);
                    }
                }
                //if tile is on the left egde
                else if(x == 0)
                {
                    //if tile is top left
                    if(y == gridSize-1)
                    {
                        tileArr[x,y].SetNeighbors(null, null, tileArr[x+1,y],tileArr[x,y-1]);
                    }
                    //if tile is bot left
                    else if(y == 0)
                    {
                        tileArr[x,y].SetNeighbors( null, tileArr[x,y+1], tileArr[x+1,y], null);
                    }
                    else
                    {
                        tileArr[x,y].SetNeighbors(null, tileArr[x,y+1], tileArr[x+1,y], tileArr[x,y-1]);
                    }
                }
                //if tile is on the right egde
                else if(x == gridSize-1)
                {
                    //if tile is top right
                    if(y == gridSize-1)
                    {
                        tileArr[x,y].SetNeighbors(tileArr[x-1,y], null, null, tileArr[x,y-1]);
                    }
                    //if tile is bot right
                    else if(y == 0)
                    {
                        tileArr[x,y].SetNeighbors(tileArr[x-1,y], tileArr[x,y+1], null, null);
                    }
                    else
                    {
                        tileArr[x,y].SetNeighbors(tileArr[x-1,y], tileArr[x,y+1], null, tileArr[x,y-1]);
                    }
                }
            }
            
        }
        *\
        

        /*
        Terrain northWestTile = gameObject.AddComponent<Terrain>();
        Terrain northTile = gameObject.AddComponent<Terrain>();
        Terrain northEastTile = gameObject.AddComponent<Terrain>();
        Terrain westTile = gameObject.AddComponent<Terrain>();
        Terrain eastTile = gameObject.AddComponent<Terrain>();
        Terrain southWestTile = gameObject.AddComponent<Terrain>();
        Terrain southTile = gameObject.AddComponent<Terrain>();
        Terrain southEastTile = gameObject.AddComponent<Terrain>();


        //Set Tiles to center tile
        centerTile.SetNeighbors(westTile, northTile, eastTile, southTile);
        */

        


    }

    private void LoadHeightmap( string filename,  TerrainData m_terrainData)
    {
        // Load heightmap.
        Texture2D heightmap = Resources.Load<Texture2D>( filename );

        res = Resources.LoadAll("");
        

        if(heightmap != null)
        {
            // Acquire an array of colour values.
            Color[] values = heightmap.GetPixels();

            // Run through array and read height values.
            int index = 0;
            for ( int z = 0; z < heightmap.height; z++ )
            {
                for ( int x = 0; x < heightmap.width; x++ )
                {
                    m_heightValues[ z, x ] = values[ index ].r;
                    index++;
                }
            }
            

            // Now set terrain heights.
            m_terrainData.SetHeights( 0, 0, m_heightValues ); 
        }
        else
        {
            Debug.Log("File is Null " + filename);
            for( int i = 0; i < res.Length; i++)
            {
                Debug.Log(res[i].ToString());
            }
        }


    }

    // Member variables.
    //----------------------------------------------------------------------------------------------
    private Terrain    m_terrain      = null;
    private float[ , ] m_heightValues = null;
    private int        m_resolution   = 0;

    public string m_heightmap;
    public int m_gridSize = 3;

    public int m_height = 257;

    public int m_offset = 257;
    public int m_start_x = 0;

    public int m_start_y = 0;
    

    private Object[]  res;
}