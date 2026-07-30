
namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE206ImbalancedDiet;

public enum OID : uint
{
    Algol = 0x4C4B, // R7.500, x1
    Helper = 0x233C, // R0.500, x32, Helper type
    _Gen_Algol = 0x4D87, // R6.000, x5
    _Gen_CrescentTomato = 0x4C4C, // R0.900, x4
    _Gen_CrescentOnion = 0x4C4D, // R0.900, x4
    _Gen_Actor1ec021 = 0x1EC021, // R0.500, x1, EventObj type
    _Gen_Actor1ea1a1 = 0x1EA1A1, // R2.000, x2, EventObj type
    _Gen_ = 0x4C4E, // R1.000, x0 (spawn during fight)
}

public enum AID : uint
{
    _Weaponskill_ = 48118, // 4D87->self, no cast, range ?-30 donut
    _AutoAttack_ = 50644, // Algol->player, no cast, single-target
    _Weaponskill_CursedScreech = 48100, // Algol->self, 5.0s cast, ???
    _Weaponskill_CursedScreech1 = 48971, // Helper->self, 6.0s cast, ???
    _Weaponskill_ShrillPeal = 50426, // Algol->self, 3.0s cast, ???
    _Weaponskill_ShrillPeal1 = 50427, // Helper->self, 4.0s cast, ???
    _Weaponskill_Inhale = 48101, // Algol->self, 2.0+1.0s cast, single-target
    _Weaponskill_Inhale1 = 48102, // Algol->self, no cast, single-target
    _Weaponskill_Inhale2 = 48104, // 4D87->self, 3.5s cast, range 60 30.000-degree cone
    _Weaponskill_Inhale3 = 48103, // Helper->4C4D/4C4C, 0.7s cast, single-target
    DevourShort = 50469, // Helper->self, 6.8s cast, range 8 120.000-degree cone
    _Weaponskill_Regurgitonion = 48107, // Algol->location, no cast, single-target
    RottenOnion1 = 48110, // Helper->self, 4.0s cast, range 60 30.000-degree cone
    RottenOnion2 = 48112, // Helper->self, 2.0s cast, range 60 30.000-degree cone
    _Weaponskill_Regurgitomato = 48106, // Algol->location, no cast, single-target
    RottenTomato1 = 48109, // Helper->self, 4.0s cast, range 50 width 6 rect
    RottenTomato2 = 48111, // Helper->self, 2.0s cast, range 50 width 6 rect
    SpinningInhale = 48113, // Algol->self, 5.0s cast, range 30 30.000-degree cone
    _Weaponskill_SpinningInhale1 = 48114, // 4D87->self, no cast, range ?-30 donut
    _Weaponskill_SpinningInhale2 = 50942, // 4D87->self, no cast, range ?-30 donut
    _Weaponskill_SpinningInhale3 = 48249, // Helper->self, no cast, range 7 ?-degree cone
    _Weaponskill_1 = 48115, // Algol->self, no cast, single-target
    _Weaponskill_Devour1 = 48105, // Algol->self, no cast, range 12 ?-degree cone
    DevourLong1 = 50422, // Helper->self, 3.0s cast, range 12 120.000-degree cone
    DevourLong2 = 50467, // Helper->self, 3.0s cast, range 12 120.000-degree cone
    DigestedJuice1 = 48116, // Algol->self, 4.0s cast, range 40 width 50 rect
    _Weaponskill_DigestedJuice1 = 50423, // Algol->self, no cast, single-target
    DigestedJuice2 = 50424, // Helper->self, 4.0s cast, range 40 width 50 rect
    _Weaponskill_Malady = 48117, // Algol->self, no cast, range 12 circle
    Malady = 50425, // Helper->self, 3.0s cast, range 11 circle
}

public enum SID : uint
{
    _Gen_DirectionalDisregard = 3808, // none->Algol, extra=0x0
    _Gen_Incapacitated = 5408, // none->4C4C/4C4D, extra=0x0
    _Gen_VulnerabilityUp = 2347, // Helper/4D87->player, extra=0x1/0x2/0x3/0x4/0x5/0x6/0x7/0x8/0x9/0xA/0xB/0xD/0xF
    _Gen_ = 2552, // Algol->Algol, extra=0x424
    _Gen_Stun1 = 5411, // 4D87/Helper->player, extra=0xEC7
    _Gen_Stun2 = 2656, // 4D87->player, extra=0xEC7

}

public enum IconID : uint
{
    _Gen_Icon_m0005sp_11o0t = 13, // 4C4D/4C4C->self
    _Gen_Icon_d1004turning_right_c0p = 167, // Algol->self
}

sealed class DevourShort(BossModule module) : Components.SimpleAOEs(module, (uint)AID.DevourShort, new AOEShapeCone(8f, 60f.Degrees()));
sealed class RottenOnion(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.RottenOnion1, (uint)AID.RottenOnion2], new AOEShapeCone(60f, 15f.Degrees()));
sealed class RottenTomato(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.RottenTomato1, (uint)AID.RottenTomato2], new AOEShapeRect(50f, 3f));
sealed class DevourLong(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.DevourLong1, (uint)AID.DevourLong2], new AOEShapeCone(12f, 60f.Degrees()));
sealed class DigestedJuice(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.DigestedJuice1, (uint)AID.DigestedJuice2], new AOEShapeRect(40f, 25f));
sealed class Malady(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Malady, 11f);
sealed class SpinningInhale(BossModule module) : Components.GenericAOEs(module)
{
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        return [];
    }
}

[SkipLocalsInit]
sealed class AlgolStates : StateMachineBuilder
{
    public AlgolStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<DevourShort>()
            .ActivateOnEnter<RottenOnion>()
            .ActivateOnEnter<RottenTomato>()
            .ActivateOnEnter<DevourLong>()
            .ActivateOnEnter<DigestedJuice>()
            .ActivateOnEnter<Malady>()
            .ActivateOnEnter<SpinningInhale>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(AlgolStates),
ConfigType = null, // replace null with typeof(AlgolConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = null, // replace null with typeof(TetherID) if applicable
IconIDType = typeof(IconID), // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.Algol,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14790u,
SortOrder = 6,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class Algol(WorldState ws, Actor primary) : BossModule(ws, primary, new(765f, 0f), new ArenaBoundsCircle(20f))
{
    protected override bool CheckPull() => base.CheckPull() && Raid.Player()!.Position.InCircle(Arena.Center, 20f);
}
