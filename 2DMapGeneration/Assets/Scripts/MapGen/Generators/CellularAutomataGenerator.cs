using UnityEngine;
using System.Collections.Generic;
using System;

public class CellularAutomataGenerator : MonoBehaviour, IMountainGenerator
{
    private CellularAutomataGeneratorArgs args;

    public void Initialize(IMountainGeneratorArgs args)
    {
        this.args = (CellularAutomataGeneratorArgs)args;
    }

    public MountainGeneratorResult Generate(BiomeData biome)
    {
        Rect bounds = OutlineUtils.GetBoundingRect(biome.outline);

        int width = (int)(bounds.xMax - bounds.xMin);
        int height = (int)(bounds.yMax - bounds.yMin);

        int[,] elevationMap = new int[width, height];

        for(int i=0; i<width; i++)
        {
            for(int j=0; j<height; j++)
            {
                elevationMap[i,j] = -1;
            }
        }


        for(int h=0; h<=3; h++)
        {
            CellularAutomata aut = new CellularAutomata(0,width-1,height-1,0,3);

            for(int i=0; i<width; i++)
            {
                for(int j=0; j<height; j++)
                {
                    if(elevationMap[i,j] != h-1)
                    {
                        aut.SetCell(i,j,2);
                    }
                    else
                    {
                        aut.SetCell(i,j, UnityEngine.Random.value < args.elevationProbability ? 1 : 0);
                    }
                }
            }

            //aut.Randomize();

            aut.SetTransitionRule((grid, x, y) =>
            {
                if(grid[x,y] == 2)
                {
                    return 2;
                }

                int width = grid.GetLength(0);
                int height = grid.GetLength(1);

                int sum = 0;
                int count = 0;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;
                        if (nx >= 0 && nx < width && ny >= 0 && ny < height && (nx!=x || ny!=y))
                        {
                            if(grid[nx,ny]!=2)
                            {
                                sum += grid[nx, ny];
                                count++;   
                            }
                            else if(args.enforceSmoothedEdges)
                            {
                                return 0;
                            }
                        }
                    }
                }

                int newValue;

                if(grid[x,y] == 1)
                {
                    newValue = (sum >= 4) ? 1 : 0;
                }
                else
                {
                    newValue = (sum >= 5) ? 1 : 0;
                }

                return newValue;
            });

            aut.ApplyTransition(args.automataIterations);

            for(int i=0; i<width; i++)
            {
                for(int j=0; j<height; j++)
                {
                    if(aut.GetCell(i,j) == 1)
                    {
                        elevationMap[i,j]=h;
                    }
                }
            }
        }


        MountainGeneratorResult mgr = new MountainGeneratorResult
        {
            elevationMap = elevationMap,
            startX = Mathf.FloorToInt(bounds.xMin),
            startY = Mathf.FloorToInt(bounds.yMin),
        };

        return mgr;
    }
}