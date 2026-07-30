
namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE215AcceptNoImitators;

public enum OID : uint
{
    Metamorph = 0x4C77, // R3.000-6.000, x1
    Helper = 0x233C, // R0.500, x39, Helper type
    _Gen_Actor1ea1a1 = 0x1EA1A1, // R2.000, x2, EventObj type
    _Gen_Actor1ec09a = 0x1EC09A, // R0.500, x0 (spawn during fight), EventObj type
    _Gen_Actor1ec09c = 0x1EC09C, // R0.500, x0 (spawn during fight), EventObj type
    _Gen_Actor1ec09b = 0x1EC09B, // R0.500, x0 (spawn during fight), EventObj type
}

public enum AID : uint
{
    _AutoAttack_ = 48334, // Metamorph->player, no cast, single-target
    _Spell_BlackenedRain = 48335, // Metamorph->self, 4.0+1.0s cast, single-target
    _Spell_BlackenedRain1 = 48336, // Helper->self, 5.0s cast, ???
    _Spell_ = 48367, // 4DFD->self, no cast, range ?-30 donut
    _Ability_Change = 48339, // Metamorph->self, 4.0s cast, single-target
    CyclonicRing = 48354, // Metamorph->self, 4.0s cast, range ?-30 donut
    _Spell_ShapeshiftingSupercell = 48355, // Metamorph->self, 5.5+0.5s cast, single-target
    ShapeshiftingSupercellCone1 = 48357, // Helper->self, 6.0s cast, range 60 90.000-degree cone
    ShapeshiftingSupercellAOE1 = 50767, // Helper->self, 6.0s cast, range 8 circle
    _Spell_ShapeshiftingSupercell3 = 48358, // Metamorph->self, no cast, single-target
    ShapeshiftingSupercellCone2 = 48359, // Helper->self, 1.5s cast, range 60 90.000-degree cone
    ShapeshiftingSupercellAOE2 = 48360, // Helper->self, 6.0s cast, range 8 circle
    ShapeshiftingSupercellDonut1 = 48361, // Helper->self, 6.0s cast, range 10-16 donut
    ShapeshiftingSupercellDonut2 = 48362, // Helper->self, 6.0s cast, range ?-30 donut
    _Spell_MadeMagic = 48363, // Metamorph->self, 4.0s cast, single-target
    _Spell_MadeMagic1 = 48364, // Helper->self, no cast, range 0 circle
    _Spell_CycloneCrossing = 48365, // Metamorph->self, 10.5+1.0s cast, single-target
    CycloneCrossing = 48366, // Helper->self, 11.5s cast, range 60 width 16 cross
    _AutoAttack_1 = 48369, // Metamorph->player, no cast, single-target
    _Ability_Revert = 48340, // Metamorph->self, no cast, single-target
    _Spell_DarkDealing = 48337, // Metamorph->player, 5.0s cast, single-target
    _Ability_Change1 = 48338, // Metamorph->self, 4.0s cast, single-target
    TongueOfFlame = 48341, // Metamorph->self, 4.0s cast, range 15 circle
    _Ability_HellfireFetch = 48342, // Metamorph->self, no cast, single-target
    _Weaponskill_HellwardBound = 48343, // Metamorph->location, 6.0s cast, width 10 rect charge
    _Weaponskill_HellwardBound1 = 48344, // Metamorph->location, no cast, width 10 rect charge
    HellfireFetch = 48345, // Helper->location, 7.0s cast, range 6 circle
    _AutoAttack_2 = 48368, // Metamorph->player, no cast, single-target
    _Weaponskill_ = 50720, // Metamorph->location, no cast, single-target
    _Weaponskill_1 = 48347, // Helper->self, 2.0s cast, range 60 ?-degree cone
    _Weaponskill_2 = 48348, // Helper->self, 4.0s cast, range 60 ?-degree cone
    _Weaponskill_3 = 48349, // Helper->self, 6.0s cast, range 60 ?-degree cone
    HellishBreathCast = 48346, // Metamorph->self, 6.0s cast, single-target
    _Weaponskill_HellishBreath1 = 48351, // Metamorph->self, no cast, single-target
    HellishBreathVisual2 = 48663, // Helper->self, 1.1s cast, range 60 ?-degree cone
    _Weaponskill_HellishBreath3 = 48352, // Metamorph->self, no cast, single-target
    HellishBreathVisual3 = 50677, // Helper->self, 1.1s cast, range 60 ?-degree cone
    _Weaponskill_HellishBreath5 = 48350, // Metamorph->self, no cast, single-target
    HellishBreathVisual1 = 48662, // Helper->self, 1.1s cast, range 60 ?-degree cone
    _Weaponskill_4 = 48353, // Metamorph->self, no cast, single-target
}

