using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls a character's (Pj or Enemy) health.
/// </summary>
[RequireComponent(typeof(CharacterMovementController))]
public class CharacterHealthController : MonoBehaviour
{

    /// <summary>
    /// Number of HP.
    /// </summary>
    [SerializeField]
    private float health;

    [SerializeField]
    private UnityEngine.UI.Image gameOver;


    void FixedUpdate()
    {


    }

    /// <summary>
    /// Does the points of damage specified.
    /// </summary>
    /// <param name="damage">Damage points.</param>
    public void DoDamage(float damage)
    {
        health -= damage;
        PjMovementController pj = this.gameObject.GetComponent <PjMovementController>();
        if (pj != null)
            pj.TriggerDamage(health);
        if (health <= 0)
            DestroyCharacter();
    }

    void DestroyCharacter()
    {
		
        PjMovementController pj = this.gameObject.GetComponent <PjMovementController>();
        if (pj != null)
        {
            StartCoroutine(WaitGameOver());
        }
        else
        {

            Destroy(this.gameObject);
        }
			
    }

    IEnumerator WaitGameOver()
    {

        GameObject visualGo = this.gameObject.GetChild("Visual");
        if (visualGo != null)
            visualGo.SetActive(false);
        GameObject frameGo = this.gameObject.GetChild("Frame");
        if (frameGo != null)
            frameGo.SetActive(false);
        PjAttackController pjAC = this.gameObject.GetComponent<PjAttackController>();
        if (pjAC != null)
            pjAC.enabled = false;
        PjMovementController pjMC = this.gameObject.GetComponent<PjMovementController>();
        if (pjMC != null)
            pjMC.enabled = false;

        gameOver.enabled = true;

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("intro");
        //SceneManager.LoadScene ("MenuInicio");
        //Application.Quit();

        Destroy(this.gameObject);

    }
}
