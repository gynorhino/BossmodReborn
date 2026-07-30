namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE214AheadOfTheCompetition;

public enum OID : uint
{
    PhantomHydra = 0x4BC5, // R4.800, x1
    Helper = 0x233C, // R0.500, x36, Helper type
    BallOfLevin = 0x4BC9, // R2.300, x3
    SwirlingOrb = 0x4BC8, // R0.500, x3
    BallOfFire = 0x4BC7, // R1.500, x12
    HolySphere = 0x4BC6, // R1.200, x2
}

public enum AID : uint
{
    _AutoAttack_ = 50759, // PhantomHydra->player, no cast, single-target
    NighDrawnEruption = 47197, // PhantomHydra->self, 5.0+2.0s cast, single-target
    ElementalCascade1 = 47199, // Helper->location, 7.0s cast, range 8 circle
    ElementalCascade2 = 47200, // Helper->location, 7.0s cast, range 8 circle
    ElementalCascade3 = 47201, // Helper->location, 7.0s cast, range 8 circle
    ElementalCascade4 = 47202, // Helper->location, 7.0s cast, range 8 circle
    ElementalCascade5 = 47203, // Helper->location, 7.0s cast, range 8 circle
    FarFlungEruption = 47198, // PhantomHydra->self, 5.0+2.0s cast, single-target
    _Ability_ElementalCascade = 47184, // PhantomHydra->self, 3.0s cast, single-target
    ElementalCascadeShort1 = 47185, // Helper->location, 3.0s cast, range 6 circle
    ElementalCascadeShort2 = 47186, // Helper->location, 3.0s cast, range 6 circle
    ElementalCascadeShort3 = 47187, // Helper->location, 3.0s cast, range 6 circle
    ElementalCascadeShort4 = 47188, // Helper->location, 3.0s cast, range 6 circle
    ElementalCascadeShort5 = 47189, // Helper->location, 3.0s cast, range 6 circle
    _Spell_Dissipate = 47193, // Helper->self, no cast, range 1 circle
    ScarletThread = 47190, // 4BC7->self, 3.0s cast, range 70 width 4 rect
    Shock = 47194, // Helper->location, 4.0s cast, range 10 circle
    StunningSheen = 47191, // 4BC6->self, 5.0s cast, range 40 circle
    LevinRing1 = 47195, // Helper->location, 7.0s cast, range 10-20 donut
    LevinRing2 = 47196, // Helper->location, 10.0s cast, range 20-30 donut
    IceBurst = 47192, // Helper->self, 3.0s cast, range 40 20.000-degree cone
    _Ability_Discordance = 47209, // PhantomHydra->self, 5.0s cast, single-target
    _Ability_Discordance1 = 47210, // Helper->self, no cast, ???
    _Ability_RadiantBreath = 47208, // PhantomHydra->self, no cast, single-target
    ManyHeadedBreathVisual = 47212, // Helper->self, 1.0s cast, range 30 120.000-degree cone
    ManyHeadedBreathCast = 47213, // PhantomHydra->self, 8.0s cast, single-target
    _Ability_ManyHeadedBreath4 = 47205, // PhantomHydra->self, no cast, ???
    _Ability_ManyHeadedBreath5 = 47206, // PhantomHydra->self, no cast, ???
    _Ability_ManyHeadedBreath6 = 47207, // PhantomHydra->self, no cast, ???
    ManyHeadedBreath1 = 50673, // Helper->self, 0.8s cast, range 30 ?-degree cone
    ManyHeadedBreath2 = 50674, // Helper->self, 0.8s cast, range 30 ?-degree cone
    ManyHeadedBreath3 = 50675, // Helper->self, 0.8s cast, range 30 ?-degree cone
}

public enum SID : uint
{
}

public enum IconID : uint
{
}

public enum TetherID : uint
{
}

