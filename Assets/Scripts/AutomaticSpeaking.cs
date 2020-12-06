using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class AutomaticSpeaking : MonoBehaviour
{
    const string LANG_CODE = "ro-ro";

    private AudioSource audioSource;

    bool rau = false;
    bool bine = false;
    bool salut = false;
    bool asta1 = false;
    [SerializeField]
    Text uiText;

    public Animator animator;
    public Animator animatorGura;

    private void Start()
    {
        Setup(LANG_CODE);

#if UNITY_ANDROID
       // SpeechToText.instance.onPartialResultsCallback = OnPartialSpeechResult;
#endif

        //SpeechToText.instance.onResultCallback = OnFinalSpeechResult;
        TextToSpeech.instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.instance.onDoneCallback = OnSpeakStop;
        
        CheckPermission();
        
    }

    void CheckPermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
    }

    public void Update()
    {
        StartListening();
        SpeechToText.instance.onPartialResultsCallback = onPartialResultsCallback;
        void onPartialResultsCallback(string result)
        {
            uiText.text = result;
            if (result.Contains("Salut") || result.Contains("salut") || result.Contains("Bună ziua") || result.Contains("bună ziua"))
            {
                salut = true;
                StopListening();
                StartSpeaking("O zi bună, domnule Stamatie! Cum vă simțiți astăzi?");
                StartListening();
            }
            
            if (result.Contains("bine"))
            {
                bine = true;
                animator.SetBool("FeelingBad", false);
                animator.SetBool("FeelingGood", true);
                animatorGura.SetBool("Bad", false);
                animatorGura.SetBool("Good", true);
                StopListening();
                StartSpeaking("Mă bucur!");
                StartListening();
                
            }

            if (result.Contains("rău"))
            {
                rau = true;
                animator.SetBool("FeelingBad", true);
                animator.SetBool("FeelingGood", false);
                animatorGura.SetBool("Bad", true);
                animatorGura.SetBool("Good", false);
                StopListening();
                StartSpeaking("Alertez personalul medical");
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.alarm);
                StartListening();
            }

            if(result.Contains("ce faci Mihaela") || result.Contains("Ce faci Mihaela"))
            {
                StopListening();
                StartSpeaking("Cânt cu orchestra extratereștrii");
                StartListening();
            }

            if((rau || bine) && salut && !asta1)
            {
                StartCoroutine(Semne_Vitale(6));
                asta1 = true;
            }
        }
    }
    IEnumerator Semne_Vitale(float time)
    {
        yield return new WaitForSeconds(time);

        StartSpeaking("Vă rog, plasați degetul pe senzor pentru a înregistra semnele vitale");
    }

    #region Text to Speech

    public void StartSpeaking(string message)
    {
        TextToSpeech.instance.StartSpeak(message);
    }

    public void StopSpeaking()
    {
        TextToSpeech.instance.StopSpeak();
    }

    void OnSpeakStart()
    {
        Debug.Log("Talking started...");
    }

    void OnSpeakStop()
    {
        Debug.Log("Talking stopped");
    }

    #endregion

    #region Speech To Text
    
    public void StartListening()
    {
        SpeechToText.instance.StartRecording();
    }

    public void StopListening()
    {
        SpeechToText.instance.StopRecording();
    }
    
    #endregion

    void Setup(string code)
    {
        TextToSpeech.instance.Setting(code, 1, 1);
        SpeechToText.instance.Setting(code);
    }
}