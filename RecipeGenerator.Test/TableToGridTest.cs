using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests
{
    public class TableToGridTest
    {
        [Test]
        public void Spike()
        {
            var lines = new[] {
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

            // A,B,C
            // A,D
            // A,E,F
            // A,E,G,H
            // A,I
            // J,K
            // J,L

            var linesWithIndex = lines.Select(line =>
            {
                var trimmed = line.TrimStart();
                return (index: line.Length - trimmed.Length, value: trimmed.TrimEnd());
            });

            Stack<(int index, string value)> steps = new Stack<(int, string)>();

            foreach (var current in linesWithIndex)
            {
                if (steps.Count == 0) {
                    steps.Push(current);
                    continue;
                }

                var parent = steps.Peek();

                if (current.index > parent.index) {
                    steps.Push(current);
                    continue;
                }

                if (current.index == parent.index) {
                    
                }
            }

            var grid = ParseList(lines).Select(list => list.ToList()).ToList();
            var maxSpan = grid.Max(row => row.Count);
            var normalised = grid.Select(row =>
            {
                if (row.Count == 1)
                {
                    return Enumerable.Repeat(row.First(), maxSpan);
                }
                if (row.Count == 2)
                {
                    row.AddRange(Enumerable.Repeat(row.Last(), maxSpan - 2));
                    return row;
                }
                row.InsertRange(row.Count - 2, Enumerable.Repeat(row[row.Count - 2], maxSpan - row.Count));
                return row;
            }).Select(list => list.ToArray()).ToArray(); ;

            var builder = new StringBuilder();
            for (int rowIndex = 0; rowIndex < normalised.Length; rowIndex++)
            {
                builder.AppendLine("<tr>");
                var row = normalised[rowIndex];
                for (int columnIndex = 0; columnIndex < row.Length; columnIndex++)
                {
                    var showTopBorder = false;
                    var showBottomBorder = false;
                    var showLeftBorder = false;
                    var showRightBorder = false;

                    if (rowIndex == 0) showTopBorder = true;
                    if (rowIndex == normalised.Length - 1) showBottomBorder = true;
                    if (columnIndex == 0) showLeftBorder = true;
                    if (columnIndex == row.Length - 1) showRightBorder = true;

                    
                    if (showTopBorder == false && row[columnIndex] != normalised[rowIndex - 1][columnIndex])
                        showTopBorder = true;
                    if (showBottomBorder == false && row[columnIndex] != normalised[rowIndex + 1][columnIndex])
                        showBottomBorder = true;
                    if (showLeftBorder == false && row[columnIndex] != normalised[rowIndex][columnIndex - 1])
                        showLeftBorder = true;
                    if (showRightBorder == false && row[columnIndex] != normalised[rowIndex][columnIndex + 1])
                        showRightBorder = true;

                    var left = showLeftBorder ? "" : "border-left: 0 solid;";
                    var right = showRightBorder ? "" : "border-right: 0 solid;";
                    var top = showTopBorder ? "" : "border-top: 0 solid;";
                    var bottom = showBottomBorder ? "" : "border-bottom: 0 solid;";

                    var style = $"{left}{right}{top}{bottom}";

                    builder.AppendLine($@"<td style=""{style}"">");
                    builder.AppendLine(normalised[rowIndex][columnIndex]);
                    builder.AppendLine("</td>");
                }
                builder.AppendLine("</tr>");
            }    
            var table = builder.ToString();
        }

        public IEnumerable<IEnumerable<string>> ParseList(IEnumerable<string> lines)
        {
            var linesWithIndex = lines.Select(line =>
            {
                var trimmed = line.TrimStart();
                var index = line.Length - trimmed.Length;
                var value = trimmed.TrimEnd();
                return (index, value);
            });

            Stack<ImmutableStack<(int index, string value)>> paths = new Stack<ImmutableStack<(int, string)>>();

            foreach (var currentNode in linesWithIndex)
            {
                if (paths.Count == 0)
                {
                    var root = ImmutableStack.Create(currentNode);
                    paths.Push(root);
                    continue;
                }

                var parentPath = paths.Peek();
                var parentNode = parentPath.Peek();

                if (parentNode.index < currentNode.index)
                {
                    var currentPath = parentPath.Push(currentNode);
                    paths.Push(currentPath);
                    continue;
                }

                var leaf = paths.Pop();
                var unwound = leaf.Select(node => node.value);
                yield return unwound;

                while (paths.Count > 0 && paths.Peek().Peek().index >= currentNode.index)
                {
                    paths.Pop();
                }

                if (paths.Count > 0)
                {
                    paths.Push(paths.Peek().Push(currentNode));
                }
                else
                {
                    var root = ImmutableStack.Create(currentNode);
                    paths.Push(root);
                }
            }

            var finalLeaf = paths.Pop();
            yield return finalLeaf.Select(node => node.value);
        }
    }
}