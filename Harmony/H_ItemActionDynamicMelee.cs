using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Platform;
using UnityEngine;
using ItemActionDynamicMeleeData = ItemActionDynamicMelee.ItemActionDynamicMeleeData;


[HarmonyPatch(typeof(ItemActionDynamicMelee), "OnHoldingUpdate")]
public class H_ItemActionDynamicMelee_OnHoldingUpdate
{
    public static readonly HashSet<int> actionDatas = new HashSet<int>();

    public static readonly Dictionary<int, float> useTimes = new Dictionary<int, float>();

    private static bool MiddleMousePressed(ItemActionData actionData)
    {
        if (!Input.GetMouseButton(2))
            return false;

        var entityID = actionData.invData.holdingEntity.entityId;

        if (!useTimes.ContainsKey(entityID))
            useTimes[entityID] = 0;

        if (Time.time - useTimes[entityID] > 0.5f)
        {
            useTimes[entityID] = Time.time;
            return true;
        }

        return false;
    }

    private static void AddAction(ItemActionData actionData)
    {
        actionDatas.Add(actionData.invData.holdingEntity.entityId);
    }

    private static void RemoveAction(ItemActionData actionData)
    {
        actionDatas.Remove(actionData.invData.holdingEntity.entityId);
    }

    public static bool ActionExists(ItemActionData actionData)
    {
        return actionDatas.Contains(actionData.invData.holdingEntity.entityId);
    }

    private static void ForceExecuteAction(ItemActionData actionData)
    {
        actionData.invData.holdingEntity.inventory.Execute(0, false, PlatformManager.NativePlatform.Input.PrimaryPlayer);
    }

    public static void Postfix(ItemActionDynamicMelee __instance, ItemActionData _actionData)
    {
        if (_actionData.invData.holdingEntity.entityId != GameManager.Instance.World.GetPrimaryPlayerId())
            return;

        var actionDataMelee = _actionData as ItemActionDynamicMeleeData;

        bool middleMousePressed = MiddleMousePressed(_actionData);
        bool actionExists = ActionExists(_actionData);

        if (middleMousePressed && !actionExists && !actionDataMelee.Attacking)
        {
            AddAction(_actionData);
            Log.Out($"[OnHoldingUpdate] add");
        }
        else if (middleMousePressed && actionExists)
        {
            RemoveAction(_actionData);
            Log.Out($"[OnHoldingUpdate] remove");
        }

        if (ActionExists(_actionData) && __instance.canStartAttack(actionDataMelee))
        {
            ForceExecuteAction(_actionData);

            var values = string.Join(", ", actionDatas.Select(a => a.ToString()));
            Log.Out($"[OnHoldingUpdate] execute action: [{values}]");
        }
    }

}