using System.Linq;
using System.Text;

namespace RecipeGenerator
{
    public class Cell {
        private readonly string _value;

        public Cell(string value)
        {
            _value = value;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append("<td>");
            builder.Append(_value);
            builder.AppendLine("</td>");
            return builder.ToString();
        }
    }
}
