namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE213LostOnTheWind;

public enum OID : uint
{
    Abductor = 0x4BE1, // R5.004, x1
    Helper = 0x233C, // R0.500, x16 (spawn during fight), Helper type
    _Gen_Abductor = 0x4BE4, // R1.000, x1
    _Gen_AbductorsPlume = 0x4BE3, // R1.000, x0 (spawn during fight)
    BitingWind = 0x4BE2, // R1.000, x0 (spawn during fight)
    Buffet = 0x1EBFA9, // R0.500, x0 (spawn during fight), EventObj type
}

public enum AID : uint
{
    _Ability_ = 47435, // 4BE4->self, no cast, range ?-30 donut
    _AutoAttack_ = 47434, // Abductor->player, no cast, single-target
    WindBlade = 47441, // Abductor->self, 5.0s cast, range 60 180.000-degree cone
    _Weaponskill_Skydive = 47446, // Abductor->location, no cast, single-target
    Skydive = 47448, // Helper->self, 5.5s cast, range 15 circle
    _Ability_PlumefallTrap = 47442, // Abductor->self, 3.0s cast, single-target
    Splinter = 47443, // 4BE3->self, 4.5s cast, range 13 circle
    Buffet = 48250, // Helper->self, 4.0s cast, range 60 width 60 rect, 24f knockback
    BuffetKnockback = 47440, // Helper->self, no cast, ???
    _Weaponskill_CyclonicRing = 47447, // Abductor->location, no cast, single-target
    CyclonicRing = 47449, // Helper->self, 5.5s cast, range 5-60 donut
    _Ability_1 = 47433, // Abductor->location, no cast, single-target
    _Spell_Hurricane = 47436, // Abductor->self, 5.0s cast, single-target
    _Spell_Hurricane1 = 48120, // Helper->self, no cast, ???
    StrongWind = 47437, // Helper->self, no cast, range 4 circle
    TendonRipper = 47439, // Helper->self, 1.0s cast, range 60 width 8 cross
    _Ability_TendonRipper1 = 47438, // 4BE2->self, 1.0s cast, single-target
    _Spell_Aerosnare = 47444, // Abductor->self, 3.5+0.5s cast, single-target
    Aerosnare = 47445, // Helper->self, 4.0s cast, range 60 60.000-degree cone
}

public enum SID : uint
{
    _Gen_Sprint = 4520, // none->4BE2, extra=0x40/0xE4, speed
}

public enum IconID : uint
{
    _Gen_Icon_m0866_lockon_8way_c0r1 = 506, // 4BE2->self
}

sealed class WindBlade(BossModule module) : Components.SimpleAOEs(module, (uint)AID.WindBlade, new AOEShapeCone(60f, 90f.Degrees()));
sealed class Skydive(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Skydive, 15f);
sealed class Splinter(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Splinter, 13f);
sealed class Buffet(BossModule module) : Components.GenericKnockback(module)
{
    private readonly List<Knockback> casters = [];
    public override ReadOnlySpan<Knockback> ActiveKnockbacks(int slot, Actor actor)
    {
        return CollectionsMarshal.AsSpan(casters);
    }

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (actor.OID == (uint)OID.Buffet)
        {
            switch (state)
            {
                case 0x00010002:
                    casters.Add(new(actor.Position, 24f, WorldState.FutureTime(11.5d), direction: actor.Rotation, kind: Kind.DirForward));
                    break;
                case 0x00100020:
                    break;
            }
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == (uint)AID.BuffetKnockback)
        {
            casters.Clear();
        }
    }
}

sealed class CyclonicRing(BossModule module) : Components.SimpleAOEs(module, (uint)AID.CyclonicRing, new AOEShapeDonut(5f, 60f));
// icon appears 4s before resolving; BitingWind travels in 2 smaller circles, 11.9f / 19.9f, 45 degrees-ish
sealed class TendonRipper(BossModule module) : Components.SimpleAOEs(module, (uint)AID.TendonRipper, new AOEShapeCross(60f, 4f));
sealed class Aerosnare(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Aerosnare, new AOEShapeCone(60f, 30f.Degrees()));
sealed class BitingWind(BossModule module) : Components.GenericAOEs(module)
{
    private const float Radius = 4f, Length = 7f;
    private static readonly AOEShapeCapsule capsule = new(Radius, Length);
    private readonly List<Actor> _winds = [with(2)];

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _winds.Count;
        if (count == 0)
        {
            return [];
        }
        var aoes = new AOEInstance[count];
        for (var i = 0; i < count; ++i)
        {
            var m = _winds[i];
            aoes[i] = new(capsule, m.Position, m.Rotation);
        }
        return aoes;
    }

    public override void OnActorCreated(Actor actor)
    {
        if (actor.OID == (uint)OID.BitingWind)
        {
            _winds.Add(actor);
        }
    }

    public override void OnActorDestroyed(Actor actor)
    {
        if (actor.OID == (uint)OID.BitingWind)
        {
            _winds.Remove(actor);
        }
    }
}

[SkipLocalsInit]
sealed class AbductorStates : StateMachineBuilder
{
    public AbductorStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<WindBlade>()
            .ActivateOnEnter<Skydive>()
            .ActivateOnEnter<Splinter>()
            .ActivateOnEnter<Buffet>()
            .ActivateOnEnter<CyclonicRing>()
            .ActivateOnEnter<BitingWind>()
            .ActivateOnEnter<TendonRipper>()
            .ActivateOnEnter<Aerosnare>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(AbductorStates),
ConfigType = null, // replace null with typeof(AbductorConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = null, // replace null with typeof(TetherID) if applicable
IconIDType = typeof(IconID), // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.Abductor,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14505u,
SortOrder = 13,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class Abductor(WorldState ws, Actor primary) : BossModule(ws, primary, new(-150f, -860f), new ArenaBoundsCircle(23f))
{
    protected override bool CheckPull() => base.CheckPull() && Raid.Player()!.Position.InCircle(Arena.Center, 23f);
}
