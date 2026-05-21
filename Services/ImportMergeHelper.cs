using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace app_pm_back.Services;

/// <summary>
/// Utilitarios para mesclar dados importados sem sobrescrever valores existentes.
/// </summary>
public static class ImportMergeHelper
{
    /// <summary>
    /// Realiza a operação de merge em tabelas a serem atualizadas
    /// </summary>
    /// <param name="target"></param>
    /// <param name="source"></param>
    /// <param name="ignoreProperties"></param>
    /// <typeparam name="T"></typeparam>
    public static void MergeNonEmpty<T>(T target, T source, params string[] ignoreProperties)
        where T : class
    {
        if (target == null || source == null) return;

        var ignore = new HashSet<string>(ignoreProperties ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (!property.CanRead || !property.CanWrite) continue;
            if (ignore.Contains(property.Name)) continue;
            if (property.GetCustomAttribute<KeyAttribute>() != null) continue;

            if (property.GetMethod?.IsVirtual == true && property.PropertyType != typeof(string))
            {
                continue;
            }

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
            {
                continue;
            }

            var value = property.GetValue(source);
            if (value == null) continue;

            if (property.PropertyType == typeof(string))
            {
                var text = value as string;
                if (string.IsNullOrWhiteSpace(text)) continue;
            }
            else if (Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                // valor nullable preenchido
            }
            else if (property.PropertyType.IsValueType)
            {
                var defaultValue = Activator.CreateInstance(property.PropertyType);
                if (value.Equals(defaultValue)) continue;
            }
            else
            {
                continue;
            }

            property.SetValue(target, value);
        }
    }
}

