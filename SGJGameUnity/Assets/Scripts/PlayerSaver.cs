using UnityEngine;

public class PlayerSaver : Saver
{
    protected override string SetKey()
    {
        return "Player" + uniqueIdentifier;
    }


    protected override void Save()
    {
        var playerController = gameObject.GetComponent<PlayerSoldierController>();
        var playerHitable = gameObject.GetComponent<PlayerHitable>();
        saveData.Save(key + "-health", playerHitable.health);
        saveData.Save(key + "-pistol-mag", playerController.weaponMagazineFills[1]);
        saveData.Save(key + "-pistol-owned", playerController.weaponBulletsOwned[1]);
        saveData.Save(key + "-rifle-mag", playerController.weaponMagazineFills[2]);
        saveData.Save(key + "-rifle-owned", playerController.weaponBulletsOwned[2]);
        saveData.Save(key + "-shotgun-mag", playerController.weaponMagazineFills[3]);
        saveData.Save(key + "-shotgun-owned", playerController.weaponBulletsOwned[3]);
    }


    protected override void Load()
    {
        var playerController = gameObject.GetComponent<PlayerSoldierController>();
        var playerHitable = gameObject.GetComponent<PlayerHitable>();
        saveData.Load(key + "-health", ref playerHitable.health);
        saveData.Load(key + "-pistol-mag", ref playerController.weaponMagazineFills[1]);
        saveData.Load(key + "-pistol-owned", ref playerController.weaponBulletsOwned[1]);
        saveData.Load(key + "-rifle-mag", ref playerController.weaponMagazineFills[2]);
        saveData.Load(key + "-rifle-owned", ref playerController.weaponBulletsOwned[2]);
        saveData.Load(key + "-shotgun-mag", ref playerController.weaponMagazineFills[3]);
        saveData.Load(key + "-shotgun-owned", ref playerController.weaponBulletsOwned[3]);
    }
}
