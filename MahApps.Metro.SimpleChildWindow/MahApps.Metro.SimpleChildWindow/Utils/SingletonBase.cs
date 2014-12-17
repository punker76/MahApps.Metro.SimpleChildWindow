using System;
using System.Globalization;
using System.Reflection;

namespace MahApps.Metro.SimpleChildWindow.Utils
{
	/// <summary>
	/// To recap, in order to create a singleton class using the singleton base class, you need to do the following:
	/// 
	/// 1) Define a sealed class which derives from SingletonBase[T], where T is the class name you are defining. It ensures that you cannot create subclasses from this singleton class.
	/// 2) Define a single parameterless private constructor inside the class. It ensures that no instances of this class can be created externally.
	/// 3) Access the class’ singleton instance and public members by calling the Instance property.
	/// 
	/// got this implementation from http://liquidsilver.codeplex.com
	/// http://codebender.denniland.com/a-singleton-base-class-to-implement-the-singleton-pattern-in-c/
	/// 
	/// also show http://www.yoda.arachsys.com/csharp/singleton.html
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SingletonBase<T> where T : class
	{
		/// <summary>
		/// A protected constructor which is accessible only to the sub classes.
		/// </summary>
		protected SingletonBase()
		{
		}

		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static T Instance
		{
			get { return SingletonFactory.Instance; }
		}

		/// <summary>
		/// The singleton class factory to create the singleton instance.
		/// </summary>
		private class SingletonFactory
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
			static SingletonFactory()
			{
			}

			// Prevent the compiler from generating a default constructor.
			private SingletonFactory()
			{
			}

			private static WeakReference instance;

			internal static T Instance
			{
				get
				{
					var comparer = instance != null ? instance.Target as T : null;
					if (comparer == null)
					{
						comparer = GetInstance();
						instance = new WeakReference(comparer);
					}
					return comparer;
				}
			}

			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Type.InvokeMember")]
			private static T GetInstance()
			{
				var theType = typeof(T);

				T inst;

				try
				{
					inst = (T)theType.InvokeMember(theType.Name,
												   BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic,
												   null, null, null,
												   CultureInfo.InvariantCulture);
				}
				catch (MissingMethodException ex)
				{
					throw new TypeLoadException(string.Format(CultureInfo.CurrentCulture, "The type '{0}' must have a private constructor to be used in the Singleton pattern.", theType.FullName), ex);
				}

				return inst;
			}
		}
	}
}