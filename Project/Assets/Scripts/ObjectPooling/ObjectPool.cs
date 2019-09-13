using UnityEngine;
using System;
using System.Collections.Generic;

namespace ToN.ObjectPooling {
	public class ObjectPool<T> where T : class {
		protected Func<T> createFunction;
		protected Action<T> retrieveFunction;
		protected Action<T> returnFunction;
		protected Action<T> destroyFunction;
		protected Stack<T> pool;
		protected int capacity;
		protected int increment;
		protected bool allowGrowth;
		
		/// <summary>
		/// A protected constructor meant to be used by child classes.  Since Funcs and Actions can't be sent in the call to base() you can't use the public constructor.
		/// Use this as the base() constructor and then manually set the protected callbacks.  Make sure to call InitializePool() at the end of the child constructor since that is skipped in this version.
		/// </summary>
		protected ObjectPool(int capacity = 5, int increment = 1, bool allowGrowth = true) {
			if(capacity < 0) capacity = 0;
			if(increment < 1) increment = 1;

			this.capacity = capacity;
			this.increment = increment;
			this.allowGrowth = allowGrowth;
		}

		/// <summary> Create a new ObjectPool of generic type T. </summary>
		/// <param name="createFunction">The function that is called whenever the pool needs to create new copies of the object. Must return a new instance of the object being pooled.</param>
		/// <param name="retrieveFunction">Called immediately before being retrieved from the pool using GetObject. Use this to initialize the object's state.</param>
		/// <param name="returnFunction">Called immediately after returning the object to the pool using ReturnObject. Use this to perform any deactivating of the object.</param>
		/// <param name="destroyFunction">Called when the pool is cleared using ClearPools. Use this to destroy and clean up the object.</param>
		/// <param name="capacity">The initial total capacity for this object.  This many objects will be created immediately when the ObjectPool is constructed.</param>
		/// <param name="increment">The number of objects to create when incrementing the pool's size. Will only trigger if allowGrowth is set to true.</param>
		/// <param name="allowGrowth">Flag whether the pool is allowed to grow larger than the initial capacity value.</param>
		public ObjectPool(Func<T> createFunction, Action<T> retrieveFunction = null, Action<T> returnFunction = null, Action<T> destroyFunction = null, int capacity = 5, int increment = 1, bool allowGrowth = true) {
			if(createFunction == null) throw new ArgumentNullException("createFunction", "createFunction can not be null.");
			if(capacity < 0) capacity = 0;
			if(increment < 1) increment = 1;

			this.createFunction = createFunction;
			this.retrieveFunction = retrieveFunction;
			this.returnFunction = returnFunction;
			this.destroyFunction = destroyFunction;
			this.capacity = capacity;
			this.increment = increment;
			this.allowGrowth = allowGrowth;
			InitializePool();
		}
		
		protected void InitializePool() {
			pool = new Stack<T>(capacity);
			AddNewObjectsToPool(capacity);
		}
		
		protected void AddNewObjectsToPool(int count) {
			for(int i = 0; i < count; i++) {
				pool.Push(createFunction());
			}
		}
		
		/// <summary> Retrieves an object from the pool. Calls the 'retrieveFunction' before returning the object.  Automatically expands the pool and spawns 'increment' number of new objects if the pool is empty and 'allowGrowth' is true. </summary>
		/// <returns> Returns a copy of the object from the pool.  If 'allowGrowth' is false returns null when no objects remain. </returns>
		public T Pop() {
			if(pool.Count == 0) {
				if(allowGrowth) {
					AddNewObjectsToPool(increment);
					capacity += increment;
				} else {
					return null;
				}
			}
			
			T item = pool.Pop();
			if(retrieveFunction != null) retrieveFunction(item);
			return item;
		}
		
		/// <summary> Adds a new copy of an object to the pool.  Only objects that were retrieved via the 'Pop' method should be pushed back into the pool.  Calls the 'returnFunction' on the object when added to the pool. </summary>
		/// <param name="item"> The object to add back into the pool. </param>
		public void Push(T item) {
			if(item != null) {
				if(pool.Count == capacity) {
					if(allowGrowth) {
						if(returnFunction != null) returnFunction(item);
						AddNewObjectsToPool(increment - 1);
						pool.Push(item);
						capacity += increment;
					} else {
						Debug.LogWarning("Attempted to push new object to an immutable pool that is already full");
						return;
					}
				} else {
					if(returnFunction != null) returnFunction(item);
					pool.Push(item);
				}
			} else {
				throw new ArgumentNullException("item", "Attempted to add a null value to the Object Pool");
			}
		}
		
		/// <summary> Removes all copies of the object from the pool and calls the 'destroyFunction' on them.  If 'allowGrowth' is false this will result in an unusable pool since the capacity is shrunk to 0 and no new objects can be added. </summary>
		public void Clear() {
			while(pool.Count > 0) {
				T item = pool.Pop();
				if(destroyFunction != null) destroyFunction(item);
			}
			
			capacity = 0;
		}
	}
}
