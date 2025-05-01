using HarmonyLib;
using UnityEngine;
using ItemActionDynamicMeleeData = ItemActionDynamicMelee.ItemActionDynamicMeleeData;


[HarmonyPatch(typeof(ItemActionDynamicMelee), "OnHoldingUpdate")]
public class H_ItemActionDynamicMelee_OnHoldingUpdate
{
    public static bool actionRunning = false;

    public static float lastUseTime = 0;

    private static bool MiddleMousePressed(ItemActionData actionData)
    {
        if (!Input.GetMouseButton(2))
            return false;

        if (Time.time - lastUseTime > 0.5f)
        {
            lastUseTime = Time.time;
            return true;
        }

        return false;
    }

    private static void ForceExecuteAction(ItemActionData actionData)
    {
        actionData.invData.holdingEntity.inventory.Execute(0, false, null);
    }

    public static void Postfix(ItemActionDynamicMelee __instance, ItemActionData _actionData)
    {
        if (_actionData.invData.holdingEntity.entityId != GameManager.Instance.World.GetPrimaryPlayerId())
            return;

        var actionDataMelee = _actionData as ItemActionDynamicMeleeData;
        bool middleMousePressed = MiddleMousePressed(_actionData);

        if (middleMousePressed && !actionRunning && !actionDataMelee.Attacking)
        {
            actionRunning = true;
        }
        else if (middleMousePressed && actionRunning)
        {
            actionRunning = false;
        }

        if (actionRunning && __instance.canStartAttack(actionDataMelee))
        {
            ForceExecuteAction(_actionData);
        }
    }

}