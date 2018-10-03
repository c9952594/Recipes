using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;

namespace Tests {
    public class Recipe {
        public readonly List<Tree> Trees;

        public Recipe() {
            Trees = new List<Tree>();
        }

        public static Recipe FromRecipe(IEnumerable<string> lines) {
            var recipe = new Recipe();

            Node currentNode = null;
            
            foreach (var line in lines)
            {
                var trimmed = line.TrimStart();
                var index = line.Length - trimmed.Length;
                var value = trimmed.TrimEnd();

                while (index <= currentNode?.Index) {
                    currentNode = currentNode.Parent;
                }

                if (currentNode == null) {
                    currentNode = new Node(index, value, currentNode);

                    var tree = new Tree(currentNode);
                    recipe.Trees.Add(tree);

                    continue;
                }

                var child = new Node(index, value, currentNode);
                currentNode.Children.Add(child);
                currentNode = child;
            }

            return recipe;
        }
    }

    public class Tree {
        public readonly Node RootNode;

        public Tree(Node node) => RootNode = node;

        public override string ToString() => $"{ColumnSpan()}";

        public int ColumnSpan() {
            return Internal(RootNode, 1);

            int Internal(Node node, int column) {
                if (node.Children.Count == 0) return column;
                return node.Children.Max(childNode => Internal(childNode, column + 1));
            }
        }
        
        public IEnumerable<List<Node>> Paths() {
            return Internal(RootNode);

            IEnumerable<List<Node>> Internal(Node node) {
                if (node.Children.Count == 0)
                    yield return new List<Node>() { node };
                foreach (var child in node.Children) {
                    foreach (var path in Internal(child)) {
                        path.Add(node);
                        yield return path;
                    }
                }
            }
        } 
     
    }

    public class Node {
        public readonly int Index;
        public readonly string Value;
        public readonly Node Parent;
        public readonly List<Node> Children;

        public Node(int index, string value, Node parent = null, List<Node> children = null)
        {
            Index = index;
            Value = value;
            Parent = parent;
            Children = children ?? new List<Node>();
        }

        public override string ToString() {
            var builder = $"{Value}({Index},{Rowspan()}) ";
            
            foreach (var child in Children)
            {
                builder = builder + child.ToString();
            }

            return builder;
        }

        public int Rowspan() {
            return Children.Sum(child => child.Rowspan()) + (Children.Count == 0 ? 1 : 0);
        }
    }

    public class TreeTest {
        [Test]
        public void ShouldParseRecipeList() {
            var lines = new [] {
                "A",
                " B",
                "  C",
                " D",
                " E",
                "  F",
                "  G",
                "   H",
                " I",
                "J",
                " K",
                " L"
            };

            var recipe = Recipe.FromRecipe(lines);
            var paths = recipe.Trees.Select(child => child.Paths());
        }
    }
}