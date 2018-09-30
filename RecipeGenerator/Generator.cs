using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RecipeGenerator
{
    public class Generator
    {
        public string CreateRecipe(IEnumerable<string> lines)
        {
            var table = new Table(lines.Select(line => 
                new Row() {
                    new Cell(line)
                }));

            return table.ToString();
        }
    }
}
