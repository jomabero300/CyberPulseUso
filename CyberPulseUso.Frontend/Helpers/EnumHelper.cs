using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CyberPulseUso.Frontend.Helpers;

public static class EnumHelper
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

        if (member == null)
        {
            return enumValue.ToString();
        }

        var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();

        if (displayAttribute != null)
        {
            if (displayAttribute.ResourceType != null)
            {
                return displayAttribute.GetName() ?? enumValue.ToString();
            }

            return displayAttribute.Name ?? enumValue.ToString();
        }

        return enumValue.ToString();
    }
}
