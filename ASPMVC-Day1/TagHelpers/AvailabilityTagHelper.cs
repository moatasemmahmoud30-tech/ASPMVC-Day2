using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ASPMVC_Day1.TagHelpers
{
    [HtmlTargetElement("availability")]
    public class AvailabilityTagHelper : TagHelper
    {
        public bool IsAvailable { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";

            output.Attributes.RemoveAll("is-available");

            if (IsAvailable)
            {
                output.Attributes.SetAttribute("class", "badge bg-success bg-opacity-10 text-success border border-success px-3 py-2 rounded-pill");
                output.Content.SetContent("In Stock");
            }
            else
            {
                output.Attributes.SetAttribute("class", "badge bg-danger bg-opacity-10 text-danger border border-danger px-3 py-2 rounded-pill");
                output.Content.SetContent("Out of Stock");
            }
        }
    }
}
