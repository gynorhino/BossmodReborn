
namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE211AppallingBehavior;

public enum OID : uint
{
    Pallmagia = 0x4D8F, // R3.504, x1
    Pallkeeper = 0x4D90, // R2.300, x4
    Helper = 0x233C, // R0.500, x46, Helper type
    _Gen_Pallmagia = 0x4D91, // R1.000, x1
    EsotericIcon = 0x1EC02A, // R0.500, x4, EventObj type, EAnim used to display add AOE type
    RouletteRing1 = 0x1EC02B, // R0.500, x0 (spawn during fight), EventObj type, EAnim used for roulette
    RouletteRing2 = 0x1EC02C, // R0.500, x0 (spawn during fight), EventObj type, EAnim used for roulette
}

public enum AID : uint
{
    _Ability_ = 49771, // 4D91->self, no cast, range ?-25 donut
    _AutoAttack_ = 50494, // Pallmagia->player, no cast, single-target
    _Spell_BadBreath = 50490, // Pallmagia->self, 4.3+0.7s cast, single-target
    BadBreath = 50491, // Helper->self, 5.0s cast, range 50 100.000-degree cone
    _Spell_Plaincracker = 50492, // Pallmagia->self, 4.3+0.7s cast, single-target
    Plaincracker = 50493, // Helper->self, 5.0s cast, range 15 circle
    _Spell_Summon = 49772, // Pallmagia->self, 3.0s cast, single-target
    EsotericInstruction = 49773, // Pallmagia->self, 13.0s cast, single-target
    PlaincrackerAdd = 49779, // Helper->self, 3.0s cast, range 30 circle
    _Ability_Plaincracker1 = 49778, // 4D90->self, no cast, single-target
    BadBreathAdd = 49777, // Helper->self, 3.0s cast, range 50 100.000-degree cone
    _Ability_BadBreath1 = 49776, // 4D90->self, no cast, single-target
    _Spell_GreatWhirlwind = 49798, // Pallmagia->self, 4.3+0.7s cast, single-target
    GreatWhirlwind = 50450, // Helper->self, 5.0s cast, ???
    _Spell_ = 49799, // Helper->self, 5.0s cast, single-target
    _Spell_OccultMissile = 49795, // Pallmagia->self, 3.3+0.7s cast, single-target
    OccultMissile = 49797, // Helper->location, 4.0s cast, range 6 circle
    _Spell_LilliputianLyric = 49791, // Pallmagia->self, 4.3+0.7s cast, single-target
    LilliputianLyric = 49792, // Helper->self, 5.0s cast, range 40 180.000-degree cone
    RouletteCast = 49787, // Pallmagia->self, 4.0s cast, single-target
    RouletteCenter = 49788, // Helper->self, no cast, range 5 circle
    RouletteInner = 49789, // Helper->self, no cast, range 5-12 donut sector
    RouletteOuter = 49790, // Helper->self, no cast, range 12-20 donut sector
    EsotericInstructionPolarity = 49774, // Pallmagia->self, 13.0s cast, single-target
    ReversePolarityCast = 49775, // Pallmagia->self, 5.0s cast, single-target
    _Ability_1 = 49785, // 4D90->location, no cast, single-target
    _Ability_2 = 49786, // 4D90->location, no cast, single-target
    _Spell_MagicHammer = 49793, // Pallmagia->self, 3.0s cast, single-target
    MagicHammer = 49794, // Helper->location, 5.5s cast, range 8 circle
    ReversePolaritySwap = 49784, // 4D90->location, no cast, single-target
}

public enum SID : uint
{
    EsotericResolve = 2056, // none->Pallmagia/4D90, extra=0x485/0x486/0x490, duration until adds AOE
}

public enum TetherID : uint
{
    EsotericInstruction = 14, // 4D90->Pallmagia
    ReversePolarity = 207, // 4D90->4D90
}

