using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ScheduleApp.Helpers
{
    public static class ListHelper
    {
        public static HtmlString OutputList(this IHtmlHelper html,string list)
        {
            var result = $"<h1>{list}</h1>";
            return new HtmlString(result);
        }
    }
}
