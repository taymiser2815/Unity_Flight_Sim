using UnityEngine;
using System.Collections;
using System.IO;


public class CreatePlayer : MonoBehaviour
{
    private void Start(){
        //get all terrain in unity
        Terrain[] terrain = FindObjectsOfType(typeof(Terrain)) as Terrain[];
        //size of terrain
        int gridsize = (int) Mathf.Sqrt(terrain.Length);
        Debug.Log("Gridsize: "+gridsize.ToString());
        //get the amount of units terrain covers
        int terrain_size = (int)terrain[0].terrainData.size.x;
        //the total length and with of the world
        int total_world_size = gridsize * terrain_size;
        Debug.Log(total_world_size.ToString());
        Vector3 vecRet = new Vector3();
        vecRet.x = ((total_world_size/2) - ((int)((total_world_size/2)/terrain_size)*terrain_size));
        vecRet.y = 0;
        vecRet.z = ((total_world_size/2) - ((int)((total_world_size/2)/terrain_size)*terrain_size));
        Debug.Log("Tex Coord: " + vecRet.x.ToString());
        GameObject currentTerrain_go = GameObject.Find(((int)(gridsize/2)).ToString()+"_"+((int)(gridsize/2)).ToString());
        Debug.Log(((int)(gridsize/2)).ToString()+"_"+((int)(gridsize/2)).ToString());
        Terrain currentTerrain = currentTerrain_go.GetComponent<Terrain>();
        float terrain_height = currentTerrain.terrainData.GetHeight((int)vecRet.x,(int)vecRet.z);
        //set orientation
        Quaternion playerRotation = Quaternion.identity;
        //get the center position
        Vector3 playerPosistion = new Vector3(total_world_size/2,terrain_height,total_world_size/2);
        //create object
        var go = Instantiate(g_playerPrefab, playerPosistion, playerRotation);
        //name the object to player
        go.name = "Player";
        

    }

    public Transform g_playerPrefab;

}