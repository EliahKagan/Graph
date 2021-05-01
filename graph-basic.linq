<Query Kind="Program" />

// Copyright (c) 2020 Eliah Kagan
//
// Permission to use, copy, modify, and/or distribute this software for any
// purpose with or without fee is hereby granted.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
// SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION
// OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN
// CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

internal static class Program {
    private static void AddEdge(this IList<int>[] adj, int src, int dest)
    {
        _ = adj[dest]; // Throw if dest is out of range.
        adj[src].Add(dest);
    }

    private static IList<int>[]
    MakeAdjacencyList(int order, params (int src, int dest)[] edges)
    {
        // Create an adjacency list for a graph with all isolated vertices.
        var adj = new IList<int>[order];
        for (var i = 0; i < order; ++i) adj[i] = new List<int>();

        // Populate adjacency list with the specified edges (if any).
        foreach (var (src, dest) in edges) adj.AddEdge(src, dest);
        return adj;
    }

    private static IList<int> VerticesReachableFrom(this IList<int>[] adj,
                                                    int start)
    {
        var result = new List<int>();
        var visited = new BitArray(adj.Length);

        void Dfs(int src)
        {
            if (visited[src]) return;

            visited[src] = true;
            result.Add(src);
            foreach (var dest in adj[src]) Dfs(dest);
        }

        Dfs(start);
        return result;
    }

    private static void Test(IList<int>[] adj)
    {
        for (var start = 0; start < adj.Length; ++start)
            adj.VerticesReachableFrom(start).Dump($"Reachable from {start}");
    }

    private static void Main()
    {
        var adj = MakeAdjacencyList(10, (0, 2),
                                        (1, 3),
                                        (2, 4),
                                        (3, 7),
                                        (5, 4),
                                        (9, 8),
                                        (4, 3),
                                        (5, 9),
                                        (8, 4),
                                        (6, 8),
                                        (0, 6),
                                        (1, 0),
                                        (8, 1));

        adj.Dump(nameof(adj));

        Test(adj);
    }
}
