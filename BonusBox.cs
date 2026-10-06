using UnityEngine;

public class BonusBox : MonoBehaviour
{
    public enum BonusType
    {
        Speed,
        DoubleShot,
        Shield,
        Health
    }

    public BonusType bonusType;

    public int healthAmount = 20;

    private bool alreadyHit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyHit)
            return;

        if (!other.CompareTag("Projectile"))
            return;

        alreadyHit = true;

        ApplyBonus();

        Destroy(other.gameObject);
        Destroy(gameObject);
    }

    void ApplyBonus()
    {
        if (GameManager.Instance == null)
            return;

        switch (bonusType)
        {
            case BonusType.Speed:

                GameManager.Instance.ActivateSpeedBonus();

                GameManager.Instance.ShowBonusText(
                    "VELOCIDADE!"
                );

                break;

            case BonusType.DoubleShot:

                GameManager.Instance.ActivateDoubleShotBonus();

                GameManager.Instance.ShowBonusText(
                    "TIRO DUPLO!"
                );

                break;

            case BonusType.Shield:

                GameManager.Instance.ActivateShieldBonus();

                GameManager.Instance.ShowBonusText(
                    "ESCUDO!"
                );

                break;

            case BonusType.Health:

                GameManager.Instance.HealPlayer(healthAmount);

                GameManager.Instance.ShowBonusText(
                    "VIDA RECUPERADA!"
                );

                break;
        }
    }
}