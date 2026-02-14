public class DSU
{
    private int[] parent;
    private int[] rank;

    public DSU(int size)
    {
        parent = new int[size];
        rank = new int[size];

        for (int i = 0; i < size; i++)
        {
            parent[i] = i;
            rank[i] = 0;
        }
    }

    public int Find(int x)
    {
        if (parent[x] != x)
            parent[x] = Find(parent[x]);

        return parent[x];
    }

    public bool Union(int x, int y)
    {
        int rootX = Find(x);
        int rootY = Find(y);

        if(rootX == rootY)
        {
            return false;
        }

        if(rank[rootX] < rank[rootY])
        {
            parent[rootX] = rootY;
        }
        else if(rank[rootX] > rank[rootY])
        {
            parent[rootY] = rootX;
        }
        else
        {
            parent[rootY] = rootX;
            rank[rootX]++;
        }

        return true;
    }

    public bool Connected(int x, int y)
    {
        return Find(x) == Find(y);
    }
}