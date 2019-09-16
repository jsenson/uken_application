using UnityEngine;

// Simple audio player to start the music and react to matches with a sound effect.
public class AudioPlayer : MonoBehaviour {
    [SerializeField] private AudioSource _music = null;
    [SerializeField] private AudioSource _matchEffect = null;

    void Start() {
        if(_music != null) _music.Play();
    }

    void OnEnable() {
        SpriteTile.onTilesMatched += PlayMatchEffect;
    }

    void OnDisable() {
        SpriteTile.onTilesMatched -= PlayMatchEffect;
    }

    void PlayMatchEffect(SpriteTile t1, SpriteTile t2) {
        if(_matchEffect != null) _matchEffect.PlayOneShot(_matchEffect.clip);
    }
}