// need to add growing poison voidzone for one of the hits
sealed class ElementalCascade(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.ElementalCascade1, (uint)AID.ElementalCascade2, (uint)AID.ElementalCascade3, (uint)AID.ElementalCascade4, (uint)AID.ElementalCascade5], 8f);
sealed class ElementalCascadeShort(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.ElementalCascadeShort1, (uint)AID.ElementalCascadeShort2, (uint)AID.ElementalCascadeShort3, (uint)AID.ElementalCascadeShort4, (uint)AID.ElementalCascadeShort5], 6f);
sealed class ScarletThread(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ScarletThread, new AOEShapeRect(70f, 2f));
sealed class Shock(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Shock, 10f);
sealed class StunningSheen(BossModule module) : Components.CastGaze(module, (uint)AID.StunningSheen, range: 40f);
sealed class IceBurst(BossModule module) : Components.SimpleAOEs(module, (uint)AID.IceBurst, new AOEShapeCone(40f, 10f.Degrees()));
sealed class LevinRing(BossModule module) : Components.ConcentricAOEs(module, _shapes)
{
    private static readonly AOEShape[] _shapes = [new AOEShapeDonut(10f, 20f), new AOEShapeDonut(20f, 30f)];

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.LevinRing1)
        {
            AddSequence(spell.LocXZ, Module.CastFinishAt(spell));
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (Sequences.Count != 0)
        {
            var order = spell.Action.ID switch
            {
                (uint)AID.LevinRing1 => 0,
                (uint)AID.LevinRing2 => 1,
                _ => -1
            };
            AdvanceSequence(order, spell.LocXZ, WorldState.FutureTime(2d));
        }
    }
}
sealed class ManyHeadedBreath(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [with(6)];
    private static readonly Angle angle = 120f.Degrees();
    private static readonly AOEShapeCone cone = new(30f, 60f.Degrees());

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var max = count > 2 ? 2 : count;
        var aoes = CollectionsMarshal.AsSpan(_aoes);
        if (count > 1)
        {
            ref var aoe0 = ref aoes[0];
            aoe0.Color = Colors.Danger;
        }
        return aoes[..max];
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID != (uint)AID.ManyHeadedBreathVisual)
            return;

        void AddAOE(Angle offset = default) => _aoes.Add(new(cone, Module.PrimaryActor.Position, Module.PrimaryActor.Rotation + offset, WorldState.FutureTime(8d)));

        var rotation = caster.Rotation;
        if (rotation.AlmostEqual(180f.Degrees(), 0.1f))
        {
            AddAOE();
        }
        else if (rotation.AlmostEqual(60f.Degrees(), 0.1f))
        {
            AddAOE(-angle);
        }
        else if (rotation.AlmostEqual(-60f.Degrees(), 0.1f))
        {
            AddAOE(angle);
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count != 0)
        {
            switch (spell.Action.ID)
            {
                case (uint)AID.ManyHeadedBreath1:
                case (uint)AID.ManyHeadedBreath2:
                case (uint)AID.ManyHeadedBreath3:
                    _aoes.RemoveAt(0);
                    break;
            }
        }
    }
}

sealed class Visual(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ManyHeadedBreathVisual, new AOEShapeCone(30f, 60f.Degrees()));

[SkipLocalsInit]
sealed class PhantomHydraStates : StateMachineBuilder
{
    public PhantomHydraStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ElementalCascade>()
            .ActivateOnEnter<ElementalCascadeShort>()
            .ActivateOnEnter<ScarletThread>()
            .ActivateOnEnter<Shock>()
            .ActivateOnEnter<StunningSheen>()
            .ActivateOnEnter<IceBurst>()
            .ActivateOnEnter<LevinRing>()
            .ActivateOnEnter<ManyHeadedBreath>();
            //.ActivateOnEnter<Visual>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(PhantomHydraStates),
ConfigType = null,
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = null, // replace null with typeof(SID) if applicable
TetherIDType = null, // replace null with typeof(TetherID) if applicable
IconIDType = null, // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.PhantomHydra,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14523u,
SortOrder = 14,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class PhantomHydra(WorldState ws, Actor primary) : BossModule(ws, primary, new(-82f, 485f), new ArenaBoundsCircle(20f))
{
    protected override bool CheckPull() => base.CheckPull() && Raid.Player()!.Position.InCircle(Arena.Center, 20f);
}
