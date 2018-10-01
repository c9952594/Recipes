using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;

namespace Tests {
 
    public class Node {
        public readonly int Index;
        public readonly string Value;
        public readonly Node Parent;
        public readonly List<Node> Children;

        Node(int index, string value, Node parent)
        {
            this.Index = index;
            this.Value = value;
            this.Parent = parent;
            this.Children = new List<Node>();
        }

        public int Colspan { get; private set; } = 1;

        public int Rowspan() {
            return Children.Sum(child => child.Rowspan()) + (Children.Count == 0 ? 1 : 0);
        }

        public static int maxPathSize = 0;

        public static Node FromList(IEnumerable<string> lines) {
            var currentSize = 0;
            var root = new Node(-1, null, null);
            var current = root;
            foreach (var line in lines)
            {
                var trimmed = line.TrimStart();
                var index = line.Length - trimmed.Length;
                var value = trimmed.TrimEnd();

                while (index <= current.Index) {
                    currentSize -= 1;
                    current = current.Parent;
                }

                var child = new Node(index, value, current);
                current.Children.Add(child);
                current = child;
                currentSize += 1;
                if (maxPathSize < currentSize) maxPathSize = currentSize;
            }
            return root;
        } 

        public override string ToString() {
            var builder = new StringBuilder();
            if (Index >= 0) {
                builder.Append($"{Value}({Index})-");
            }
            foreach (var child in Children)
            {
                builder.Append(child.ToString());
            }
            return builder.ToString();
        }

        public IEnumerable<List<Node>> Paths() {
            if (Children.Count == 0) yield return new List<Node>() { this };
            foreach (var child in Children)
            {
                foreach (var path in child.Paths())
                {
                    if (Index >= 0) {
                        path.Add(this);
                    }
                    yield return ResizeTo(path, maxPathSize);
                }
            }
        }

        public static void ResizeTo(int size) {
            if (Children.Count == 1) {
                Colspan = size;
            }
            if (Children.Count == 2) {
                Children.Insert(1, new Node(Index, "", this));
            }
            Children[1].Colspan = size - Children.Count;

            foreach (var child in Children)
            {
                child.ResizeTo(size);
            }
        }
    }

    public class RecipeTest {
        [Test]
        public void Resizing() {
             var root = Node.FromList(new [] {
                "A",
                " B",
                "  C",
                " D",
                " E",
                "  F",
                "  G",
                "   H",
                " I"
            });

            var a = root.Children[0];
            var b = a.Children[0];
            var c = b.Children[0];
            var d = a.Children[1];
            var e = a.Children[2];
            var f = e.Children[0];
            var g = e.Children[1];
            var h = g.Children[0];
            var i = a.Children[3];
            
            IEnumerable<List<Node>> paths = root.Paths();
            var maximumSize = paths.Max(path => path.Count);

            maximumSize.ShouldBe(4);
        }


        [Test]
        public void RowSpan() {
             var root = Node.FromList(new [] {
                "A",
                " B",
                "  C",
                " D",
                " E",
                "  F",
                "  G",
                "   H",
                " I"
            });

            var a = root.Children[0];
            var b = a.Children[0];
            var c = b.Children[0];
            var d = a.Children[1];
            var e = a.Children[2];
            var f = e.Children[0];
            var g = e.Children[1];
            var h = g.Children[0];
            var i = a.Children[3];

            a.Rowspan().ShouldBe(5);
            b.Rowspan().ShouldBe(1);
            c.Rowspan().ShouldBe(1);
            d.Rowspan().ShouldBe(1);
            e.Rowspan().ShouldBe(2);
            f.Rowspan().ShouldBe(1);
            g.Rowspan().ShouldBe(1);
            h.Rowspan().ShouldBe(1);
            i.Rowspan().ShouldBe(1);
        }

        [Test]
        public void Pathing() {
             var root = Node.FromList(new [] {
                "A",
                " B",
                "  C",
                " D",
                " E",
                "  F",
                "  G",
                "   H",
                " I"
            });

            var a = root.Children[0];
            var b = a.Children[0];
            var c = b.Children[0];
            var d = a.Children[1];
            var e = a.Children[2];
            var f = e.Children[0];
            var g = e.Children[1];
            var h = g.Children[0];
            var i = a.Children[3];
            
            List<Node>[] paths = root.Paths().ToArray();

            paths[0][0].ShouldBe(c);
            paths[0][1].ShouldBe(b);
            paths[0][2].ShouldBe(a);

            paths[1][0].ShouldBe(d);
            paths[1][1].ShouldBe(a);

            paths[2][0].ShouldBe(f);
            paths[2][1].ShouldBe(e);
            paths[2][2].ShouldBe(a);

            paths[3][0].ShouldBe(h);
            paths[3][1].ShouldBe(g);
            paths[3][2].ShouldBe(e);
            paths[3][3].ShouldBe(a);

            paths[4][0].ShouldBe(i);
            paths[4][1].ShouldBe(a);
        }

        [Test]
        public void ShouldCreateATreeOfTheInputRecipeList() {
            var root = Node.FromList(new [] {
                "A",
                " B",
                "  C",
                "  D",
                " E",
                "  F",
                "  G",
            });
            
            var built = root.ToString();

            Node a = root.Children.First();
            a.Index.ShouldBe(0);
            a.Value.ShouldBe("A");
            a.Children.Count.ShouldBe(2);

                Node b = a.Children.First();
                b.Index.ShouldBe(1);
                b.Value.ShouldBe("B");
                b.Children.Count.ShouldBe(2);

                    Node c = b.Children.First();
                    c.Index.ShouldBe(2);
                    c.Value.ShouldBe("C");
                    c.Children.Count.ShouldBe(0);

                    Node d = b.Children.ToArray()[1];
                    d.Index.ShouldBe(2);
                    d.Value.ShouldBe("D");
                    d.Children.Count.ShouldBe(0);

                Node e = a.Children.ToArray()[1];
                e.Index.ShouldBe(1);
                e.Value.ShouldBe("E");
                e.Children.Count.ShouldBe(2);

                    Node f = e.Children.First();
                    f.Index.ShouldBe(2);
                    f.Value.ShouldBe("F");
                    f.Children.Count.ShouldBe(0);

                    Node g = e.Children.ToArray()[1];
                    g.Index.ShouldBe(2);
                    g.Value.ShouldBe("G");
                    g.Children.Count.ShouldBe(0);
        }
    }
}