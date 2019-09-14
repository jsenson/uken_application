using UnityEngine;
/// <summary>
/// by Jon Senson
/// Generic Singleton base class for Unity MonoBehaviours.  Simply extend this class with the child class as the generic type.
/// The Awake() method needs to be overridden if you need to use it and returns a bool value.  It must also check the base class's Awake() method and return false if it failed in order to avoid Instantiating multiples of itself.
/// 
/// Example:
/// 
/// public class MyClass : MonoBehaviourSingleton<MyClass> {
/// 	public override bool Awake() {
/// 		if(!base.Awake()) return false;
/// 
/// 		// Do your initilization stuff
/// 		
/// 		return true;
/// 	}
/// }
/// </summary>
namespace ToN.Singletons {
	public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component {
		private static T mInstance;
		// Apparently OnDisable/OnDestroy are called at the same time per object. It's possible to destroy the manager, then have another object access it in OnDisable. Which is stupid. 
		// Sanity check to make sure we don't create a new manager object that stays alive after quitting the game in the editor
		private static bool mShuttingDown = false;

		public static T Instance {
			get {
				if (mInstance == null) {
					mInstance = FindObjectOfType<T> ();
					if (mInstance == null && !mShuttingDown) {
						GameObject obj = new GameObject (typeof(T).ToString());
						mInstance = obj.AddComponent<T> ();
					}
				}
				return mInstance;
			}
		}

		// Sanity check to avoid leaking new instances of the Singleton during shutdown
		protected virtual void OnApplicationQuit() {
			mShuttingDown = true;
		}

		protected virtual void OnDestroy() {
			if(mInstance == this) {
				mInstance = null;
				mShuttingDown = true;
			}
		}

		protected virtual bool Awake() {
			mShuttingDown = false;
			if (mInstance == null || mInstance == this) {
				mInstance = this as T;
				return true;
			} else {
				Destroy(this);
				return false;
			}
		}
	}
}
