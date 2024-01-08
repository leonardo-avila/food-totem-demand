using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

namespace FoodTotem.Demand.API.Setup;
public class DashRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        controller.Selectors[0].AttributeRouteModel = new AttributeRouteModel()
        {
            Template = string.Join("-", Regex.Split(controller.ControllerName, @"(?<!^)(?=[A-Z])", RegexOptions.None, TimeSpan.FromMilliseconds(200))).ToLower()
        };
    }
}