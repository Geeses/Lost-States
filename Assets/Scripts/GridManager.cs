using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> children;

    public GameObject [,] tileArray;

    void Awake()
    {
        Grid grid = gameObject.GetComponentInParent(typeof(Grid)) as Grid;
        float cellSizeX = grid.cellSize.x;
        float cellSizeZ = grid.cellSize.z;

        tileArray = CreateTileArray(cellSizeX, cellSizeZ);
    }


    GameObject[,] CreateTileArray(float cellSizeX_, float cellSizeZ_)
    {
        // initialize min and max values for x and z dimensions, setting them by comparing to transforms of children
        float xmin = Mathf.Infinity;
        float xmax = Mathf.NegativeInfinity;
        float zmin = Mathf.Infinity;
        float zmax = Mathf.NegativeInfinity;

        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);

            if (child.transform.position.x < xmin)
            {
                xmin = child.transform.position.x;
            }
            else if (child.transform.position.x > xmax)
            {
                xmax = child.transform.position.x;
            }

            if (child.transform.position.z < zmin)
            {
                zmin = child.transform.position.z;
            }
            else if (child.transform.position.z > zmax)
            {
                zmax = child.transform.position.z;
            }

            print(child + " at " + "x:" + child.transform.position.x.ToString() + "z:" + child.transform.position.z.ToString());  // debug   
        }

        // calculate rows in x and z dimension
        float xrows = (Mathf.Abs(xmin - xmax) / cellSizeX_) + 1;
        float zrows = (Mathf.Abs(zmin - zmax) / cellSizeZ_) + 1;


        GameObject [,] array = new GameObject[(int)xrows, (int)zrows];

        /* put every element from the GameObject List from above in the right spot of the array by taking
           the top left most tile of all tiles (the one that would be if its not a rectangle) as starting
           point and iterate trough the tile-area (boundaries were determined above) placing a tile at
           array[x,z] if its transform matches the searched one (calculated by checkvector) for the specific 
           field. also has a check for cells that have multiple tiles in them */ 
        for (int z = 0; z <= (int)zrows; z++)
        {
            for (int x = 0; x <= (int)xrows; x++) 
            {
                Vector3 checkvector = new Vector3(xmin + (x * cellSizeX_), 0f, zmax - (z * cellSizeZ_));

                foreach (GameObject child in children)
                {
                    if(child.transform.position == checkvector)
                    {
                        if (array[x,z] != null)
                        {  
                            throw new System.Exception(string.Format("Position {0} in the grid has multiple assigned tiles! (It may only have one per cell)", checkvector));
                        }
                        array[x,z] = child;
                    } 
                }
            }          
        }

        // debug-prints to check if all elements are on the correct array-positions
        for (int i = 0; i < zrows; i++)
        {
            for (int j = 0; j < xrows; j++)
            {
                if(array[j,i] == null)
                {
                    print(j.ToString() + "||" + i.ToString() + " " + "null");
                }
                else
                {
                    print(j.ToString() + "||" + i.ToString() + " " + array[j,i].transform.position.ToString("F2"));
                }
            }
        }

        return array;

    }
}
