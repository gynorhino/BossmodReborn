namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE212DoubledTrouble;

public enum OID : uint
{
    ConjuredCalofisteri = 0x4BB8, // R5.500, x1
    Helper = 0x233C, // R0.500, x16, Helper type
    Entanglement = 0x4BB9, // R4.440, x0 (spawn during fight)
    _Gen_LitheLock = 0x4BBA, // R1.000, x0 (spawn during fight)
    _Gen_ = 0x4BBB, // R1.000, x0 (spawn during fight)
    _Gen_1 = 0x4BBC, // R1.000, x0 (spawn during fight)
}

public enum AID : uint
{
    _AutoAttack_Attack = 50122, // ConjuredCalofisteri->player, no cast, single-target
    AuraBurst = 47079, // ConjuredCalofisteri->self, 5.0s cast, single-target
    _Ability_AuraBurst1 = 47080, // Helper->self, no cast, ???
    _Ability_AsymmetricCoifChange = 47054, // ConjuredCalofisteri->self, 3.0s cast, single-target
    _Ability_CoifChange = 47057, // ConjuredCalofisteri->self, no cast, single-target
    _Weaponskill_DualCut = 47058, // ConjuredCalofisteri->self, 2.0s cast, single-target
    DualCut1 = 50691, // Helper->self, 2.8s cast, range 60 ?-degree cone
    _Weaponskill_DualCut2 = 47061, // ConjuredCalofisteri->self, no cast, single-target
    DualCut2 = 50692, // Helper->self, 4.8s cast, range 60 ?-degree cone
    _Ability_ResettingSpray = 47062, // ConjuredCalofisteri->self, no cast, single-target
    _Ability_ResettingSpray1 = 47065, // ConjuredCalofisteri->self, no cast, single-target
    _Ability_Extension = 47069, // ConjuredCalofisteri->self, 3.0s cast, single-target
    Graft = 47070, // 4BBA->self, 3.0s cast, range 6 circle
    _Ability_BalefulBlowout = 47071, // ConjuredCalofisteri->self, 5.0s cast, single-target
    MaliciousWeave1 = 47072, // 4BB9->self, 5.5s cast, range 6 circle
    _Ability_Garrote = 47074, // 4BB9->self, no cast, single-target
    _Ability_ = 47066, // 4BBC->location, no cast, single-target
    _Ability_DashingCut = 47067, // ConjuredCalofisteri->location, 6.0s cast, single-target
    DashingCut1 = 49052, // Helper->location, 6.5s cast, width 10 rect charge
    _Ability_DashingCut2 = 47068, // ConjuredCalofisteri->location, 0.5s cast, single-target
    DashingCut2 = 49053, // Helper->location, 1.0s cast, width 10 rect charge
    _Ability_HairShears = 47075, // ConjuredCalofisteri->self, 5.0s cast, single-target
    HairShears1 = 47076, // Helper->self, 5.0s cast, range 10 circle
    HairShears2 = 47077, // Helper->self, 5.0s cast, range 60 width 4 cross
    HairShears3 = 47599, // Helper->self, no cast, range 60 width 4 cross
    MaliciousWeave2 = 47078, // 4BB9->self, 1.0s cast, range 6 circle
    _Ability_AsymmetricCoifChange1 = 47055, // ConjuredCalofisteri->self, 3.0s cast, single-target
    _Ability_CoifChange1 = 47056, // ConjuredCalofisteri->self, no cast, single-target
    _Weaponskill_DualCut4 = 47059, // ConjuredCalofisteri->self, 2.0s cast, single-target
    _Weaponskill_DualCut5 = 47060, // ConjuredCalofisteri->self, no cast, single-target
    _Ability_ResettingSpray2 = 47063, // ConjuredCalofisteri->self, no cast, single-target
    _Ability_ResettingSpray3 = 47064, // ConjuredCalofisteri->self, no cast, single-target
    _Ability_Garrote1 = 47073, // 4BB9->self, 10.0s cast, range 6 circle
}

public enum SID : uint
{
    _Gen_Fetters = 5349, // 4BB9->player, extra=0xEC4
}

sealed class AuraBurst(BossModule module) : Components.RaidwideCast(module, (uint)AID.AuraBurst);
sealed class DualCut(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.DualCut1, (uint)AID.DualCut2], new AOEShapeCone(60f, 90f.Degrees()), 2);
sealed class Graft(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Graft, 6f);
sealed class MaliciousWeave1(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MaliciousWeave1, 6f);
sealed class MaliciousWeave2(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MaliciousWeave2, 6f);
sealed class DashingCut1(BossModule module) : Components.ChargeAOEs(module, (uint)AID.DashingCut1, 5f); // turn into single component
sealed class DashingCut2(BossModule module) : Components.ChargeAOEs(module, (uint)AID.DashingCut2, 5f);
sealed class HairShears1(BossModule module) : Components.SimpleAOEs(module, (uint)AID.HairShears1, 10f);
sealed class HairShears2(BossModule module) : Components.SimpleAOEs(module, (uint)AID.HairShears2, new AOEShapeCross(60f, 2f)); // when is 3 cast?
sealed class Entanglement(BossModule module) : Components.Adds(module, (uint)OID.Entanglement);

[SkipLocalsInit]
sealed class ConjuredCalofisteriStates : StateMachineBuilder
{
    public ConjuredCalofisteriStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<AuraBurst>()
            .ActivateOnEnter<DualCut>()
            .ActivateOnEnter<Graft>()
            .ActivateOnEnter<MaliciousWeave1>()
            .ActivateOnEnter<MaliciousWeave2>()
            .ActivateOnEnter<DashingCut1>()
            .ActivateOnEnter<DashingCut2>()
            .ActivateOnEnter<HairShears1>()
            .ActivateOnEnter<HairShears2>()
            .ActivateOnEnter<Entanglement>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(ConjuredCalofisteriStates),
ConfigType = null, // replace null with typeof(ConjuredCalofisteriConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = null, // replace null with typeof(TetherID) if applicable
IconIDType = null, // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.ConjuredCalofisteri,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14517u,
SortOrder = 2,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class ConjuredCalofisteri(WorldState ws, Actor primary) : BossModule(ws, primary, new(-215f, -70f), new ArenaBoundsCircle(20f))
{
    protected override bool CheckPull() => base.CheckPull() && Raid.Player()!.Position.InCircle(Arena.Center, 20f);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies((uint)OID.Entanglement));
    }
}
