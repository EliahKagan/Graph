<Query Kind="Program">
  <Namespace>System.Collections.ObjectModel</Namespace>
</Query>

/// <summary>Adjacency list representation of a fixed-order digraph.</summary>
/// <remarks>
/// The graph never gains or loses vertices but can gain (but not lose) edges.
/// </remarks>
internal sealed class Graph : IEnumerable<Graph.Edge> {
    /// <summary>An (unweighted) edge in a directed graph.</summary>
    /// <remarks>Can be an edge between separate vertices or a loop.</remarks>
    internal readonly struct Edge : IEquatable<Edge> {
        public static bool operator==(Edge lhs, Edge rhs)
            => (lhs.Src, lhs.Dest) == (rhs.Src, rhs.Dest);
        
        public static bool operator!=(Edge lhs, Edge rhs) => !(lhs == rhs);
    
        internal Edge(int src, int dest) => (Src, Dest) = (src, dest);
        
        internal void Deconstruct(out int src, out int dest)
            => (src, dest) = (Src, Dest);
        
        public bool Equals(Edge rhs) => this == rhs;
        
        public override bool Equals(object? obj)
            => obj is Edge rhs && this == rhs;
        
        public override int GetHashCode()
        {
            const int seed = 17;
            const int multiplier = 8191;
            
            unchecked {
                var result = seed;
                result = result * multiplier + Src;
                result = result * multiplier + Dest;
                return result;
            }
        }
        
        public override string ToString() => $"({Src}, {Dest})";
    
        internal int Src { get; }
        
        internal int Dest { get; }
    }

    /// <summary>Constructs an graph that initially has no edges.</summary>
    /// <param name="order">The number of vertices in the graph.</param>
    internal Graph(int order)
        => _adj = Enumerable.Range(0, order)
                            .Select(_ => new List<int>())
                            .ToArray();
    
    /// <summary>The number of vertices in the graph.</summary>
    /// <remarks>This remains the same through the graph's lifetime.</remarks>
    internal int Order => _adj.Length;
    
    /// <summary>Adds a directed edge to the graph.</summary>
    /// <param name="edge">The edge to add to the graph.</param>
    /// <remarks>Permits parallel edges (and loops).</remarks>
    internal void Add(Edge edge) => Add(edge.Src, edge.Dest);
    
    /// <summary>Adds a directed edge to the graph.</summary>
    /// <param name="edge">A 2-tuple of source and destination vertices.</param>
    /// <remarks>Permits parallel edges and loops.</remarks>
    internal void Add((int src, int dest) edge) => Add(edge.src, edge.dest);
    
    /// <summary>Adds an directed edge to the graph.</summary>
    /// <param name="src">The source vertex this is an out-edge from.</param>
    /// <param name="dest">The destination vertex this is an in-edge to.</param>
    /// <remarks>Permits parallel edges and loops.</remarks>
    internal void Add(int src, int dest)
    {
        _ = _adj[dest]; // Throw if dest is out of range.
        _adj[src].Add(dest);
    }
    
    /// <summary>Enumerates a vertex's outgoing neighbors.</summary>
    /// <param name="src">The vertex to look from.</param>
    /// <returns>The destination vertices of <c>src</c>'s out-edges.</returns>
    internal ReadOnlyCollection<int> this[int src] => _adj[src].AsReadOnly();
    
    public IEnumerator<Edge> GetEnumerator()
    {
        foreach (var src in Enumerable.Range(0, Order)) {
            foreach (var dest in _adj[src])
                yield return new Edge(src, dest);
        }
    }
    
    System.Collections.IEnumerator
    System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly List<int>[] _adj;
}

/// <summary>Extension methods providing functionality for Graph.</summary>
/// <remarks>
/// These could be implemented in Graph itself. But they don't participate in
/// enforcing Graph's invariants, so having them as members would weaken
/// encapsulation unnecessarily.
/// </remarks>
internal static class GraphExtensions {
    /// <summary>Find vertices reachable from a given start vertex.</summary>
    /// <param name="self">The graph to search in.</param>
    /// <param name="start">The vertex to search from.</param>
    /// <returns>The vertices reachable from start (including itself).</returns>
    internal static IList<int> VerticesReachableFrom(this Graph self, int start)
    {
        var visited = new BitArray(self.Order);
        var result = new List<int>();
        
        void Dfs(int src)
        {
            if (visited[src]) return;
            
            visited[src] = true;
            result.Add(src);
            foreach (var dest in self[src]) Dfs(dest);
        }
        
        Dfs(start);
        return result;
    }
}

/// <summary>Tests some functionality in Graph and GraphExtensions.</summary>
internal static class UnitTest {
    private static void Main()
    {
        var graph = new Graph(10) {
            { 0, 2 },
            { 1, 3 },
            { 2, 4 },
            { 3, 7 },
            { 5, 4 },
            { 9, 8 },
            { 4, 3 },
            { 5, 9 },
            { 8, 4 },
            { 6, 8 },
            { 0, 6 },
            { 1, 0 },
            { 8, 1 },
        };
        
        graph.Dump(nameof(graph));
        
        const int start = 2;
        graph.VerticesReachableFrom(start).Dump($"reachable from {start}");
    }
}
