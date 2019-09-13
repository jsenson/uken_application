using UnityEngine;

namespace ToN.ObjectPooling {
	/// <summary> A Generic ObjectPool helper implementation to handle any Unity Components.  You can use this if you want a simple Pool that calls gameObject.SetActive() when returning and retrieving objects without any object specific behaviour. </summary>
	public class GameObjectPool<T> : ObjectPool<T> where T : Component {
		private T _prefab;
		private Transform _parent;

		/// <summary> Create a new ObjectPool that handles creating, activating, and deactivating the given prefab as a child of the given parent Transform. </summary>
		/// <param name="prefab"> The original prefab object to use when creating new instances. </param>
		/// <param name="parent"> The parent Transform that new instances of prefab are created as children of. </param>
		/// <param name="capacity">The initial total capacity for this object.  This many objects will be created immediately when the ObjectPool is constructed.</param>
		/// <param name="increment">The number of objects to create when incrementing the pool's size. Will only trigger if allowGrowth is set to true.</param>
		/// <param name="allowGrowth">Flag whether the pool is allowed to grow larger than the initial capacity value.</param>
		public GameObjectPool(T prefab, Transform parent, int capacity = 5, int increment = 1, bool allowGrowth = true) : base(capacity, increment, allowGrowth) {
			_prefab = prefab;
			_parent = parent;

			createFunction = OnPoolCreate;
			retrieveFunction = OnPoolRetrieve;
			returnFunction = OnPoolReturn;
			destroyFunction = OnPoolDestroy;
			InitializePool();
		}

		/// <summary> The function called to create new instances of the object for the pool. </summary>
		/// <returns> A new instance of the prefab object. </returns>
		protected virtual T OnPoolCreate() {
			T obj = GameObject.Instantiate<T>(_prefab, _parent);
			obj.gameObject.SetActive(false);
			return obj;
		}

		/// <summary> The function called when an object is retrieved from the pool. </summary>
		/// <param name="obj"> The object that was retrieved from the pool. </param>
		protected virtual void OnPoolRetrieve(T obj) {
			obj.gameObject.SetActive(true);
		}

		/// <summary> The function called when an object is returned to the pool. </summary>
		/// <param name="obj"> The object that was returned to the pool. </param>
		protected virtual void OnPoolReturn(T obj) {
			obj.gameObject.SetActive(false);
		}

		/// <summary> The function called when an object is deleted from the pool. </summary>
		/// <param name="obj"> The object that was deleted from the pool. </param>
		protected virtual void OnPoolDestroy(T obj) {
			GameObject.Destroy(obj.gameObject);
		}
	}

	/// <summary> A non-generic version of GameObjectPool<T> to handle GameObjects directly.  Annoyingly GameObject is a sealed class which can't be used as a generic type constraint so use this instead of GameObjectPool<GameObject>. </summary>
	public class GameObjectPool : ObjectPool<GameObject> {
		private GameObject _prefab;
		private Transform _parent;

		/// <summary> Create a new ObjectPool that handles creating, activating, and deactivating the given prefab as a child of the given parent Transform. </summary>
		/// <param name="prefab"> The original prefab object to use when creating new instances. </param>
		/// <param name="parent"> The parent Transform that new instances of prefab are created as children of. </param>
		/// <param name="capacity">The initial total capacity for this object.  This many objects will be created immediately when the ObjectPool is constructed.</param>
		/// <param name="increment">The number of objects to create when incrementing the pool's size. Will only trigger if allowGrowth is set to true.</param>
		/// <param name="allowGrowth">Flag whether the pool is allowed to grow larger than the initial capacity value.</param>
		public GameObjectPool(GameObject prefab, Transform parent, int capacity = 5, int increment = 1, bool allowGrowth = true) : base(capacity, increment, allowGrowth) {
			_prefab = prefab;
			_parent = parent;

			createFunction = OnPoolCreate;
			retrieveFunction = OnPoolRetrieve;
			returnFunction = OnPoolReturn;
			destroyFunction = OnPoolDestroy;
			InitializePool();
		}

		/// <summary> The function called to create new instances of the object for the pool. </summary>
		/// <returns> A new instance of the prefab object. </returns>
		protected virtual GameObject OnPoolCreate() {
			GameObject obj = GameObject.Instantiate<GameObject>(_prefab, _parent);
			obj.gameObject.SetActive(false);
			return obj;
		}

		/// <summary> The function called when an object is retrieved from the pool. </summary>
		/// <param name="obj"> The object that was retrieved from the pool. </param>
		protected virtual void OnPoolRetrieve(GameObject obj) {
			obj.gameObject.SetActive(true);
		}

		/// <summary> The function called when an object is returned to the pool. </summary>
		/// <param name="obj"> The object that was returned to the pool. </param>
		protected virtual void OnPoolReturn(GameObject obj) {
			obj.gameObject.SetActive(false);
		}

		/// <summary> The function called when an object is deleted from the pool. </summary>
		/// <param name="obj"> The object that was deleted from the pool. </param>
		protected virtual void OnPoolDestroy(GameObject obj) {
			GameObject.Destroy(obj.gameObject);
		}
	}
}
