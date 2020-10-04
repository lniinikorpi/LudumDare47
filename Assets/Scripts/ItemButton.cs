using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType
{
    speed,
    health,
    damage,
    clipSize,
    reloadSpeed,
    staminaMax,
    staminaRecharge
}

public class ItemButton : MonoBehaviour
{
    public int itemCost = 2;
    public UpgradeType upgradeType;
    public Image upgradeImage;

    public Sprite speedSprite;
    public Sprite healthSprite;
    public Sprite damageSprite;
    public Sprite clipSizeSprite;
    public Sprite reloadSpeedSprite;
    public Sprite staminaRechargeSprite;
    public Sprite staminaMaxSprite;
    string upgradeInfo;

    public Button button;
    public GameObject noMoneyText;
    public TMP_Text upgradeInfoText;
    public AudioSource audioSource;

    public GameManager gm;
    public BuyButton buyButton;

    Player player;

    private void Start()
    {
        gm = GameManager.instance;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void NewItems()
    {
        upgradeInfoText.text = "";
        button.interactable = true;
        noMoneyText.SetActive(false);
        int index = Random.Range(0, 7);
        switch (index)
        {
            case 0:
                upgradeType = UpgradeType.speed;
                upgradeImage.sprite = speedSprite;
                upgradeInfo = "Speed upgrade";
                break;
            case 1:
                upgradeType = UpgradeType.health;
                upgradeImage.sprite = healthSprite;
                upgradeInfo = "Health upgrade";
                break;
            case 2:
                upgradeType = UpgradeType.damage;
                upgradeImage.sprite = damageSprite;
                upgradeInfo = "Damage upgrade";
                break;
            case 3:
                upgradeType = UpgradeType.clipSize;
                upgradeImage.sprite = clipSizeSprite;
                upgradeInfo = "Clip size upgrade";
                break;
            case 4:
                upgradeType = UpgradeType.reloadSpeed;
                upgradeImage.sprite = reloadSpeedSprite;
                upgradeInfo = "Reload speed upgrade";
                break;
            case 5:
                upgradeType = UpgradeType.staminaMax;
                upgradeImage.sprite = staminaMaxSprite;
                upgradeInfo = "Max stamina upgrade";
                break;
            case 6:
                upgradeType = UpgradeType.staminaRecharge;
                upgradeImage.sprite = staminaRechargeSprite;
                upgradeInfo = "Stamina recharge rate upgrade";
                break;
        }
    }

    public void CheckItem()
    {
        upgradeInfoText.text = upgradeInfo;
        buyButton.itemButton = this;
    }

    public void BuyItem()
    {
        if(gm.scoreMultiplier >= itemCost + 1)
        {
            gm.scoreMultiplier -= itemCost;
            gm.UpdateMultiplierText();

            switch (upgradeType)
            {
                case UpgradeType.speed:
                    player.playerMovement.speed += gm.speedUpgrade;
                    break;
                case UpgradeType.health:
                    player.maxHealth += (int)gm.healthUpgrade;
                    player._currentHealth += (int)gm.healthUpgrade;
                    gm.UpdateHealthBar((float)player._currentHealth / (float)player.maxHealth);
                    break;
                case UpgradeType.damage:
                    player.playerShoot.bulletDamage += gm.damageUpgrade;
                    break;
                case UpgradeType.clipSize:
                    player.playerShoot.maxClipCount += (int)gm.clipSizeUpgrade;
                    player.playerShoot._currentClipCount += (int)gm.clipSizeUpgrade;
                    gm.UpdateBulletCountText(player.playerShoot._currentClipCount);
                    break;
                case UpgradeType.reloadSpeed:
                    player.playerShoot.reloadTime -= gm.reloadSpeedUpgrade;
                    break;
                case UpgradeType.staminaMax:
                    player.playerMovement.maxStamina += gm.maxStaminaUpgrade;
                    break;
                case UpgradeType.staminaRecharge:
                    player.playerMovement.staminaRechargeRate += gm.staminaRechargeUpgrade;
                    break;
            }
            button.interactable = false;
            noMoneyText.SetActive(false);
            audioSource.Play();
        }
        else
        {
            noMoneyText.SetActive(true);
        }
    }
}
