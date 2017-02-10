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
        GenerateTerrainGrid(m_gridSize,m_height, m_terrainSize,m_start_x,m_start_y,m_heightmap);
        
    }

    //tms folder is = x
    //tms/x/x_y.png
    void GenerateTerrainGrid(int gridSize, int height, int offset, int start_x, int start_y, string path)
    {
        
        //Initiate vars
        TerrainData[ , ] tileArr = new TerrainData[gridSize,gridSize];
        Terrain[ , ] terrainArr = new Terrain[gridSize,gridSize];

        


        //Get textures
        Texture2D[] textures = new Texture2D[4];
        textures[0] = Resources.Load<Texture2D>("Textures/ForestFloor/forest_floor");
        textures[1] = Resources.Load<Texture2D>("Textures/GrassStraw/grass_straw");
        textures[2] = Resources.Load<Texture2D>("Textures/SolidRock/solid_rock");
        textures[3] = Resources.Load<Texture2D>("Textures/SolidRock2/solid_rock_2");
        
        SplatPrototype[] splatTexture = CreateSplatTexture(textures);

        //Initiate Tiles onto grid
        for(int x = 0; x < gridSize; x++)
        {
            for(int y = 0; y < gridSize; y++)
            {
                tileArr[x,y] = new TerrainData();
                
                Vector3 worldSize = new Vector3(offset, height, offset);
                Vector3 tilePosition = new Vector3(x*offset,0,y*offset);
                
                tileArr[x,y].heightmapResolution = m_resolution;
                tileArr[x,y].splatPrototypes = splatTexture;
                GameObject go = Terrain.CreateTerrainGameObject(tileArr[x,y]);
                go.transform.position = tilePosition;
                go.name = x.ToString() + "_" + y.ToString();//(start_x + x).ToString() + "_" + (start_y - y).ToString() + ".raw";
                terrainArr[x,y] = go.GetComponent<Terrain>();
                string terr_path = path + "/" + (start_x + x).ToString() + "/" + (start_x + x).ToString() + "_" + (start_y - y).ToString() + ".raw";
                //Debug.Log(terr_path);
                tileArr[x,y].size = worldSize;
                LoadTerrain(terr_path, tileArr[x,y]);
                assignSplatMaptoTerrain(tileArr[x,y]);
            }
        }
        //setneighbors
        setTerrainNeigboors(terrainArr);
        //fixSeam
        fixSeam(terrainArr);


    }

    SplatPrototype[] CreateSplatTexture( Texture2D[] TerrainTextures)
    {
        SplatPrototype[] tex = new SplatPrototype [TerrainTextures.Length];
        for (int i=0; i<TerrainTextures.Length; i++) {
            tex [i] = new SplatPrototype ();
            tex [i].texture = TerrainTextures [i];    //Sets the texture
            tex [i].tileSize = new Vector2 (15, 15);
        }
        return tex;
    }


    void assignSplatMaptoTerrain(TerrainData terrainData)
    {
        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
         
        for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
             {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y/(float)terrainData.alphamapHeight;
                float x_01 = (float)x/(float)terrainData.alphamapWidth;
                 
                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight),Mathf.RoundToInt(x_01 * terrainData.heightmapWidth) );
                 
                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01,x_01);
      
                // Calculate the steepness of the terrain
                float steepness = terrainData.GetSteepness(y_01,x_01);
                 
                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];
                 
                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT
     
                // Texture[0] has constant influence
                splatWeights[0] = 0.5f;
                 
                // Texture[1] is stronger at lower altitudes
                splatWeights[1] = Mathf.Clamp01((terrainData.heightmapHeight - height));
                 
                // Texture[2] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                splatWeights[2] = 1.0f - Mathf.Clamp01(steepness*steepness/(terrainData.heightmapHeight/5.0f));
                 
                //Texture[3] increases with height but only on surfaces facing positive Z axis 
                splatWeights[3] = height * Mathf.Clamp01(normal.z);
                 
                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights[0] + splatWeights[1] + splatWeights[2] + splatWeights[3];
                 
                // Loop through each terrain texture
                for(int i = 0; i<terrainData.alphamapLayers; i++)
                {
                     
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;
                     
                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }
      
        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    void setTerrainNeigboors(Terrain[ , ] terrainArr)
    {
        int gridSize = m_gridSize;
        for(int x = 0; x < gridSize; x++)
        {
            for(int y = 0; y < gridSize; y++)
            {
                string str = "(" + x.ToString() + "," + y.ToString() + ") - ";
                //if tile is on the top egde
                if(y == gridSize-1)
                {
                    //if tile is top left
                    if(x == 0)
                    {
                        terrainArr[x,y].SetNeighbors(null, null, terrainArr[x+1,y],terrainArr[x,y-1]);
                        str += "null,null," + terrainArr[x+1,y].name + "," + terrainArr[x,y-1].name;
                    }
                    //if tile is top right
                    else if(x == gridSize-1)
                    {
                        terrainArr[x,y].SetNeighbors( terrainArr[x-1,y], null, null, terrainArr[x,y-1]);
                        str +=  terrainArr[x-1,y].name + ",null,null," + terrainArr[x,y-1].name;
                    }
                    else
                    {
                        terrainArr[x,y].SetNeighbors( terrainArr[x-1,y], null, terrainArr[x+1,y], terrainArr[x,y-1]);
                        str += terrainArr[x-1,y].name + ",null," + terrainArr[x+1,y].name + "," + terrainArr[x,y-1].name;
                    }

                }
                //if tile is on the bottom egde
                else if(y == 0)
                {
                    //if tile is bot left
                    if(x == 0)
                    {
                        terrainArr[x,y].SetNeighbors(null, terrainArr[x,y+1],  terrainArr[x+1,y],null);
                        str +=  "null," + terrainArr[x,y+1].name + "," + terrainArr[x+1,y].name + ",null";
                    }
                    //if tile is bot right
                    else if(x == gridSize-1)
                    {
                        terrainArr[x,y].SetNeighbors( terrainArr[x-1,y], terrainArr[x,y+1], null, null);
                        str +=  terrainArr[x-1,y].name + "," + terrainArr[x,y+1].name + ",null,null";
                    }
                    else
                    {
                        terrainArr[x,y].SetNeighbors( terrainArr[x-1,y], terrainArr[x,y+1], terrainArr[x+1,y], null);
                        str +=  terrainArr[x-1,y].name + "," + terrainArr[x,y+1].name + terrainArr[x+1,y].name + ",null";
                    }
                }
                //if tile is on the left egde
                else if(x == 0)
                {
                    //if tile is top left
                    if(y == gridSize-1)
                    {
                        terrainArr[x,y].SetNeighbors(null, null, terrainArr[x+1,y],terrainArr[x,y-1]);
                        str += "null,null," + terrainArr[x+1,y].name + "," + terrainArr[x,y-1].name;
                    }
                    //if tile is bot left
                    else if(y == 0)
                    {
                        terrainArr[x,y].SetNeighbors( null, terrainArr[x,y+1], terrainArr[x+1,y], null);
                        str += "null," + terrainArr[x,y+1].name + "," + terrainArr[x+1,y].name + ",null";
                    }
                    else
                    {
                        terrainArr[x,y].SetNeighbors(null, terrainArr[x,y+1], terrainArr[x+1,y], terrainArr[x,y-1]);
                        str += "null," + terrainArr[x,y+1].name + "," + terrainArr[x+1,y].name + "," + terrainArr[x,y-1];
                    }
                }
                //if tile is on the right egde
                else if(x == gridSize-1)
                {
                    //if tile is top right
                    if(y == gridSize-1)
                    {
                        terrainArr[x,y].SetNeighbors(terrainArr[x-1,y], null, null, terrainArr[x,y-1]);
                        str +=  terrainArr[x-1,y].name + ",null,null," + terrainArr[x,y-1].name;
                    }
                    //if tile is bot right
                    else if(y == 0)
                    {
                        terrainArr[x,y].SetNeighbors(terrainArr[x-1,y], terrainArr[x,y+1], null, null);
                        str +=  terrainArr[x-1,y].name + "," + terrainArr[x,y+1].name + ",null,null";
                    }
                    else
                    {
                        terrainArr[x,y].SetNeighbors(terrainArr[x-1,y], terrainArr[x,y+1], null, terrainArr[x,y-1]);
                        str +=  terrainArr[x-1,y].name + "," + terrainArr[x,y+1].name + ",null," + terrainArr[x,y-1].name;
                    }
                }
                else
                {
                    terrainArr[x,y].SetNeighbors(terrainArr[x-1,y], terrainArr[x,y+1], terrainArr[x+1,y], terrainArr[x,y-1]);
                    str += terrainArr[x-1,y].name + "," + terrainArr[x,y+1].name + "," + terrainArr[x+1,y].name +"," + terrainArr[x,y-1].name;
                }
                Debug.Log(str);
            }
            
        }
        

    }

    void fixSeam(Terrain[ , ] terrainArr)
    {
        for(int x = 0; x < m_gridSize; x++)
        {
            for(int y = 0; y < m_gridSize; y++)
            {
                string str = "";
                Terrain cur;
                try
                {
                    cur = terrainArr[x,y];
                    str += cur.name + ',';
                }
                catch
                {
                    cur = null;
                    str += "null,";
                }
                Terrain right;
                try
                {
                    right = terrainArr[x+1,y];
                    str += right.name+ ',';
                }
                catch
                {
                    right = null;
                    str += "null,";
                }
                Terrain bottom;
                try
                {
                    bottom = terrainArr[x,y-1];
                    str += bottom.name;
                }
                catch
                {
                    bottom = null;
                    str += "null";
                }
                //Debug.Log(str);
                float[,] newHeights = new float[m_resolution, m_resolution];
                float[,] rightHeights = new float[m_resolution, m_resolution], bottomHeights = new float[m_resolution, m_resolution];
        
                if (right != null)
                {
                    rightHeights = right.terrainData.GetHeights(0, 0, m_resolution, m_resolution);
                }
                if (bottom != null)
                {
                    bottomHeights = bottom.terrainData.GetHeights(0, 0, m_resolution, m_resolution);
                }
        
                if (right != null || bottom != null)
                {
        
                    newHeights = cur.terrainData.GetHeights(0, 0, m_resolution, m_resolution);
        
                    for (int i = 0; i < m_resolution; i++)
                    {
                        if (right != null)
                            newHeights[i, m_resolution - 1] = rightHeights[i, 0];
        
                        if (bottom != null)
                            newHeights[0, i] = bottomHeights[m_resolution - 1, i];
                    }
                    cur.terrainData.SetHeights(0, 0, newHeights);
                }
            }
        }
    }


    void LoadTerrain(string aFileName, TerrainData aTerrain)
    {
        Debug.Log(m_resolution.ToString());
        int h = m_resolution;//aTerrain.heightmapHeight;
        int w = m_resolution;//aTerrain.heightmapWidth;
        float[,] data = new float[h, w];
        var file = File.OpenRead(aFileName);
        BinaryReader reader = new BinaryReader(file);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float v = (float)reader.ReadUInt16() / 0xFFFF;
                    data[y, x] = v;
                }
            }
        aTerrain.SetHeights(0, 0, data);
        file.Close();
    }






/*
    private void LoadHeightmap( string filename,  TerrainData m_terrainData)
    {
        // Load heightmap.
        Texture2D heightmap = Resources.Load<Texture2D>( filename );

        res = Resources.LoadAll("");
        for(int x =0; x < res.Length; x++)
        {
            Debug.Log(res[x].name);
        }

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
                    Debug.Log(values[ index ].r.ToString());
                    index++;
                }
            }
            

            // Now set terrain heights.
            m_terrainData.SetHeights( 0, 0, m_heightValues ); 
        }
        else
        {
            Debug.Log("File is Null " + filename);
        }


    }
*/
    // Member variables.
    //----------------------------------------------------------------------------------------------
    private Terrain    m_terrain      = null;
    private float[ , ] m_heightValues = null;
    private int        m_resolution   = 0;

    public string m_heightmap;
    public int m_gridSize = 3;

    public int m_terrainSize = 257;

    public int m_height = 257;

    public int m_offset = 257;
    public int m_start_x = 0;

    public int m_start_y = 0;
    

    private Object[]  res;
}