<!--
  Copyright (c) 2020 Eliah Kagan

  Permission to use, copy, modify, and/or distribute this software for any
  purpose with or without fee is hereby granted.

  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
  REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY
  AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
  INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM
  LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
  OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
  PERFORMANCE OF THIS SOFTWARE.
-->

# Graph - C# local functions example: graph DFS

This is an example of local functions in C#, showing a use in implementing
depth-first traversal of a directed graph.

## Contents

`GraphBasic/Program.cs` (or `graph-basic.linq`) is the clearer example.

`Graph/Program.cs` (or `graph.linq`) shows it with a `Graph` class.

## Notes

If one implements `VerticesReachableFrom` as part of one&rsquo;s `Graph` class,
one could still benefit from using a local function to capture the visitation
list and results sink. But I figured the example was more compelling this way.

(Also, there is a design argument for implementing `VerticesReachableFrom` in
such a way that a bug in it cannot cause any invariant of `Graph` to be
violated.)

## License

The contents of this repository were written in 2020 by Eliah Kagan (with a
minor documentation update in 2021). They are offered under
[**0BSD**](https://spdx.org/licenses/0BSD.html), a [&ldquo;public-domain
equivalent&rdquo;](https://en.wikipedia.org/wiki/Public-domain-equivalent_license)
license. See [`LICENSE`](LICENSE).

## See also

`GraphBasic/Program.cs` and `Graph/Program.cs` may also be viewed in the gist
[C# local functions
example](https://gist.github.com/EliahKagan/ca7277a9a20e5631af7ee6a222ecd443),
where they are named `GraphBasic.cs` and `Graph.cs`, respectively.
