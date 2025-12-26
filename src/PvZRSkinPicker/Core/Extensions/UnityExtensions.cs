namespace PvZRSkinPicker.Extensions;

using System.Diagnostics.CodeAnalysis;

using GameObject = UnityEngine.GameObject;

internal static class UnityExtensions
{
    private const string UnityNullJustification = "Needed to force Unity null check; ?? skips overloaded operators.";

    [SuppressMessage(
        "Style",
        "IDE0029:Use coalesce expression",
        Justification = UnityNullJustification)]
    [SuppressMessage(
        "Roslynator",
        "RCS1084:Use coalesce expression instead of conditional expression",
        Justification = UnityNullJustification)]
    public static T? Ref<T>(this T? obj)
        where T : UnityEngine.Object
    {
        return obj != null ? obj : null;
    }

    extension(GameObject gameObject)
    {
        public static GameObject FindOrThrow(string name)
            => GameObject.Find(name).Ref()
            ?? throw new InvalidOperationException(
                $"'{name}' was not found in the scene.");
    }
}
