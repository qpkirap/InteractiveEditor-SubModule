using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Module.InteractiveEditor
{
    public static class ReflectionUtility
    {
        public static T GetFieldValue<T>(this object targetObject, string fieldName)
        {
            Type targetType = targetObject.GetType();
            FieldInfo fieldInfo = null;

            while (fieldInfo == null && targetType != null)
            {
                fieldInfo = targetType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                targetType = targetType.BaseType;
            }

            if (fieldInfo != null)
            {
                if (fieldInfo.FieldType == typeof(T))
                {
                    return (T)fieldInfo.GetValue(targetObject);
                }
                else
                {
                    throw new InvalidCastException($"Тип значения не соответствует типу поля {fieldName}");
                }
            }
            else
            {
                throw new FieldAccessException($"Поле с именем {fieldName} не найдено в классе и его базовых классах");
            }
        }
        
        public static void SetPropertyValue<T>(this object targetObject, string propertyName, T value)
        {
            var targetType = targetObject.GetType();
            PropertyInfo propertyInfo = null;

            // Ищем свойство в текущем типе и его базовых типах
            while (propertyInfo == null && targetType != null)
            {
                propertyInfo = targetType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                targetType = targetType.BaseType;
            }

            if (propertyInfo != null)
            {
                if (propertyInfo.CanWrite && propertyInfo.PropertyType == typeof(T))
                {
                    propertyInfo.SetValue(targetObject, value);
                }
                else
                {
                    Debug.LogError($"Свойство {propertyName} либо доступно только для чтения, либо тип значения не соответствует типу свойства");
                }
            }
            else
            {
                Debug.LogError($"Свойство с именем {propertyName} не найдено в классе и его базовых классах");
            }
        }
        
        public static void SetFieldValue<T>(this object targetObject, string fieldName, T value)
        {
            var targetType = targetObject.GetType();
            FieldInfo fieldInfo = null;
            
            while (fieldInfo == null && targetType != null)
            {
                fieldInfo = targetType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                targetType = targetType.BaseType;
            }

            if (fieldInfo != null)
            {
                if (fieldInfo.FieldType == typeof(T))
                {
                    fieldInfo.SetValue(targetObject, value);
                }
                else
                {
                    Debug.LogError($"Тип значения не соответствует типу поля");
                }
            }
            else
            {
                Debug.LogError($"Поле с именем {fieldName} не найдено");
            }
        }
        
        public static void AddToList<T>(this object targetObject, string listFieldName, T value)
        {
            var targetType = targetObject.GetType();
            FieldInfo fieldInfo = null;

            // Ищем поле в текущем типе и его базовых типах
            while (fieldInfo == null && targetType != null)
            {
                fieldInfo = targetType.GetField(listFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                targetType = targetType.BaseType;
            }

            if (fieldInfo != null)
            {
                Type listType = fieldInfo.FieldType;

                if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    MethodInfo addMethod = listType.GetMethod("Add");

                    if (addMethod != null)
                    {
                        if (listType.GetGenericArguments()[0] == typeof(T))
                        {
                            object list = fieldInfo.GetValue(targetObject);
                            addMethod.Invoke(list, new object[] { value });
                        }
                        else
                        {
                            Debug.LogError("Тип значения не соответствует типу элементов списка");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Метод 'Add' не найден для списка с именем '{listFieldName}");
                    }
                }
                else
                {
                    Debug.LogError($"Поле с именем '{listFieldName}' не является списком (List)");
                }
            }
            else
            {
                Debug.LogError($"Поле с именем '{listFieldName}' не найдено в классе и его базовых классах");
            }
        }
        
        public static void RemoveFromList<T>(this object targetObject, string listFieldName, T value)
        {
            var targetType = targetObject.GetType();
            FieldInfo fieldInfo = null;

            // Ищем поле в текущем типе и его базовых типах
            while (fieldInfo == null && targetType != null)
            {
                fieldInfo = targetType.GetField(listFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                targetType = targetType.BaseType;
            }

            if (fieldInfo != null)
            {
                var listType = fieldInfo.FieldType;

                if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    MethodInfo removeMethod = listType.GetMethod("Remove");

                    if (removeMethod != null)
                    {
                        if (listType.GetGenericArguments()[0] == typeof(T))
                        {
                            var list = fieldInfo.GetValue(targetObject);
                            removeMethod.Invoke(list, new object[] { value });
                        }
                        else
                        {
                            Debug.LogError("Тип значения не соответствует типу элементов списка");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Метод 'Remove' не найден для списка с именем '{listFieldName}'");
                    }
                }
                else
                {
                    Debug.LogError($"Поле с именем '{listFieldName}' не является списком (List)");
                }
            }
            else
            {
                Debug.LogError($"Поле с именем '{listFieldName}' не найдено в классе и его базовых классах");
            }
        }
    }
}