using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
	[Header("Cutscene Components")]
	public AudioSource voiceOverSource;
	public AudioSource music;
	public Image blackScreen;

	[Header("Intro Texts (Assign in Inspector)")]
	public Text[] introTexts; // Assign: Sometimes, Today, Finality, Go Down Fighting

	[Header("Voice Over Timings (Optional)")]
	public float[] voiceOverTimings; // If you want to sync audio per line

	private int currentIndex = 0;
	private bool cutsceneActive = true;

	private void Start()
	{
		// Hide all texts initially
		foreach (var t in introTexts)
			t.gameObject.SetActive(false);

		blackScreen.gameObject.SetActive(true);

		// Show first text
		if (introTexts.Length > 0)
		{
			introTexts[0].gameObject.SetActive(true);
			voiceOverSource.gameObject.SetActive(true);
			music.gameObject.SetActive(true);

            Debug.Log("First cutscene text enabled: " + introTexts[0].name);
		}
		else
		{
			Debug.LogWarning("No introTexts assigned in CutsceneManager!");
		}

		// Play voice over if provided
		if (voiceOverSource != null)
			voiceOverSource.Play();
	}

	private void Update()
	{
		if (!cutsceneActive)
			return;

		if (Input.anyKeyDown)
		{
			// Hide current text
			introTexts[currentIndex].gameObject.SetActive(false);

			currentIndex++;

			if (currentIndex < introTexts.Length)
			{
				// Show next text
				introTexts[currentIndex].gameObject.SetActive(true);

				// Optionally, play next audio segment if you split your voice over
				// if (voiceOverSource != null && voiceOverTimings.Length > currentIndex)
				//     voiceOverSource.time = voiceOverTimings[currentIndex];
			}
            else
            {
                // End cutscene: hide black screen and all texts
                SceneManager.LoadScene("aliveState");
                blackScreen.gameObject.SetActive(false);
                cutsceneActive = false;
            }
        }
	}
}