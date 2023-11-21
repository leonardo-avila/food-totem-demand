using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

public class DashRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        controller.Selectors[0].AttributeRouteModel = new AttributeRouteModel()
        {
            Template = string.Join("-", Regex.Split(controller.ControllerName, @"(?<!^)(?=[A-Z])")).ToLower()
        };
    }
}