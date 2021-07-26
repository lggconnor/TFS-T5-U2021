using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager 
{
    // Make sure the parent class is a static and if possible doesn't inherit from monobehaviour

    // This will make sure that the function can be called from anywhere without the manager script having to be attached to a gameobject
    // This script will not hold any sounds- sounds will be provided by the script that calls the function

    // this function will play a given sound with a given parent, location, and rotation.

    // DEPRECIATED: use AudioSource.PlayClipAtPoint() - Takes clip, position, volume. OR use AudioSource.PlayOneShot() - takes clip, volumescale(0-1) 
    public static void PlaySoundAtPos(AudioClip clip, GameObject parent, Vector3 spawnLocation = new Vector3(), float volume = 1f, bool destroyOnComplete = false)
    {
        // Instantiate New Gameobject with an AudioSource attached
        GameObject soundObject = new GameObject(clip.name);

        // Attach an AudioSource to the new gameobject
        soundObject.AddComponent<AudioSource>();

        // set the parent of the new GameObject
        if(parent != null)
        {
            soundObject.transform.position = parent.transform.position;
            soundObject.transform.SetParent(parent.transform);
        }
        else
        {
            // place the audiosource at position
            soundObject.transform.position = spawnLocation;
        }

        AudioSource source = soundObject.GetComponent<AudioSource>();

        // Set the clip of the new GameObject's AudioSource to the clip that was passed into the function
        source.clip = clip;

        // Set the volume of the new GameObject's AudioSource to the clip that was passed into the function
        source.volume = volume;

        // Play the clip
        source.Play();

        // Set the Gameobject to Destroy itself after the clip finishes playing
        if (destroyOnComplete)
            Object.Destroy(soundObject, clip.length);
    }

    // this function will play a new global sound from an existing audio source (eg the current song) Not as important as the above though
    public static void PlayNewTrack(AudioClip clip, AudioSource source)
    {
        // Set the clip of source to the clip that was passed into the function
        source.clip = clip;
        // Play the clip
        source.Play();
    }

    // Functions like the above where an audio source is passed in to play and pause the current clip would also be useful potentially
    public static void PauseTrack( AudioSource source)
    {
       
        // Pause the source
        source.Pause();
    }

    public static void StopTrack(AudioSource source)
    {

        // Stop the source
        source.Stop();
    }

}
