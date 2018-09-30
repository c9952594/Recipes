using System.Collections.Generic;
using System.Text;

namespace RecipeGenerator
{
    public class Table : List<Row> {
        public Table(IEnumerable<Row> collection) : base(collection)
        {
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine("<table>");
            foreach (var row in this) {
                builder.Append(row.ToString());
            }
            builder.AppendLine("</table>");
            return builder.ToString();
        }
    }
}
