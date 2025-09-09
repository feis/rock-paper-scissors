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

        SceneDefinition? scene = JsonSerializer.Deserialize<SceneDefinition>(jsonContent);
        
        if (scene == null)
        {
            throw new InvalidOperationException($"Failed to load scene from {scenePath}");
        }
        
        Dictionary<string, GameObject> namedObjects = new();
        
        List<(Component component, Dictionary<string, JsonElement> properties)> pendingComponentReferences = new();
        
        foreach (GameObjectDefinition objDef in scene.GameObjects)
        {
            GameObject gameObject = new();
            
            if (!string.IsNullOrEmpty(objDef.Name))
            {
                gameObject.Name = objDef.Name;
                namedObjects[objDef.Name] = gameObject;
                GameObject.NamedGameObjects[objDef.Name] = gameObject;
            }
            
            foreach (ComponentDefinition compDef in objDef.Components)
            {
                Component? component = CreateComponent(compDef.Type);
                
                if (component == null) continue;
                
                gameObject.AddComponent(component);

                if (compDef.Properties == null) continue;
                
                SetSerializeFields(component, compDef.Properties);
                    
                if (HasObjectReferences(component))
                {
                    pendingComponentReferences.Add((component, compDef.Properties));
                }
            }
            
            gameObjects.Add(gameObject);
        }
        
        foreach ((Component component, Dictionary<string, JsonElement> properties) in pendingComponentReferences)
        {
            SetObjectReferences(component, properties, namedObjects);
        }
    }
    
    
    private static void SetSerializeFields(Component component, Dictionary<string, JsonElement> properties)
    {
        Type type = component.GetType();
        
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
                field.SetValue(component, convertedValue);
            }
        }
    }
    
    private static bool HasObjectReferences(Component component)
    {
        Type type = component.GetType();
        
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
        if (typeof(GameObject).IsAssignableFrom(type) || typeof(Component).IsAssignableFrom(type))
        {
            return true;
        }

        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>)) return false;
        
        Type elementType = type.GetGenericArguments()[0];
        
        return typeof(GameObject).IsAssignableFrom(elementType) || typeof(Component).IsAssignableFrom(elementType);

    }
    
    private static void SetObjectReferences(
        Component component, Dictionary<string, JsonElement> properties, Dictionary<string, GameObject> namedObjects)
    {
        Type type = component.GetType();
        
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
                field.SetValue(component, convertedValue);
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
            
            // 不要對複雜的泛型類型（如 List<>）進行直接反序列化
            if (targetType.IsGenericType)
                return null;
                
            return JsonSerializer.Deserialize(jsonElement.GetRawText(), targetType);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ConvertJsonValue error for type {targetType.Name}: {e.Message}");
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
                
                if (jsonElement.ValueKind == JsonValueKind.Object && 
                    jsonElement.TryGetProperty("ElementType", out JsonElement elementTypeElement) &&
                    jsonElement.TryGetProperty("References", out JsonElement referencesElement))
                {
                    string? elementTypeName = elementTypeElement.GetString();
                    if (string.IsNullOrEmpty(elementTypeName)) return null;
                    
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    Type? resolvedElementType = assembly.GetTypes()
                        .FirstOrDefault(t => t.Name == elementTypeName && typeof(Component).IsAssignableFrom(t));
                    
                    if (resolvedElementType == null) 
                    {
                        Console.WriteLine($"Could not find component type: {elementTypeName}");
                        return null;
                    }
                    
                    IList list = (IList)Activator.CreateInstance(targetType)!;
                    
                    foreach (JsonElement element in referencesElement.EnumerateArray())
                    {
                        if (element.ValueKind != JsonValueKind.String) continue;
                        
                        string? referenceName = element.GetString();

                        if (string.IsNullOrEmpty(referenceName) ||
                            !namedObjects.TryGetValue(referenceName, out GameObject? referencedObject)) 
                        {
                            continue;
                        }
                        
                        Component? component = referencedObject.GetComponent(resolvedElementType);
                        if (component != null)
                        {
                            list.Add(component);
                        }
                        else
                        {
                            Console.WriteLine($"Component {elementTypeName} not found in {referenceName}");
                        }
                    }
                    
                    return list;
                }
                else if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    IList list = (IList)Activator.CreateInstance(targetType)!;
                    
                    foreach (JsonElement element in jsonElement.EnumerateArray())
                    {
                        if (element.ValueKind != JsonValueKind.String) continue;
                        
                        string? referenceName = element.GetString();

                        if (string.IsNullOrEmpty(referenceName) ||
                            !namedObjects.TryGetValue(referenceName, out GameObject? referencedObject)) continue;
                        
                        if (typeof(Component).IsAssignableFrom(elementType))
                        {
                            Component? component = referencedObject.GetComponent(elementType);
                            if (component != null)
                            {
                                list.Add(component);
                            }
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
                if (typeof(Component).IsAssignableFrom(targetType))
                {
                    Component? component = referencedObject.GetComponent(targetType);
                    if (component != null)
                    {
                        return component;
                    }
                }

                if (targetType.IsInstanceOfType(referencedObject))
                {
                    return referencedObject;
                }
            }
            
            return ConvertJsonValue(jsonElement, targetType);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    
    private static Component? CreateComponent(string typeName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        
        Type? type = assembly.GetTypes()
            .FirstOrDefault(t => t.Name == typeName && 
                                 typeof(Component).IsAssignableFrom(t) &&
                                 t is { IsInterface: false, IsAbstract: false });
        
        if (type == null)
        {
            return null;
        }
        
        return Activator.CreateInstance(type) as Component;
    }
}