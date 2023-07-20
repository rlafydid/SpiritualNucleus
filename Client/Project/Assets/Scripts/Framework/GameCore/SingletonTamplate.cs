using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class SingletonTamplate<T> where T : new()
{
		private static T instance;
		public static T Instance
		{
				get{
						if(instance == null)
						{
								instance = new T();
						}
						return instance;
				}
		}

}
