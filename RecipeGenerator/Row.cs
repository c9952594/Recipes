using System.Collections.Generic;
using System.Text;

namespace RecipeGenerator
{
    public class Row : List<Cell> {
        public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine("<tr>");
            foreach (var cell in this) {
                builder.Append(cell.ToString());
            }
            builder.AppendLine("</tr>");
            return builder.ToString();
        }
    }
}
