<Query Kind="Program">
  <Namespace>System.Collections.ObjectModel</Namespace>
</Query>

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

/// <summary>Adjacency list representation of a fixed-order digraph.</summary>
/// <remarks>
/// <para>
/// The graph never gains or loses vertices but can gain (but not lose) edges.
/// </para>
/// <para>
/// Bad input should throw an exception, but the exception may not be very
/// illuminating. If this is used as a library, error reporting should be
/// improved, either by checking for errors explicitly or by catching
/// exceptions and throwing clearer ones (perhaps with the caught exceptions
/// wrapped as inner exceptions).
/// </para>
/// </remarks>
internal sealed class Graph : IEnumerable<Graph.Edge> {
    /// <summary>An (unweighted) edge in a directed graph.</summary>
    /// <remarks>Can be an edge between separate vertices or a loop.</remarks>
    internal readonly struct Edge : IEquatable<Edge> {
        /// <summary>Check if directed edges are equal.</summary>
        /// <param name="lhs">The first edge to compare for equality.</param>
        /// <param name="rhs">The second edge to compare for equality.</param>
        /// <returns>
        /// True if the source vertices are equal and the destination vertices
        /// are equal. False otherwise.
        /// </returns>
        public static bool operator==(Edge lhs, Edge rhs)
            => (lhs.Src, lhs.Dest) == (rhs.Src, rhs.Dest);

        /// <summary>Check if directed edges are unequal.</summary>
        /// <param name="lhs">The first edge to compare.</param>
        /// <param name="rhs">The second edge to compare.</param>
        /// <returns>False if <c>lhs == rhs</c>. True otherwise.</returns>
        /// <remarks>Calls and negates <see cref="operator=="/>.</remarks>
        public static bool operator!=(Edge lhs, Edge rhs) => !(lhs == rhs);

        /// <summary>
        /// Constructs a directed edge from its incident vertices.
        /// </summary>
        /// <param name="src">
        /// The source vertex, from which this is an out-edge.
        /// </param>
        /// <param name="dest">
        /// The destination vertex, to which this is an in-edge.
        /// </param>
        /// <returns>The edge from <c>src</c> to <c>dest</c>.</returns>
        internal Edge(int src, int dest) => (Src, Dest) = (src, dest);

        /// <summary>
        /// Deconstructs a directed edge into its incident vertices.
        /// </summary>
        /// <param name="src">
        /// Receives the source vertex, from which this is an out-edge.
        /// </param>
        /// <param name="dest">
        /// Receives the destination vertex, to which this is an in-edge.
        /// </param>
        internal void Deconstruct(out int src, out int dest)
            => (src, dest) = (Src, Dest);

        /// <summary>Compares this edge to another, for equality.</summary>
        /// <param name="rhs">The other vertex, to comapre this to.</param>
        /// <returns>
        /// True iff <c>this</c> and <c>other</c> are the same edge.
        /// </returns>
        /// <remarks>Calls <see cref="operator=="/>.</remarks>
        public bool Equals(Edge rhs) => this == rhs;

        /// <summary>
        /// Compares this edge to an arbitrary object or boxed value-type
        /// instance (or null).
        /// </summary>
        /// <param name="obj">The thing to compare this to.</param>
        /// <returns>True off obj is an edge equal to this one.</returns>
        /// <remarks>Uses <see cref="operator=="/>.</remarks>
        public override bool Equals(object? obj)
            => obj is Edge rhs && this == rhs;

        /// <summary>Computes a prehash used by hash-based containers.</summary>
        /// <returns>The computed prehash ("hash code").</returns>
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

        /// <summary>Gets this edge as human-readable text.</summary>
        /// <returns>A text ordered-pair representation of this edge.</returns>
        /// <remarks>
        /// Uses <c>(</c> <c>)</c> (not <c>{</c> <c>}</c>) as the edge is
        /// directed.
        /// </remarks>
        public override string ToString() => $"({Src}, {Dest})";

        /// <summary>
        /// The source vertex, from which this is an out-edge.
        /// </summary>
        internal int Src { get; }

        /// <summary>
        /// The destination vertex, to which this is an in-edge.
        /// </summary>
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

    /// <summary>Enumerates the edges in this graph.</summary>
    /// <returns>An enumerator that yields each directed edge.</returns>
    /// <remarks>
    /// This graph uses an adjacency-list representation, but when enumerated
    /// is treated as a collection of edges rather than a collection of rows.
    /// </remarks>
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
    /// <summary>Finds vertices reachable from a given start vertex.</summary>
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
    /// <summary>
    /// Test <see cref="GraphExtensions.VerticesReachableFrom"/> from each
    /// vertex in the graph.
    /// </summary>
    /// <remarks>
    /// If the goal were to produce all this information, rather than to test
    /// that function, then this algorithm would be needlessly inefficient.
    /// </remarks>
    private static void Test(Graph graph)
    {
        foreach (var start in Enumerable.Range(0, graph.Order))
            graph.VerticesReachableFrom(start).Dump($"Reachable from {start}");
    }

    /// <summary>
    /// Makes a small graph, shows its contents, and shows reachability from
    /// each of its vertices.
    /// </summary>
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

        Test(graph);
    }
}
