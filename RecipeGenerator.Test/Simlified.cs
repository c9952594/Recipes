using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class Simplified {
    [Test]
    public void Runner() {
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
            " L",
            "M"
        };

        // B,C,A,A
        // D,A,A,A
        // F,E,A,A
        // H,G,E,A
        // I,A,A,A
        // K,J,J,J
        // L,J,J,J
        // M,M,M,M

        // A,A,B,C
        // A,A,A,D
        // A,A,E,F
        // A,E,G,H
        // A,A,A,I
        // J,J,J,K
        // J,J,J,L
        // M,M,M,M

        var paths = Paths(lines);
        var columns = paths.Max(path => path.Length);
        var grid = Grid(paths, columns);
    }

    public string[][] Grid(string[][] paths, int columns) {
        return _().Select(path => path.ToArray()).ToArray();

        IEnumerable<IEnumerable<string>> _() {
            foreach (var path in paths)
            {
                if (path.Length == 1) {
                    yield return Enumerable.Repeat(path[0], columns);
                    continue;
                }

                var additionalSteps = Enumerable.Repeat(path[path.Length - 2], columns - path.Length);
                var listPath = path.ToList();
                listPath.InsertRange(path.Length - 2, additionalSteps);
                yield return listPath;
            }
        }
    }

    public string[][] Paths(IEnumerable<string> lines) {
        return _().Select(path => path.ToArray()).ToArray();

        IEnumerable<IEnumerable<string>> _() {
            Stack<(int index, string value)> steps = new Stack<(int, string)>();

            foreach (var line in lines) {
                var trimmed = line.TrimStart();
                var index = line.Length - trimmed.Length;
                var value = trimmed.TrimEnd();

                if (steps.Count > 0 && index <= steps.Peek().index) {
                    yield return steps.Select(step => step.value);

                    while(steps.Count > 0 && index <= steps.Peek().index) {
                        steps.Pop();
                    }
                }

                steps.Push((index, value));
            }

            yield return steps.Select(step => step.value);
        }
        
    }
}