sealed class GreatWhirlwind(BossModule module) : Components.RaidwideCast(module, (uint)AID.GreatWhirlwind);
sealed class BadBreath1(BossModule module) : Components.SimpleAOEs(module, (uint)AID.BadBreath, new AOEShapeCone(50f, 50f.Degrees()));
sealed class Plaincracker(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Plaincracker, 15f);
sealed class OccultMissile(BossModule module) : Components.SimpleAOEs(module, (uint)AID.OccultMissile, 6f);
sealed class LilliputianLyric(BossModule module) : Components.SimpleAOEs(module, (uint)AID.LilliputianLyric, new AOEShapeCone(40f, 90f.Degrees()));
sealed class MagicHammer(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MagicHammer, 8f, 8);
sealed class EsotericInstruction(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [with(4)];
    private readonly List<Esoteric> _esoteric = [with(4)];
    private readonly AOEShapeCircle _circle = new(30f);
    private readonly AOEShapeCone _cone = new(50f, 50f.Degrees());
    private int _tetherCount = 0;
    private bool _isActive = false;

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (!_isActive)
        {
            return [];
        }

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

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (actor.OID == (uint)OID.EsotericIcon)
        {
            var mechanic = state switch
            {
                0x00010002 => Mechanic.BadBreath,
                0x00100020 => Mechanic.Plaincracker,
                _ => Mechanic.Invalid
            };

            if (mechanic == Mechanic.Invalid)
                return;

            var pallkeepers = Module.Enemies((uint)OID.Pallkeeper);
            var count = pallkeepers.Count;

            for (var i = 0; i < count; i++)
            {
                var pallkeeper = pallkeepers[i];
                if (pallkeeper.Position.AlmostEqual(actor.Position, 1f))
                {
                    _esoteric.Add(new(pallkeeper, mechanic, pallkeeper.Position, pallkeeper.Rotation, default));
                }
            }
        }
    }

    public override void OnTethered(Actor source, in ActorTetherInfo tether)
    {
        // always 2 tethers for reverse polarity?
        if (tether.ID == (uint)TetherID.ReversePolarity)
        {
            var targetId = tether.Target;
            var sourceIndex = -1;
            var targetIndex = -1;

            var count = _esoteric.Count;
            for (var i = 0; i < count; i++)
            {
                var actor = _esoteric[i].Actor;
                if (actor == source)
                {
                    sourceIndex = i;
                }
                if (actor.InstanceID == targetId)
                {
                    targetIndex = i;
                }
            }

            var esoSource = _esoteric[sourceIndex];
            var esoTarget = _esoteric[targetIndex];

            (WPos Position, Angle Rotation) newSource = (esoTarget.Actor.Position, esoTarget.Actor.Rotation);
            (WPos Position, Angle Rotation) newTarget = (source.Position, source.Rotation);

            esoSource.Position = newSource.Position;
            esoSource.Rotation = newSource.Rotation;
            esoTarget.Position = newTarget.Position;
            esoTarget.Rotation = newTarget.Rotation;

            _tetherCount++;

            if (_tetherCount == 2)
            {
                SetAOEs();
            }
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.EsotericInstruction or (uint)AID.EsotericInstructionPolarity)
        {
            var isPolarity = spell.Action.ID == (uint)AID.EsotericInstructionPolarity;

            var count = _esoteric.Count;
            for (var i = 0; i < count; i++)
            {
                var eso = _esoteric[i];
                eso.Activation = WorldState.FutureTime((isPolarity ? 6.6d : 0d) + 6d + i * 4.5d);
            }

            if (!isPolarity)
            {
                SetAOEs();
            }
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count != 0)
        {
            switch (spell.Action.ID)
            {
                case (uint)AID.BadBreathAdd:
                case (uint)AID.PlaincrackerAdd:
                    _aoes.RemoveAt(0);
                    _tetherCount = 0;
                    _isActive = _aoes.Count != 0;
                    break;
            }
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (_esoteric.Count > 0 && !_isActive)
        {
            hints.AddForbiddenZone(new SDDonut(Arena.Center, 5f, 20f));
        }
        base.AddAIHints(slot, actor, assignment, hints);
    }

    private void SetAOEs()
    {
        _aoes.Clear();
        var count = _esoteric.Count;
        for (var i = 0; i < count; i++)
        {
            var eso = _esoteric[i];
            AOEShape shape = eso.Mechanic == Mechanic.BadBreath ? _cone : _circle;
            _aoes.Add(new(shape, eso.Position, eso.Rotation, eso.Activation));
        }
        _esoteric.Clear();
        _isActive = true;
    }

    private class Esoteric(Actor actor, Mechanic mechanic, WPos position, Angle rotation, DateTime activation)
    {
        public Actor Actor { get; set; } = actor;
        public Mechanic Mechanic { get; set; } = mechanic;
        public WPos Position { get; set; } = position;
        public Angle Rotation { get; set; } = rotation;
        public DateTime Activation { get; set; } = activation;
    }

    private enum Mechanic
    {
        Plaincracker,
        BadBreath,
        Invalid
    }
}

