// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CivOne.Advances;
using CivOne.Buildings;
using CivOne.Civilizations;
using CivOne.Concepts;
using CivOne.Governments;
using CivOne.Graphics.Sprites;
using CivOne.Tiles;
using CivOne.Units;
using CivOne.Wonders;

namespace CivOne
{
	internal static class Reflect
	{
		private static void Log(string text, params object[] parameters) => RuntimeHandler.Runtime.Log(text, parameters);

		private static Dictionary<IPlugin, Assembly> _plugins;
		private static void LoadPlugins()
		{
			_plugins = new Dictionary<IPlugin, Assembly>();
			foreach(string filename in Directory.GetFiles(Settings.Instance.PluginsDirectory, "*.dll"))
			{
				try
				{
					Assembly assembly = Assembly.LoadFile(filename);
					Type[] types = assembly.GetTypes().Where(x => x.Namespace == "CivOne" && x.Name == "Plugin" && x.GetInterfaces().Contains(typeof(IPlugin))).ToArray();
					if (types.Count() != 1)
					{
						Log($" - Invalid plugin format: {filename}");
						continue;
					}
					
					IPlugin plugin = (IPlugin)Activator.CreateInstance(types[0]);
					_plugins.Add(plugin, assembly);
				}
				catch (Exception ex)
				{
					Log($" - Loading plugin failed: {filename}");
					Log($"   - {ex.Message}");
				}
			}
		}

		private static IEnumerable<Assembly> GetAssemblies
		{
			get
			{
				yield return typeof(Reflect).GetTypeInfo().Assembly;
			}
		}
		
		private static IEnumerable<T> GetTypes<T>()
		{
			foreach (Assembly asm in GetAssemblies)
			foreach (Type type in asm.GetTypes().Where(t => typeof(T).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()) && t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract))
			{
				yield return (T)Activator.CreateInstance(type);
			}

			foreach (Assembly asm in GetAssemblies)
			foreach (Type type in asm.GetTypes().Where(t => (t is T) && t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract))
			{
				yield return (T)Activator.CreateInstance(type);
			}
		}
		
		internal static IEnumerable<IAdvance> GetAdvances()
		{
			return GetTypes<IAdvance>().OrderBy(x => x.Id);
		}

		internal static IEnumerable<ICivilization> GetCivilizations()
		{
			return GetTypes<ICivilization>().OrderBy(x => (int)x.Id);
		}
		
		internal static IEnumerable<IGovernment> GetGovernments()
		{
			return GetTypes<IGovernment>().OrderBy(x => x.Id);
		}
		
		internal static IEnumerable<IUnit> GetUnits()
		{
			return GetTypes<IUnit>().OrderBy(x => (int)x.Type);
		}
		
		internal static IEnumerable<IBuilding> GetBuildings()
		{
			return GetTypes<IBuilding>().OrderBy(x => x.Id);
		}
		
		internal static IEnumerable<IWonder> GetWonders()
		{
			return GetTypes<IWonder>().OrderBy(x => x.Id);
		}

		internal static IEnumerable<IProduction> GetProduction()
		{
			foreach (IProduction production in GetUnits())
				yield return production;
			foreach (IProduction production in GetBuildings())
				yield return production;
			foreach (IProduction production in GetWonders())
				yield return production;
		}
		
		internal static IEnumerable<IConcept> GetConcepts()
		{
			return GetTypes<IConcept>();
		}
		
		internal static IEnumerable<ICivilopedia> GetCivilopediaAll()
		{
			List<string> articles = new List<string>();
			foreach (ICivilopedia article in GetTypes<ICivilopedia>().OrderBy(a => (a is IConcept) ? 1 : 0))
			{
				if (articles.Contains(article.Name)) continue;
				articles.Add(article.Name);
				yield return article;
			}
		}
		
		internal static IEnumerable<ICivilopedia> GetCivilopediaAdvances()
		{
			return GetTypes<IAdvance>();
		}
		
		internal static IEnumerable<ICivilopedia> GetCivilopediaCityImprovements()
		{
			foreach (ICivilopedia civilopedia in GetTypes<IBuilding>())
				yield return civilopedia;
			foreach (ICivilopedia civilopedia in GetTypes<IWonder>())
				yield return civilopedia;
		}
		
		internal static IEnumerable<ICivilopedia> GetCivilopediaUnits()
		{
			return GetTypes<IUnit>();
		}
		
		internal static IEnumerable<ICivilopedia> GetCivilopediaTerrainTypes()
		{
			return GetTypes<ITile>();
		}

		internal static IEnumerable<IPlugin> Plugins()
		{
			if (_plugins == null)
			{
				LoadPlugins();
			}
			return _plugins.Keys;
		}

		private static IEnumerable<Type> PluginModifications
		{
			get
			{
				if (_plugins == null) yield break;
				foreach (Assembly assembly in _plugins.Values)
				foreach (Type type in assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.GetInterfaces().Contains(typeof(IModification))))
				{
					yield return type;
				}
			}
		}

		private static object[] ParseParameters(params object[] parameters)
		{
			List<object> output = new List<object>();
			foreach (object parameter in parameters)
			{
				switch (parameter)
				{
					case string stringParameter:
						output.Add(stringParameter);
						break;
					case int intParameter:
						output.Add(intParameter);
						break;
					default:
						output.Add(parameter);
						break;
				}
			}
			return output.ToArray();
		}

		internal static IEnumerable<T> GetModifications<T>()
		{
			foreach (Type type in PluginModifications.Where(x => x.IsSubclassOf(typeof(T))))
			{
				yield return (T)Activator.CreateInstance(type);
			}
		}
		
		internal static void PreloadCivilopedia()
		{
			Log("Civilopedia: Preloading articles...");
			foreach (ICivilopedia article in GetCivilopediaAll());
			Log("Civilopedia: Preloading done!");
		}
	}
}