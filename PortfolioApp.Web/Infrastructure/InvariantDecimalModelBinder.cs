using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PortfolioApp.Web.Infrastructure;

// HTML5 number/range inputs always submit "." as the decimal separator regardless of browser
// locale (per the HTML spec's floating-point parsing algorithm), but ASP.NET Core's default
// model binder parses using the request's current culture. Under tr-TR that expects "," as the
// decimal separator, so a browser-submitted "3.75" silently binds as 375. Force invariant-culture
// parsing for decimal so numeric inputs round-trip correctly regardless of UI culture.
public class InvariantDecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(value))
            return Task.CompletedTask;

        if (!decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            bindingContext.ModelState.TryAddModelError(modelName, "Geçerli bir sayı giriniz.");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(result);
        return Task.CompletedTask;
    }
}

public class InvariantDecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var modelType = context.Metadata.UnderlyingOrModelType;
        return modelType == typeof(decimal) ? new InvariantDecimalModelBinder() : null;
    }
}
