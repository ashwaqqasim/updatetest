using UnityEngine;

public class AIHandController : MonoBehaviour
{
    public Animator animator;

    private int aiChoice = -1;

    public int PlayRandomChoice()
    {
        int aiChoice = Random.Range(0, 3); // 0: حجر, 1: ورقة, 2: مقص
        animator.SetFloat("Blend", aiChoice);
        animator.SetBool("StartPlay", true);
        return aiChoice;
    }


    public int GetAIChoice()
    {
        return aiChoice;
    }

    public void ResetAIHand()
    {
        if (animator == null)
        {
            Debug.LogError("❌ Animator غير مربوط في AIHandController!");
            return;
        }

        animator.Rebind(); // يرجّع اليد للوضع الأساسي
        animator.Update(0f);
    }

    public void PlayChoice(int choice)
    {
        if (animator == null)
        {
            Debug.LogError("❌ Animator غير مربوط!");
            return;
        }

        animator.SetFloat("Blend", choice);
        animator.SetBool("StartPlay", true);
    }

    public void PlayChoiceRandomOnlyAnimation()
    {
        int random = UnityEngine.Random.Range(0, 3);
        PlayChoice(random); // شغل الأنيميشن فقط بدون تحديد الفائز
    }

    private int lastChoice;

    public int PlayRandomChoiceOnlyAnimation()
    {
        lastChoice = UnityEngine.Random.Range(0, 3);
        PlayChoice(lastChoice); // أنيميشن فقط
        return lastChoice;
    }

    public int GetLastPlayedChoice()
    {
        return lastChoice;
    }


}

