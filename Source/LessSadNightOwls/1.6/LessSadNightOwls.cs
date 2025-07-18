using HarmonyLib;
using RimWorld;
using Verse;

namespace LessSadNightOwls;

[StaticConstructorOnStartup]
public class LessSadNightOwls
{
    private static readonly HashSet<TimeAssignmentDef> suppressedAssignments = [
        TimeAssignmentDefOf.Sleep,
        TimeAssignmentDefOf.Joy,
        TimeAssignmentDefOf.Anything];

    static LessSadNightOwls()
    {
        Harmony harmony = new("NachoToast.LessSadNightOwls");

        harmony.Patch(
            original: AccessTools.Method(
                type: typeof(ThoughtWorker_IsDayForNightOwl),
                name: "CurrentStateInternal"),
            postfix: new HarmonyMethod(
                methodType: typeof(LessSadNightOwls),
                methodName: nameof(CurrentStateInteral_Postfix)));
    }

    private static void CurrentStateInteral_Postfix(Pawn p, ref ThoughtState __result)
    {
        if (!__result.Active)
        {
            // already inactive
            return;
        }

        if (p.IsPrisoner)
        {
            // prisoner - suppress
            __result = false;
            return;
        }

        TimeAssignmentDef assignment = p.timetable?.CurrentAssignment;

        if (assignment != null && suppressedAssignments.Contains(assignment))
        {
            // suppressable assignment
            __result = false;
        }
    }
}
