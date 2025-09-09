using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace Engine;

internal static class SceneLoader
{
    public static void LoadScene(string scenePath, List<GameObject> gameObjects)
    {
        gameObjects.Clear();
        
        string jsonContent = File.ReadAllText(scenePath);
        
        SceneDefinition? scene = JsonSerializer.Deserialize<SceneDefinition>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (scene == null)
        {
            throw new InvalidOperationException($"Failed to load scene from {scenePath}");
        }
        
        Dictionary<string, GameObject> namedObjects = new();
        List<(GameObject gameObject, Dictionary<string, JsonElement> properties)> pendingObjectReferences = new();
        
        foreach (GameObjectDefinition objDef in scene.GameObjects)
        {
            GameObject? gameObject = CreateGameObject(objDef.Type);

            if (gameObject == null) continue;

            if (objDef.Properties != null)
            {
                SetSerializeFields(gameObject, objDef.Properties);
                    
                if (HasObjectReferences(gameObject))
                {
                    pendingObjectReferences.Add((gameObject, objDef.Properties));
                }
            }
                
            gameObjects.Add(gameObject);
                
            if (!string.IsNullOrEmpty(objDef.Name))
            {
                namedObjects[objDef.Name] = gameObject;
            }
        }
        
        foreach ((GameObject gameObject, Dictionary<string, JsonElement> properties) in pendingObjectReferences)
        {
            SetObjectReferences(gameObject, properties, namedObjects);
        }
    }
    
    
    private static void SetSerializeFields(GameObject gameObject, Dictionary<string, JsonElement> properties)
    {
        Type type = gameObject.GetType();
        
        FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            SerializeFieldAttribute? attr = field.GetCustomAttribute<SerializeFieldAttribute>();
            
            if (attr == null) continue;
            
            string propertyName = attr.Name ?? field.Name;
            
            if (!properties.TryGetValue(propertyName, out JsonElement value)) continue;
            
            object? convertedValue = ConvertJsonValue(value, field.FieldType);
            
            if (convertedValue != null)
            {
                field.SetValue(gameObject, convertedValue);
            }
        }
    }
    
    private static bool HasObjectReferences(GameObject gameObject)
    {
        Type type = gameObject.GetType();
        
        FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        
        foreach (FieldInfo field in fields)
        {
            SerializeFieldAttribute? attr = field.GetCustomAttribute<SerializeFieldAttribute>();
            if (attr != null && IsObjectReferenceType(field.FieldType))
            {
                return true;
            }
        }
        
        return false;
    }
    
    private static bool IsObjectReferenceType(Type type)
    {
        if (typeof(GameObject).IsAssignableFrom(type) || typeof(ITextRenderer).IsAssignableFrom(type))
        {
            return true;
        }

        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)) return false;
        
        Type elementType = type.GetGenericArguments()[0];
        
        return typeof(GameObject).IsAssignableFrom(elementType) || typeof(ITextRenderer).IsAssignableFrom(elementType);

    }
    
    private static void SetObjectReferences(
        GameObject gameObject, Dictionary<string, JsonElement> properties, Dictionary<string, GameObject> namedObjects)
    {
        Type type = gameObject.GetType();
        
        FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            SerializeFieldAttribute? attr = field.GetCustomAttribute<SerializeFieldAttribute>();
            if (attr == null || !IsObjectReferenceType(field.FieldType)) continue;
            string propertyName = attr.Name ?? field.Name;
            if (!properties.TryGetValue(propertyName, out JsonElement value)) continue;
            object? convertedValue = ConvertJsonValueWithReferences(value, field.FieldType, namedObjects);
            if (convertedValue != null)
            {
                field.SetValue(gameObject, convertedValue);
            }
        }
    }
    
    
    private static object? ConvertJsonValue(JsonElement jsonElement, Type targetType)
    {
        try
        {
            if (targetType == typeof(string))
                return jsonElement.GetString();
            if (targetType == typeof(int))
                return jsonElement.GetInt32();
            if (targetType == typeof(float))
                return jsonElement.GetSingle();
            if (targetType == typeof(double))
                return jsonElement.GetDouble();
            if (targetType == typeof(bool))
                return jsonElement.GetBoolean();
            if (targetType == typeof(long))
                return jsonElement.GetInt64();
                
            return JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType);
        }
        catch
        {
            return null;
        }
    }
    
    private static object? ConvertJsonValueWithReferences(
        JsonElement jsonElement, Type targetType, Dictionary<string, GameObject> namedObjects)
    {
        try
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = targetType.GetGenericArguments()[0];
                
                if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    IList list = (IList)Activator.CreateInstance(targetType)!;
                    
                    foreach (JsonElement element in jsonElement.EnumerateArray())
                    {
                        if (element.ValueKind != JsonValueKind.String) continue;
                        
                        string? referenceName = element.GetString();

                        if (string.IsNullOrEmpty(referenceName) ||
                            !namedObjects.TryGetValue(referenceName, out GameObject? referencedObject)) continue;
                        
                        if (elementType == typeof(ITextRenderer) && referencedObject is ITextProvider textProvider)
                        {
                            list.Add(textProvider.GetTextRenderer());
                        }
                        else if (elementType.IsInstanceOfType(referencedObject))
                        {
                            list.Add(referencedObject);
                        }
                    }
                    
                    return list;
                }
            }
            
            if (jsonElement.ValueKind == JsonValueKind.String)
            {
                string? referenceName = jsonElement.GetString();
                if (string.IsNullOrEmpty(referenceName) ||
                    !namedObjects.TryGetValue(referenceName, out GameObject? referencedObject))
                    return ConvertJsonValue(jsonElement, targetType);
                if (targetType == typeof(ITextRenderer) && referencedObject is ITextProvider textProvider)
                {
                    return textProvider.GetTextRenderer();
                }

                if (targetType.IsInstanceOfType(referencedObject))
                {
                    return referencedObject;
                }
            }
            
            return ConvertJsonValue(jsonElement, targetType);
        }
        catch
        {
            return null;
        }
    }
    
    private static GameObject? CreateGameObject(string typeName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        
        Type? type = assembly.GetTypes()
            .FirstOrDefault(t => t.Name == typeName && 
                               typeof(GameObject).IsAssignableFrom(t) &&
                               !t.IsInterface && 
                               !t.IsAbstract);
        
        if (type == null)
        {
            return null;
        }
        
        return Activator.CreateInstance(type) as GameObject;
    }
}