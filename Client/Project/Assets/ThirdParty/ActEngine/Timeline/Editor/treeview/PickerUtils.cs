using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.ActTimeline
{
    static class PickerUtils
    {
        public static List<object> pickedElements { get; private set; }

        public static Vector2 lastMousePos;
        public static void DoPick(WindowState state, Vector2 mousePosition)
        {
            if (state.GetWindow().sequenceContentRect.Contains(mousePosition))
            {
                lastMousePos = mousePosition;
                pickedElements = state.spacePartitioner.GetItemsAtPosition<object>(mousePosition).ToList();
            }
            else
            {
                if (pickedElements != null)
                    pickedElements.Clear();
                else
                    pickedElements = new List<object>();
            }
        }



        public static ILayerable TopmostPickedItem()
        {
            return PickedItemsSortedByZOrderOfType<ILayerable>().FirstOrDefault();
        }

        public static T TopmostPickedItemOfType<T>() where T : class, ILayerable
        {
            return PickedItemsSortedByZOrderOfType<T>().FirstOrDefault();
        }

        public static T TopmostPickedItemOfType<T>(Func<T, bool> predicate) where T : class, ILayerable
        {
            return PickedItemsSortedByZOrderOfType<T>().FirstOrDefault(predicate);
        }

        static IEnumerable<T> PickedItemsSortedByZOrderOfType<T>() where T : class, ILayerable
        {
            return pickedElements.OfType<T>().OrderByDescending(x => x.zOrder);
        }

        public static T FirstPickedElementOfType<T>() where T : class, IBounds
        {
            return pickedElements.FirstOrDefault(e => e is T) as T;
        }
    }
}
