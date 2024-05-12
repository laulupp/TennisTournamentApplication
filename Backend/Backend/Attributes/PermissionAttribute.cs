namespace Backend.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class PermissionAttribute : Attribute
{
    public string[] Permissions { get; }

    public PermissionAttribute(params string[] permissions)
    {
        Permissions = permissions;
    }
}