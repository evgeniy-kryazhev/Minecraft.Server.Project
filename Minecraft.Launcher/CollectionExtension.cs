using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Minecraft.Launcher;

public static class CollectionExtension
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable) {
        return new ObservableCollection<T>(enumerable);
    }
}