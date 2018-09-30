using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RecipeGenerator;
using Shouldly;

namespace Tests
{
    [TestFixture]
    public class GeneratorTest
    {
        static IEnumerable<string> GetTestFiles() {
            var projectFolder = Path.Combine("..","..","..");
            string testDataFolder = Path.Combine(TestContext.CurrentContext.WorkDirectory, projectFolder, "TestData");
            return Directory.EnumerateFiles(testDataFolder, "*.input");
        }

        [Test, TestCaseSource(nameof(GetTestFiles))]
        public void GenerateRecipe(string inputPath)
        {
            var folder = Path.GetDirectoryName(inputPath);
            var filename = Path.GetFileNameWithoutExtension(inputPath);

            var expectedPath = Path.Combine(folder, $"{filename}.expected");
            var actualPath = Path.Combine(folder, $"{filename}.actual.html");

            var expectedRecipe = File.ReadAllText(expectedPath);

            var inputLines = File.ReadAllLines(inputPath);
            var generator = new Generator();
            var actualRecipe = generator.CreateRecipe(inputLines);

            if (string.Equals(expectedRecipe, actualRecipe, StringComparison.Ordinal)) {
                if (File.Exists(actualPath))
                    File.Delete(actualPath);
                Assert.Pass();
            }
                
            File.WriteAllText(actualPath, actualRecipe);

            string bc4 = @"C:\Program Files\Beyond Compare 4\bcompare.exe";
            Process.Start(bc4, $@"""{expectedPath}"" ""{actualPath}""");

            Assert.Fail($"Files '{expectedPath}' and '{actualPath}' are not approved");
        }
    }
}