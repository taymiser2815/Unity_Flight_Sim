using UnityEngine;
using System.Collections;

public class TerrainLoader : MonoBehaviour
{
    // Private methods.
    //----------------------------------------------------------------------------------------------
    private void Start()
    {
        m_terrain = ( Terrain ) gameObject.GetComponent( "Terrain" );
        m_resolution = m_terrain.terrainData.heightmapResolution;
        m_heightValues = new float[ m_resolution, m_resolution ];
        LoadHeightmap( "Clouds_more" );
    }

    private void LoadHeightmap( string filename )
    {
        // Load heightmap.
        Texture2D heightmap = Resources.Load( "Heightmaps/" + filename ) as Texture2D;

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
            m_terrain.terrainData.SetHeights( 0, 0, m_heightValues ); 
        }
        else
        {
            Debug.Log("File is NUll " + filename);
        }


    }

    // Member variables.
    //----------------------------------------------------------------------------------------------
    private Terrain    m_terrain      = null;
    private float[ , ] m_heightValues = null;
    private int        m_resolution   = 0;

    public string m_heightmap;
}