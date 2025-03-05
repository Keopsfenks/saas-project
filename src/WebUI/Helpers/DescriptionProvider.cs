using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Localization;

namespace WebUI.Helpers
{
    public class DescriptionProvider : IDisplayMetadataProvider
    {
        private readonly IStringLocalizer _localizer;
        public DescriptionProvider(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            var propertyAttributes = context.Attributes;
            var modelMetadata = context.DisplayMetadata;
            var propertyName = context.Key.Name;
            modelMetadata.DisplayName = () => _localizer[propertyName ?? "NotAvailable"]?.Value ?? propertyName + ".EksikVeri";
        }
    }
}
