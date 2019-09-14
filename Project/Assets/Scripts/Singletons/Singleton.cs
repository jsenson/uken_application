namespace ToN.Singletons {
	public abstract class Singleton<T> where T : new() {
		public static T Instance {
			get {
				return Nested.Instance;
			}
		}
		
		// Thread safe - lazy instantiation
		private class Nested
		{
			internal static readonly T Instance = new T();
			static Nested () {}
		}
	}
}
