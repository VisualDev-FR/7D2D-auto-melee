using HarmonyLib;
using UnityEngine;

using ItemActionDynamicMeleeData = ItemActionDynamicMelee.ItemActionDynamicMeleeData;


[HarmonyPatch(typeof(ItemActionDynamicMelee), "OnHoldingUpdate")]
public class H_ItemActionDynamicMelee_OnHoldingUpdate
{
    public static bool autoMelee = false;

    public static float lastUseTime = 0;

    public static void Postfix(ItemActionDynamicMelee __instance, ItemActionData _actionData)
    {
        if (_actionData.invData.holdingEntity.entityId != GameManager.Instance.World.GetPrimaryPlayerId())
            return;

        bool middleMousePressed = Input.GetKey(Config.autoMeleeKey);
        bool middleMouseDelayOK = Time.time - lastUseTime > 0.5f;

        if (middleMousePressed && middleMouseDelayOK)
        {
            autoMelee = !autoMelee;
            lastUseTime = Time.time;
        }

        if (autoMelee && __instance.canStartAttack(_actionData as ItemActionDynamicMeleeData))
        {
            _actionData.invData.holdingEntity.inventory.Execute(0, false, null);
        }
    }

}