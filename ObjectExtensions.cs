/*

The ObjectExtensions class provides a set of extension methods for efficiently mapping properties between objects in .NET applications. In this example, we demonstrate the usage of the AutoMapNew, AutoMapTo, and AutoMapFrom methods using the Country and CountrySummary classes.
    
// Assuming the 'Country' class has properties 'Id', 'Name', 'Iso2', 'Iso3', and 'Tld',
// and the 'CountrySummary' class has properties 'Name' and 'Iso2'.
Country sourceCountry = new Country { Id = 1, Name = "United States", Iso2 = "US", Iso3 = "USA", Tld = ".us" };
CountrySummary destCountrySummary;

// Example usage of AutoMapNew method:
// Maps properties from 'sourceCountry' to a new instance of 'CountrySummary'.
destCountrySummary = sourceCountry.AutoMapNew<CountrySummary>();

// Example usage of AutoMapTo method:
// Maps properties from 'sourceCountry' to an existing 'CountrySummary' instance.
sourceCountry.AutoMapTo(destCountrySummary);

// Example usage of AutoMapFrom method:
// Maps properties from 'sourceCountry' to the 'destCountrySummary' instance.
// The 'Id' property is not included in the mapping (default behavior).
destCountrySummary.AutoMapFrom(sourceCountry);

// Example usage of AutoMapFrom method with includeId = true:
// Maps properties from 'sourceCountry' to the 'destCountrySummary' instance.
// The 'Id' property is included in the mapping.
destCountrySummary.AutoMapFrom(sourceCountry, true);

// To exclude any properties that should not be mapped, modify the following HashSet<string>:
    
HashSet<string> excludeProperties = new() { "ETag" };

// With these examples, you can easily map properties between objects in your .NET applications using the provided extension methods.

*/

public static class ObjectExtensions
{
    // Creates a new instance of the destination type T and maps the properties from the source object.
    // Optional parameter 'includeId' determines whether the 'Id' property should be mapped or not.
    public static T AutoMapNew<T>(this object source, bool includeId = false) where T : new()
    {
        T destination = new();
        AutoMapValues(source, destination, includeId);
        return destination;
    }

    // Maps properties from the source object to an existing destination object.
    // Optional parameter 'includeId' determines whether the 'Id' property should be mapped or not.
    public static void AutoMapTo(this object source, object destination, bool includeId = false)
    {
        AutoMapValues(source, destination, includeId);
    }

    // Maps properties from the source object to an existing destination object (reverse of AutoMapTo).
    // Optional parameter 'includeId' determines whether the 'Id' property should be mapped or not.
    public static void AutoMapFrom(this object destination, object source, bool includeId = false)
    {
        AutoMapValues(source, destination, includeId);
    }

    // Shared method that maps properties between source and destination objects.
    // Optional parameter 'includeId' determines whether the 'Id' property should be mapped or not.
    public static void AutoMapValues(object source, object destination, bool includeId = false)
    {
        if (source == null || destination == null)
            throw new Exception("Source and destination object cannot be null.");

        Dictionary<string, PropertyInfo> sourceProperties = GetCachedProperties(type: source.GetType(), includeId: includeId, canRead: true);

        foreach (KeyValuePair<string, PropertyInfo> destinationProperty in GetCachedProperties(type: destination.GetType(), includeId: includeId, canWrite: true))
            if (sourceProperties.TryGetValue(destinationProperty.Key, out PropertyInfo? sourceProperty))
                destinationProperty.Value.SetValue(destination, sourceProperty.GetValue(source));
    }

    // Cache for storing PropertyInfo objects, to minimize reflection overhead.
    private static readonly Dictionary<(string, byte), Dictionary<string, PropertyInfo>> PropertyInfoCache = new();

    // Fetches the relevant properties based on the provided criteria (e.g., includeId, canWrite, and canRead).
    // Returns a dictionary of PropertyInfo objects keyed by property names.
    private static Dictionary<string, PropertyInfo> GetCachedProperties(Type type, bool includeId, bool? canWrite = null, bool? canRead = null)
    {
        var cacheKey = (type.Name, (byte)((includeId ? 1 : 0) | (canWrite.HasValue && canWrite.Value ? 2 : 0) | (canRead.HasValue && canRead.Value ? 4 : 0)));

        if (!PropertyInfoCache.TryGetValue(cacheKey, out var properties))
        {
            HashSet<string> excludeProperties = new() { "ETag" };
            if (!includeId) excludeProperties.Add("Id");

            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => !excludeProperties.Contains(prop.Name) && (!canWrite.HasValue || (canWrite.Value == prop.CanWrite)) && (!canRead.HasValue || (canRead.Value == prop.CanRead)))
                .ToDictionary(prop => prop.Name, prop => prop);

            PropertyInfoCache[cacheKey] = properties;
        }

        return properties;
    }
}
