using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Builders
{
    static class DelimeterStringBuilder
    {
        public static string Build(char separator, params string[] values)
        {
            StringBuilder builder = new StringBuilder();

            foreach(var v in values)
            {
                builder.Append(v);
                builder.Append(separator);
            }

            return builder.ToString().Trim(',');
        }
    }
}
