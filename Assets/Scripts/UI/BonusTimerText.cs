using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
[RequireComponent(typeof(Animator))]
public class BonusTimerText : MonoBehaviour
{
    public TMP_Text Text { get; private set; }
    private Animator animator;

    private void Awake()
    {
        Text = GetComponent<TMP_Text>();
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        gameObject.SetActive(true);
        animator.Play("Text Fading", 0, 0);
    }
}