sealed class Roulette(BossModule module) : Components.GenericAOEs(module)
{
    // 2/6 close spots safe, 2/8 far sides safe, center always unsafe
    // center 5f circle, inner 5-12 donut sector 120deg, outer 12-20 donut sector 135deg
    // spawns 2 roulette actors, visibility set with EObjAnim 00010002
    // after roulette finishes casting, ~6s in danger indicator, after ~10s total actual attacks (x1 center, x2 inner, x2 outer)
    // spawned actors not always same initial rotation; affects start/end safe positions?
    // outer always CW, inner always CCW?
    /*
roulettes cast #1 (actors spawned with 0deg rotation) (outer CW, inner CCW)
outer -> X X X O (horizontal flat) (facing north)
inner -> 0 X X (slightly skewed right 22.5deg)
inner -> -120, 60
outer -> -45, 135

roulettes cast #2 (actors spawned with 90deg rotation) (outer CW, inner CCW)
outer -> X O X X
inner -> X X O (flat)
inner -> -30, 150
outer -> -135, 45

finished
outer -> X X X 0
inner -> 0 X X (slightly skewed right)

================================

1st instance has both with rot90
both 0x00010002 on visibility
0x00040020 or 0x00040010 depending on CW or CWW
spawn
outer -> X O X X (flat)
inner -> X X O (flat)
resolve
outer -> X X X O (slight tilt)
inner -> O X X (slight tilt)

2nd instance has both with rot0
spawn
outer -> X X X 0 (flat)
inner -> 0 X X (slight tilt)
resolve
outer -> 0 X X X (slight tilt)
inner -> X 0 X (slight tilt)

outer = 90deg rot-ish
inner = 120deg rot-ish
    */
    private readonly List<AOEInstance> _aoes = [];
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        return CollectionsMarshal.AsSpan(_aoes);
    }

    public override void OnActorCreated(Actor actor)
    {
        if (actor.OID is (uint)OID.RouletteRing1 or (uint)OID.RouletteRing2)
        {
            var act = WorldState.FutureTime(18d);
            var outerDiff = -67.5f.Degrees();
            var innerDiff = 120f.Degrees();
            //var outerDiff = 0f.Degrees();
            //var innerDiff = 0f.Degrees();
            switch (actor.OID)
            {
                case (uint)OID.RouletteRing1:
                    _aoes.Add(new(new AOEShapeCircle(5f), Arena.Center, activation: act));
                    _aoes.Add(new(_outer, Arena.Center, actor.Rotation + outerDiff, act));
                    _aoes.Add(new(_outer, Arena.Center, actor.Rotation + outerDiff + 180f.Degrees(), act));
                    break;
                case (uint)OID.RouletteRing2:
                    _aoes.Add(new(_inner, Arena.Center, actor.Rotation + innerDiff, act));
                    _aoes.Add(new(_inner, Arena.Center, actor.Rotation + innerDiff + 180f.Degrees(), act));
                    break;
            }
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.RouletteCenter:
            case (uint)AID.RouletteInner:
            case (uint)AID.RouletteOuter:
                _aoes.Clear();
                break;
        }
    }

    private readonly AOEShapeDonutSector _outer = new(12f, 20f, 67.5f.Degrees(), 22.5f.Degrees());
    private readonly AOEShapeDonutSector _inner = new(5f, 12f, 60f.Degrees(), -60f.Degrees());
}

[SkipLocalsInit]
sealed class PallmagiaStates : StateMachineBuilder
{
    public PallmagiaStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<GreatWhirlwind>()
            .ActivateOnEnter<BadBreath1>()
            .ActivateOnEnter<Plaincracker>()
            .ActivateOnEnter<EsotericInstruction>()
            .ActivateOnEnter<OccultMissile>()
            .ActivateOnEnter<LilliputianLyric>()
            .ActivateOnEnter<MagicHammer>()
            .ActivateOnEnter<Roulette>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(PallmagiaStates),
ConfigType = null, // replace null with typeof(PallmagiaConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = typeof(TetherID), // replace null with typeof(TetherID) if applicable
IconIDType = null, // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.Pallmagia,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14714u,
SortOrder = 11,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class Pallmagia(WorldState ws, Actor primary) : BossModule(ws, primary, new(807f, -562f), new ArenaBoundsCircle(20f))
{
    protected override bool CheckPull() => base.CheckPull() && Raid.Player()!.Position.InCircle(Arena.Center, 20f);
}