sealed class CyclonicRing(BossModule module) : Components.SimpleAOEs(module, (uint)AID.CyclonicRing, new AOEShapeDonut(10f, 30f)); //verify inner
sealed class ShapeshiftingSupercellCone(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.ShapeshiftingSupercellCone1, (uint)AID.ShapeshiftingSupercellCone2], new AOEShapeCone(60f, 45f.Degrees()));
sealed class ShapeshiftingSupercellAOE(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.ShapeshiftingSupercellAOE1, (uint)AID.ShapeshiftingSupercellAOE2], 8f);
sealed class ShapeshiftingSupercellDonut1(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ShapeshiftingSupercellDonut1, new AOEShapeDonut(10f, 16f));
sealed class ShapeshiftingSupercellDonut2(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ShapeshiftingSupercellDonut1, new AOEShapeDonut(16f, 30f)); // verify inner
sealed class CycloneCrossing(BossModule module) : Components.SimpleAOEs(module, (uint)AID.CycloneCrossing, new AOEShapeCross(60f, 8f));
sealed class TongueOfFlame(BossModule module) : Components.SimpleAOEs(module, (uint)AID.TongueOfFlame, 15f);
sealed class HellfireFetch(BossModule module) : Components.SimpleAOEs(module, (uint)AID.HellfireFetch, 6f);
sealed class HellwardBound(BossModule module) : Components.GenericAOEs(module)
{
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        return [];
    }
}
/*
sealed class HellishBreath(BossModule module) : Components.GenericAOEs(module)
{
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        return [];
    }
}
*/
//sealed class DebugVisual(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.HellishBreathVisual1, (uint)AID.HellishBreathVisual2, (uint)AID.HellishBreathVisual3], new AOEShapeCone(60f, 30f.Degrees()));
sealed class HellishBreath(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID._Weaponskill_1, (uint)AID._Weaponskill_2, (uint)AID._Weaponskill_3], new AOEShapeCone(60f, 30f.Degrees()), 2);

public enum SID : uint
{
    _Gen_Transfiguration = 2548, // Metamorph->Metamorph, extra=0x174/0x173
    _Gen_VulnerabilityUp = 2347, // Metamorph/Helper->player, extra=0x1/0x2/0x3/0x4/0x5/0x6/0x7/0x8/0x9/0xA/0xB/0xC/0xD
    _Gen_AreaOfInfluenceUp = 1909, // none->Helper, extra=0x1/0x2/0x3/0x4/0x5/0x6/0x7
    _Gen_DirectionalDisregard = 3808, // none->Metamorph, extra=0x0
}

public enum IconID : uint
{
    _Gen_Icon_x6r3_turning_right_c0e1 = 546, // Metamorph->self
    _Gen_Icon_tank_lockon01i = 198, // player->self
}

[SkipLocalsInit]
sealed class MetamorphStates : StateMachineBuilder
{
    public MetamorphStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<CyclonicRing>()
            .ActivateOnEnter<ShapeshiftingSupercellCone>()
            .ActivateOnEnter<ShapeshiftingSupercellAOE>()
            .ActivateOnEnter<ShapeshiftingSupercellDonut1>()
            .ActivateOnEnter<ShapeshiftingSupercellDonut2>()
            .ActivateOnEnter<CycloneCrossing>()
            .ActivateOnEnter<TongueOfFlame>()
            .ActivateOnEnter<HellfireFetch>()
            .ActivateOnEnter<HellwardBound>()
            .ActivateOnEnter<HellishBreath>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(MetamorphStates),
ConfigType = null, // replace null with typeof(MetamorphConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = null, // replace null with typeof(TetherID) if applicable
IconIDType = typeof(IconID), // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.Metamorph,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14801u,
SortOrder = 15,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class Metamorph(WorldState ws, Actor primary) : BossModule(ws, primary, new(500f, -310f), new ArenaBoundsCircle(20f))
{
    protected override bool CheckPull() => base.CheckPull() && Raid.Player()!.Position.InCircle(Arena.Center, 20f);
}
