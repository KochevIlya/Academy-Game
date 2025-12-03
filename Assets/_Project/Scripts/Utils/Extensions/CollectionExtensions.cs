using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Scripts.Utils.Extensions
{
  public static class CollectionExtension
  {
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
      if (enumerable == null)
        return true;

      if (enumerable is ICollection<T> collection)
        return collection.Count == 0;

      return !enumerable.Any();
    }

    public static void Foreach<T>(this IEnumerable<T> collection, Action<T> action)
    {
      foreach (var element in collection) action.Invoke(element);
    }

    public static void Foreach<T>(this IReadOnlyList<T> collection, Action<T> action)
    {
      for (var i = collection.Count - 1; i >= 0; i--) action.Invoke(collection[i]);
    }

    public static bool HasIndex<T>(this IList<T> collection, int index) => index >= 0 && collection.Count > index;

    public static int GetRandomIndex<T>(this IList<T> collection) => UnityEngine.Random.Range(0, collection.Count);

    public static T GetRandomElement<T>(this IList<T> collection) => collection[UnityEngine.Random.Range(0, collection.Count)];

    public static T GetRandomElementAndRemove<T>(this IList<T> collection)
    {
      var index = UnityEngine.Random.Range(0, collection.Count);
      var element = collection[index];
      collection.RemoveAt(index);
      return element;
    }

    public static T GetElementOrLast<T>(this IList<T> collection, int index) => index >= collection.Count ? collection[^1] : collection[index];

    public static T GetElementOrFirst<T>(this IList<T> collection, int index) => collection[index % collection.Count];

    public static T GetFirst<T>(this IList<T> collection) => collection[0];

    public static T GetFirstAndRemove<T>(this IList<T> collection)
    {
      var element = collection[0];
      collection.RemoveAt(0);
      return element;
    }

    public static T GetLast<T>(this IList<T> collection) => collection[^1];

    public static T GetLastAndRemove<T>(this IList<T> collection)
    {
      var element = collection[^1];
      collection.RemoveAt(collection.Count - 1);
      return element;
    }
  }
}