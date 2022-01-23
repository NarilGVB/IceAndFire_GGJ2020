using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            ChangeVolume(0.2f, "Master Volume");
        }

        if (instance != this)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    [SerializeField] AudioMixer mixMaster, mixFx, mixMusic;
    [SerializeField] AudioSource auMusic;
    public AudioSource auButton, auGetPoints, auEnemyDies, auPlayerDeadFreeze, auPlayerDeadBurn
        , auFireAbility, auIceAbility, auFireTouchEnemy, auIceTouchEnemy, auTemperatureLevelUp, auTemperatureLevelDown;

    public void PlayButton() { auButton.Play(); }
    public void PlayGetPoints() { auGetPoints.Play(); }
    public void PlayEnemyDies() { auEnemyDies.Play(); }
    public void PlayPlayerDeadFreeze() {  auPlayerDeadFreeze.Play(); }
    public void PlayPlayerDeadBurn() { auPlayerDeadBurn.Play(); }
    public void PlayIceAbility() { auIceAbility.Play(); }
    public void PlayFireAbility() { auFireAbility.Play(); }
    public void PlayFireTouchEnemy() { auFireTouchEnemy.Play(); }
    public void PlayIceTouchEnemy() { auIceTouchEnemy.Play(); }
    public void PlayTemperatureLevelUp() { auTemperatureLevelUp.Play(); }
    public void PlayTemperatureLevelDown() { auTemperatureLevelDown.Play(); }

    [SerializeField] float multiplier = 30f;
    
    public void ChangeVolume(float volume, string parameterName)
    {
        
        mixMaster.SetFloat(parameterName, Mathf.Log10(volume) * multiplier);
    }

    [SerializeField] AudioClip[] songs;
    int actualSong = 0;
    private void Update()
    {
        //Debug.Log(auButton.enabled);

        if (auMusic.time >= auMusic.clip.length)
        {
            print("Music ended");
            actualSong++;

            if (actualSong >= songs.Length)
                actualSong = 0;
            
            auMusic.clip = songs[actualSong];
            auMusic.Play();
        }
    }

}
