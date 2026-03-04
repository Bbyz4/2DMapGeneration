using UnityEngine;
using System.Collections.Generic;
using System;

public class PriorityQueue
{
    public struct QueueElement
    {
        public Vector2Int Element;
        public int Priority;

        public QueueElement(Vector2Int element, int priority)
        {
            this.Element = element;
            this.Priority = priority;
        }
    }

    private List<QueueElement> queue;

    public PriorityQueue()
    {
        queue = new List<QueueElement>();
    }

    private void HeapifyUp(int i)
    {
        while(i > 0)
        {
            int parent = (i-1)/2;

            if(queue[i].Priority >= queue[parent].Priority)
            {
                break;
            }

            var temp = queue[i];
            queue[i] = queue[parent];
            queue[parent] = temp;

            i = parent;
        }
    }

    private void HeapifyDown(int i)
    {
        int leftChild = 2*i + 1;
        int rightChild = 2*i + 2;
        int smallestChild = i;

        if(leftChild < queue.Count && queue[leftChild].Priority < queue[smallestChild].Priority)
        {
            smallestChild = leftChild;
        }

        if(rightChild < queue.Count && queue[rightChild].Priority < queue[smallestChild].Priority)
        {
            smallestChild = rightChild;
        }

        if(smallestChild != i)
        {
            var temp = queue[smallestChild];
            queue[smallestChild] = queue[i];
            queue[i] = temp;

            HeapifyDown(smallestChild);
        }
    }

    public void Enqueue(Vector2Int Element, int priority)
    {
        queue.Add(new QueueElement(Element, priority));
        HeapifyUp(queue.Count - 1);
    }

    public Vector2Int Dequeue()
    {
        if(queue.Count == 0)
        {
            throw new Exception();
        }

        Vector2Int value = queue[0].Element;

        var temp = queue[0];
        queue[0] = queue[queue.Count - 1];
        queue[queue.Count - 1] = temp;

        queue.RemoveAt(queue.Count - 1);

        HeapifyDown(0);

        return value;
    }

    public bool IsEmpty()
    {
        return queue.Count == 0;
    }
}

public class WFCMountainGenerator : MonoBehaviour, IMountainGenerator
{
    private WFCMountainGeneratorArgs args;

    private Dictionary<int, float> heightProbabilities;

    public void Initialize(IMountainGeneratorArgs args)
    {
        this.args = (WFCMountainGeneratorArgs)args;

        heightProbabilities = new Dictionary<int, float>()
        {
          {-1, this.args.lakeProbability},
          {0, this.args.surfaceProbability},
          {1, this.args.mountain1Probability},
          {2, this.args.mountain2Probability},
          {3, this.args.mountain3Probability}  
        };
    }

    private List<Vector2Int> GetNeighbours(Vector2Int tile, int width, int height)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        if(tile.x - 1 >= 0)
        {
            result.Add(new Vector2Int(tile.x - 1, tile.y));
        }

        if(tile.x + 1 < width)
        {
            result.Add(new Vector2Int(tile.x + 1, tile.y));
        }

        if(tile.y - 1 >= 0)
        {
            result.Add(new Vector2Int(tile.x, tile.y - 1));
        }

        if(tile.y + 1 < height)
        {
            result.Add(new Vector2Int(tile.x, tile.y + 1));
        }

        return result;
    }

    private int[,] WFCGenerateMap(int width, int height)
    {
        Dictionary<Vector2Int, HashSet<int>> possibleFieldValues = new Dictionary<Vector2Int, HashSet<int>>();
        int[,] elevationMap = new int[width, height];

        PriorityQueue entropyQueue = new PriorityQueue();

        //1. At the start, each field can take any value
        for(int x=0; x<width; x++)
        {
            for(int y=0; y<height; y++)
            {
                possibleFieldValues[new Vector2Int(x,y)] = new HashSet<int>(){-1,0,1,2,3};
                elevationMap[x,y] = -2;
                entropyQueue.Enqueue(new Vector2Int(x,y), 5);
            }      
        }

        while(!entropyQueue.IsEmpty())
        {
            //2. Choose a tile with the lowest entropy
            Vector2Int tile = entropyQueue.Dequeue();

            if(elevationMap[tile.x, tile.y] != -2)
            {
                continue;
            }

            //2.5 If this tile has no possible neighbours, algorithm run failed
            if(possibleFieldValues[tile].Count == 0)
            {
                return null;
            }

            //3. Get information about neighbours
            Dictionary<int, int> neighbourCounter = new Dictionary<int, int>()
            {
              {-1,0},
              {0,0},
              {1,0},
              {2,0},
              {3,0}  
            };

            foreach(Vector2Int neigh in GetNeighbours(tile, width, height))
            {
                int neighHeight = elevationMap[neigh.x, neigh.y];

                if(neighHeight != -2)
                {
                    int temp = neighbourCounter[neighHeight];
                    neighbourCounter[neighHeight] = temp + 1;
                }
            }

            //4. Calculate each tile's probability
            Dictionary<int, float> probabilitiesCumsum = new Dictionary<int, float>();
            float probabilitySum = 0;

            for(int h=-1; h<=3; h++)
            {
                float thisTileProb = possibleFieldValues[tile].Contains(h) ? heightProbabilities[h] * MathF.Pow(MathF.E, neighbourCounter[h] * args.clusteringCoefficient) : 0f;

                probabilitySum += thisTileProb;
                probabilitiesCumsum[h] = probabilitySum;
            }

            //5. Choose a random tile
            float randomNum = UnityEngine.Random.Range(0f, probabilitySum);
            int decidedHeight = -1;

            for(int h=-1; h<3; h++)
            {
                if(probabilitiesCumsum[h] > randomNum)
                {
                    decidedHeight++;
                }
            }

            elevationMap[tile.x, tile.y] = decidedHeight;

            //6. Apply constraints to neighbours
            //IMPORTANT TODO: constraints should be propagated recursively to other tiles as well, not just direct neighbours
            foreach(Vector2Int neigh in GetNeighbours(tile, width, height))
            {
                int oldSize = possibleFieldValues[neigh].Count;
                possibleFieldValues[neigh].RemoveWhere(x => Math.Abs(x-decidedHeight) > 1);
            
                if(possibleFieldValues[neigh].Count != oldSize)
                {
                    entropyQueue.Enqueue(neigh, possibleFieldValues[neigh].Count);
                }
            }

        }

        return elevationMap;
    }

    public List<MountainData> Generate(BiomeData biome)
    {
        Rect bounds = OutlineUtils.GetBoundingRect(biome.outline);

        int width = (int)(bounds.xMax - bounds.xMin);
        int height = (int)(bounds.yMax - bounds.yMin);

        int[,] elevationMap = null;

        do
        {
            elevationMap = WFCGenerateMap(width, height);
            Debug.Log("Try");
        }
        while(elevationMap == null);

        List<MountainData> result = GeneratorUtils.BuildMountainOutlines(elevationMap, width, height, (int)bounds.xMin, (int)bounds.yMin);
    
        return result;
    }
}