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

        public static Node FromList(IEnumerable<string> lines) {
            var root = new Node(-1, null, null);
            var current = root;
            foreach (var line in lines)
            {
                var trimmed = line.TrimStart();
                var index = line.Length - trimmed.Length;
                var value = trimmed.TrimEnd();

                while (index <= current.Index) {
                    current = current.Parent;
                }

                var child = new Node(index, value, current);
                current.Children.Add(child);
                current = child;
            }
            return root;
        } 

        public override string ToString() {
            var builder = new StringBuilder();
            if (Index >= 0) {
                builder.AppendLine($"{"".PadLeft(Index, ' ')}{Value}({Index})");
            }
            foreach (var child in Children)
            {
                builder.Append(child.ToString());
            }
            return builder.ToString();
        }
    }

    public class RecipeTest {
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