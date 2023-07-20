using System.Collections.Generic;

namespace UnityEditor.ActTimeline
{
    interface IAddDeleteItemMode
    {
        void InsertItemsAtTime(IEnumerable<ItemsPerTrack> itemsGroups, double requestedTime);
        void RemoveItems(IEnumerable<ItemsPerTrack> itemsGroups);
    }
}
