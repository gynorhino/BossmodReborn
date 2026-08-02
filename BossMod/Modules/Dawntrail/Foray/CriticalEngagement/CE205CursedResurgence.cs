
namespace BossMod.Dawntrail.Foray.CriticalEngagement.CE205CursedResurgence;

public enum OID : uint
{
    ClaretDragon = 0x4C46, // R5.000, x?
    Helper = 0x233C, // R0.500, x?, Helper type
    _Gen_ClaretDragon2 = 0x4D25, // R1.000, x?
    _Gen_Actor1ec094 = 0x1EC094, // R0.500, x?, EventObj type
    _Gen_Actor1ec093 = 0x1EC093, // R0.500, x?, EventObj type
    _Gen_Actor1ec096 = 0x1EC096, // R0.500, x?, EventObj type
    _Gen_Necrohaze = 0x4C47, // R1.500, x?
    _Gen_Actor1ec095 = 0x1EC095, // R0.500, x?, EventObj type
    AetherialWard = 0x4C48, // R7.000, x?
}

public enum AID : uint
{
    _Ability_ = 48279, // 4D25->self, no cast, ???
    _AutoAttack_ = 48259, // ClaretDragon->player, no cast, single-target
    HowlingDarkness = 48277, // ClaretDragon->self, 5.0s cast, single-target
    _Spell_HowlingDarkness1 = 48278, // Helper->self, no cast, ???
    SnakingNecrobreath = 48260, // ClaretDragon->self, 6.0s cast, range 60 270.000-degree cone
    _Spell_GraveMold = 48261, // ClaretDragon->self, 5.0s cast, single-target
    GraveMold = 48262, // Helper->self, 6.0s cast, range 8 circle
    _Ability_Necrohaze = 48263, // 4C47->self, no cast, range 5 circle
    _Ability_Soar = 50488, // ClaretDragon->self, 4.0s cast, single-target
    _Ability_1 = 48302, // ClaretDragon->self, no cast, single-target
    _Weaponskill_Cauterize = 48264, // ClaretDragon->self, 6.0s cast, single-target
    Cauterize = 48265, // Helper->self, 7.0s cast, range 40 width 10 rect
    Catching = 48267, // 4C47->self, no cast, range 30 width 10 rect
    _Weaponskill_ = 48266, // ClaretDragon->self, no cast, single-target
    _Ability_AetherialWard = 48271, // ClaretDragon->self, 4.0+0.5s cast, single-target
    _Spell_Necrohaze = 50484, // Helper->self, 4.0s cast, range 5 circle
    _Ability_2 = 48275, // ClaretDragon->self, no cast, single-target
    _Ability_Necrohaze1 = 48269, // Helper->self, no cast, range 5 circle
    _Ability_Necrohaze2 = 48268, // Helper->location, no cast, range 5 circle
    _Ability_3 = 48276, // ClaretDragon->self, no cast, single-target
    BreathInThrees1 = 48270, // ClaretDragon->self, 5.0s cast, range 60 120.000-degree cone
    BreathInThrees2 = 48248, // ClaretDragon->self, 2.5s cast, range 60 120.000-degree cone
}

public enum SID : uint
{
    _Gen_GradualZombification = 5059, // Helper/4C47/ClaretDragon->player, extra=0x1
    _Gen_ZombieProof = 5138, // Helper/4C47->player, extra=0x0
    _Gen_Zombification = 2305, // 4C47/Helper->player, extra=0x0
    _Gen_ = 2056, // ClaretDragon->ClaretDragon, extra=0x164
    _Gen_Heavy = 1796, // none->4C47, extra=0x32
    _Gen_DirectionalInvincibility = 1125, // none->4C48, extra=0x0
}

sealed class HowlingDarkness(BossModule module) : Components.RaidwideCast(module, (uint)AID.HowlingDarkness);
sealed class SnakingNecrobreath(BossModule module) : Components.SimpleAOEs(module, (uint)AID.SnakingNecrobreath, new AOEShapeCone(60f, 135f.Degrees()));
sealed class GraveMold(BossModule module) : Components.SimpleAOEs(module, (uint)AID.GraveMold, 8f);
sealed class Cauterize(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Cauterize, new AOEShapeRect(40f, 5f));
sealed class Catching(BossModule module) : Components.GenericAOEs(module)
{
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        return [];
    }
}
sealed class BreathInThrees(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.BreathInThrees1, (uint)AID.BreathInThrees2], new AOEShapeCone(60f, 60f.Degrees()));

[SkipLocalsInit]
sealed class ClaretDragonStates : StateMachineBuilder
{
    public ClaretDragonStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<HowlingDarkness>()
            .ActivateOnEnter<SnakingNecrobreath>()
            .ActivateOnEnter<GraveMold>()
            .ActivateOnEnter<Cauterize>()
            .ActivateOnEnter<Catching>()
            .ActivateOnEnter<BreathInThrees>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(ClaretDragonStates),
ConfigType = null, // replace null with typeof(ClaretDragonConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = null, // replace null with typeof(TetherID) if applicable
IconIDType = null, // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.ClaretDragon,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14787u,
SortOrder = 5,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class ClaretDragon(WorldState ws, Actor primary) : BossModule(ws, primary, new(-688f, 150f), new ArenaBoundsSquare(20f));
