using UnityEngine;

public class ButtonManager : MonoBehaviour {
    public void DoRun() => WorldManager.Instance.LoadWorld();
    public void SelectWorld(string worldName) => WorldManager.Instance.SelectWorld(worldName);
    public void GoToScene(string scene) => ChangeScene.Instance.GoToScene(scene);
    public void HitSlime() => DungeonScript.Instance.HitEnemy();
    public void ToggleAutoHit() => UpgradeManager.Instance.ToggleAutoDungeon();
    public void ToggleAutoRun() => UpgradeManager.Instance.ToggleAutoRun();
    public void ForgePart() => FurnaceScript.Instance.ForgePart();
    public void ShowBlueprint(GameObject blueprint) => FurnaceScript.Instance.ShowBlueprint(blueprint);
    public void CombineMaterials() => AltarScript.Instance.CombineMaterials();
    public void CraftSword() => AnvilScript.Instance.CraftSword();
    public void ShowUpgrade(string upgradeName) => UpgradeManager.Instance.ShowUpgrade(upgradeName);
    public void PurchaseUpgrade() => UpgradeManager.Instance.PurchaseUpgrade();
    public void OpenInventory() => Inventory.Instance.OpenInventory();
    public void CloseInventory() => Inventory.Instance.CloseInventory();
    public void ShowTab(GameObject tab) => Inventory.Instance.ShowTab(tab);
    public void ShowUpgrade() => UpgradeManager.Instance.ShowUpgrade(UpgradeManager.Instance.CurrentUpgrade);
    public void ActivateMultiRun(int number) => WorldManager.Instance.ActivateMultiRun(number);
}
