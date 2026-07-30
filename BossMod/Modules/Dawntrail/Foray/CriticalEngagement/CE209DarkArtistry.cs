namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE209DarkArtistry;

public enum OID : uint
{
    PhantomNecromancer = 0x4BC1, // R4.000, x1
    Helper = 0x233C, // R0.500, x3, Helper type
    _Gen_PhantomNecromancer = 0x4C75, // R1.000, x1
    _Gen_LongDeadExplorer = 0x4BC2, // R1.000, x0 (spawn during fight)
    _Gen_LongDeadPirate = 0x4BC3, // R2.600, x0 (spawn during fight)
}

public enum AID : uint
{
    _Ability_ = 47173, // 4C75->self, no cast, ???
    _AutoAttack_ = 50761, // PhantomNecromancer->player, no cast, single-target
    DarkII = 47181, // PhantomNecromancer->self, 5.0s cast, range 50 width 50 rect
    _Ability_RiseOfTheFallen = 47174, // PhantomNecromancer->self, 3.0s cast, single-target
    ExplosionAOE = 47175, // 4BC2->self, 2.0s cast, range 8 circle
    ExplosionCross = 47176, // 4BC3->self, 4.0s cast, range 80 width 7 cross
    DarkFlare = 47182, // PhantomNecromancer->self, 5.0s cast, single-target
    _Spell_DarkFlare = 47183, // Helper->self, no cast, ???
    _Ability_ArcaneRevelation = 47179, // PhantomNecromancer->self, 3.0s cast, single-target
    Necrosurge = 47180, // Helper->self, 7.0s cast, range 70 width 12 rect
}

public enum SID : uint
{
    _Gen_ = 2056, // none->4BC2, extra=0x26B
}

sealed class DarkII(BossModule module) : Components.SimpleAOEs(module, (uint)AID.DarkII, new AOEShapeRect(50f, 25f));
sealed class ExplosionAOE(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ExplosionAOE, 8f);
sealed class ExplosionCross(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ExplosionCross, new AOEShapeCross(80f, 3.5f));
sealed class Necrosurge(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Necrosurge, new AOEShapeRect(70f, 6f));
sealed class DarkFlare(BossModule module) : Components.RaidwideCast(module, (uint)AID.DarkFlare);

[SkipLocalsInit]
sealed class PhantomNecromancerStates : StateMachineBuilder
{
    public PhantomNecromancerStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<DarkII>()
            .ActivateOnEnter<ExplosionAOE>()
            .ActivateOnEnter<ExplosionCross>()
            .ActivateOnEnter<Necrosurge>()
            .ActivateOnEnter<DarkFlare>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(PhantomNecromancerStates),
ConfigType = null, // replace null with typeof(PhantomNecromancerConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = null, // replace null with typeof(TetherID) if applicable
IconIDType = null, // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.PhantomNecromancer,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14512u,
SortOrder = 9,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class PhantomNecromancer(WorldState ws, Actor primary) : BossModule(ws, primary, new(224f, -860f), new ArenaBoundsSquare(20f))
{
    protected override bool CheckPull() => base.CheckPull() && Raid.Player()!.Position.InSquare(Arena.Center, 20f);
}
