namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE208ABeastUnleashed;

public enum OID : uint
{
    AtlasCarbuncle = 0x4C4F, // R9.067, x1
    Helper = 0x233C, // R0.500, x20, Helper type
    _Gen_CrescentGusion = 0x4E41, // R3.500, x1 (spawn during fight)
    TopazStone = 0x4C50, // R1.000, x12
    _Gen_CrescentMedusa = 0x4E1D, // R1.350, x1 (spawn during fight)
    _Gen_Actor1ec031 = 0x1EC031, // R0.500, x1, EventObj type
    RubyReflectionSquare = 0x1EC045, // R0.500, x1, EventObj type
    RubyReflectionL = 0x1EC046, // R0.500, x2, EventObj type
    _Gen_AtlasCarbuncle = 0x4D88, // R1.000, x1
}

public enum AID : uint
{
    _Ability_ = 49104, // 4D88->self, no cast, ???
    _AutoAttack_ = 50852, // AtlasCarbuncle->player, no cast, single-target
    SonicHowl = 48298, // AtlasCarbuncle->self, 5.0s cast, ???
    _Weaponskill_SonicHowl1 = 49505, // Helper->self, no cast, ???
    _Weaponskill_TopazStones = 48280, // AtlasCarbuncle->self, 3.0s cast, single-target
    TopazRayReflected = 48281, // 4C50->self, 3.0s cast, range 4 circle, reflected crystal
    TopazRay = 48282, // 4C50->self, 3.0s cast, range 4 circle
    _Weaponskill_RubyGlow = 48284, // AtlasCarbuncle->self, 3.0s cast, ???
    _Ability_RubyGlow = 50637, // Helper->self, no cast, ???
    _Weaponskill_ReflectiveCoat = 50418, // AtlasCarbuncle->self, 3.0s cast, single-target
    RubyReflectionQuadrant = 48285, // Helper->self, no cast, range 20 width 20 rect
    RubyReflection1 = 48286, // Helper->self, no cast, range 40 width 40 rect
    RubyReflection2 = 48287, // Helper->self, no cast, range 40 width 40 rect
    _Weaponskill_ = 48299, // AtlasCarbuncle->location, no cast, single-target
    ClawToTail1 = 48294, // AtlasCarbuncle->self, 6.0s cast, range 40 180.000-degree cone
    ClawToTail2 = 48296, // AtlasCarbuncle->self, no cast, range 45 ?-degree cone
    TailToClaw1 = 48295, // AtlasCarbuncle->self, 6.0s cast, range 40 180.000-degree cone
    TailToClaw2 = 48297, // AtlasCarbuncle->self, no cast, range 45 ?-degree cone
    KnockbackCircleVisual = 48288, // Helper->self, 2.5s cast, range 60 circle, 30f
    KnockbackRectVisual = 48289, // Helper->self, 2.5s cast, range 40 width 60 rect, 15f
    SpinebreakingStampedeJump1 = 48291, // AtlasCarbuncle->location, 8.0s cast, ???
    SpinebreakingStampedeJump2 = 48292, // AtlasCarbuncle->location, no cast, ???
    SpinebreakingStampedeCircle = 49506, // Helper->self, no cast, ???
    SpinebreakingStampedeRect = 49507, // Helper->self, no cast, ???
    _Ability_1 = 50461, // AtlasCarbuncle->self, no cast, single-target
}

public enum SID : uint
{
    _Gen_DirectionalDisregard = 3808, // none->AtlasCarbuncle, extra=0x0
}

sealed class SonicHowl(BossModule module) : Components.RaidwideCast(module, (uint)AID.SonicHowl);
sealed class TopazRay(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        return CollectionsMarshal.AsSpan(_aoes);
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (actor.OID == (uint)OID.TopazStone && id == 0x2489)
        {
            _aoes.Add(new(new AOEShapeCircle(4f), actor.Position));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID is (uint)AID.TopazRay or (uint)AID.TopazRayReflected)
        {
            _aoes.Clear();
        }
    }
}

sealed class RubyReflection(BossModule module) : Components.GenericAOEs(module)
{
    // 4 rubies but only 2 EObjAnim (00010002 & 00100020)
    // topaz that get reflected cast TopazRay1, safe ones cast TopazRay2
    // 1st instance, arena gets split into quadrants, 2 bad topaz, 2 quadrants unsafe
    // helpers cast RubyReflectionQuadrant in the middle of their quadrants, so aoerect 10 front 10 back
    // bad topaz facing towards the direction that helper will be in?
    // 2nd instance, arena split into 4 L shapes, 3 bad topaz, 3x RubyReflection2
    // bad topaz facing towards area that gets hit
    // RubyReflection2 shape like an L on its side, ___|, with helper at 90deg rotation like an upsidedown L when facing N, regular L shape at -90 deg
    // RubyReflection1 shape is a backwards L, |___
    // maybe map topaz position and rotation to L-shape AOE?
    // maybe EObjAnim 0x1EC046 determines type? 9s before Reflective Coat starts casting, 0x01000200 for 1st one @119s (Reflection2), 0x00100020 for 2nd one @141s (Reflection1), 0x00100020 for 3rd one @241s (Reflection1), 0x00100020 for 4th one @260s (Reflection 1)
    /*
ReflectorSquare (sets 4 quadrants for reflection)
- 0x00010002 @36.4
- 0x00100020 @39.4
- 0x00400080 @48.5 (same time as topaz starts casting)

Reflector1 L (regular L shape when facing north) (actor rot 0)
- 0x00010002 @116.7
- 0x01000200 @119.7
- 0x00010002 @131.58 (same time as topaz starts casting)
- 0x10002000 @131.95
- 0x00040080 @136.6 (after reflect resolves)

----
X-O-
X-O-
XXOO

Reflector2 L (backward L shape when facing north) (actor rot 90)
- 0x00100020 @141.7
- 0x04000800 @153.8 (same time as topaz starts)

OOXX
O-X-
O-X-
----

Reflector2 L (backward L shape but rotated CW when facing north) (actor rot 0)
- 0x00010002 @235.7 (rotation modifier?)
- 0x00100020 @238.7
- 0x00010002 @250.6 (same time as topaz starts)
- 0x04000800 @250.9
- 0x00040008 @255.6 (after reflect resolves)

X---
XXX-
O---
OOO-

Reflector2 L (backward L shape when facing north) (actor rot 90)
- 0x00100020 @260.6
- 0x04000800 @272.7 (same time as topaz starts)
- 0x00040008 @277.5 (after reflect resolves)

OOXX
O-X-
O-X-
----

====================================================
2 actors that have EOBjAnim for L-shape Reflectors; exists from start, rotations never change
- 0x4006C20 = rot90
- 0x4006C21 = rot0
- actor and rotation doesn't appear to affect AOE

Reflector2 L (backward L shape when facing north) (actor 0x400062C0) (rot 90)
- 0x00010002
- 0x00100020
- 0x04000800 (topaz starts)
- 0x00040008 (resolved)

OO44
O142
O142
1122

Reflector2 (backward L shape but rotating CCW when facing north) (actor 0x40062C1) (rot 0)
- 0x00100020
- 0x04000800 (topaz starts)
- 0x00040008 (resolved)

1OOO
111O
3444
3334

Reflector1 (regular L shape but rotated CW) (actor 0x40062C0) (rot 90)
- 0x00010002
- 0x01000200
- 0x00010002 (topaz starts) (other actor 0x40062C1)
- 0x10002000 (topaz starts)
- 0x00040080 (resolved)

111O
1OOO
3334
3444

Reflector2 (backward L shape but rotated CCW) (actor 0x40062C1) (rot 0)
- 0x00100020
- 0x04000800 (topaz starts)
- 0x00040008 (resolved)

1OOO
111O
3444
3334

    */
    private readonly List<AOEInstance> _aoes = [];
    private readonly List<Actor> _badTopaz = [];

    private readonly List<Actor> _topaz = [];

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        return CollectionsMarshal.AsSpan(_aoes);
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (actor.OID == (uint)OID.TopazStone && id == 0x2489)
        {
            _topaz.Add(actor);
        }
    }

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (actor.OID == (uint)OID.RubyReflectionSquare)
        {
            if (state == 0x00010002)
            {
                var quadCount = Quadrants.Length;
                var tcount = _topaz.Count;
                for (var i = 0; i < tcount; i++)
                {
                    for (var j = 0; j < quadCount; j++)
                    {
                        var quad = Quadrants[j];
                        var t = _topaz[i];
                        var p = Arena.ClampToBounds(t.Position + (t.Rotation + 180f.Degrees()).ToDirection() * 3f);
                        if (quad.InSquare(t.Position, 10f) && !quad.InSquare(p, 10f))
                        {
                            _aoes.Add(new(new AOEShapeRect(10f, 10f, 10f), quad));
                        }
                    }
                }
            }
        }
        else if (actor.OID == (uint)OID.RubyReflectionL)
        {
            if (state is 0x00100020 or 0x01000200)
            {
                var shapes = state == 0x00100020 ? Reflection2Zero : Reflection1Zero;
                var rubyRot = actor.Rotation;
                var shapeCount = shapes.Length;
                var topazCount = _topaz.Count;
                for (var i = 0; i < topazCount; i++)
                {
                    for (var j = 0; j < shapeCount; j++)
                    {
                        var shape = shapes[j];
                        shape.Polygon = shape.GetCombinedPolygon(Arena.Center).Transform(default, rubyRot.ToDirection());
                        var t = _topaz[i];
                        var p = Arena.ClampToBounds(t.Position + (t.Rotation + 180f.Degrees()).ToDirection() * 3f);
                        if (shape.Check(t.Position, Arena.Center, default) && !shape.Check(p, Arena.Center, default))
                        {
                            _aoes.Add(new(shape, Arena.Center));
                        }
                    }
                }
            }
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.TopazRayReflected)
        {
            _badTopaz.Add(caster);
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_badTopaz.Count > 0)
        {
            switch (spell.Action.ID)
            {
                case (uint)AID.TopazRay:
                case (uint)AID.TopazRayReflected:
                case (uint)AID.RubyReflectionQuadrant:
                case (uint)AID.RubyReflection1:
                case (uint)AID.RubyReflection2:
                    _badTopaz.Clear();
                    _topaz.Clear();
                    _aoes.Clear();
                    break;
            }
        }

        if (_topaz.Count > 0)
        {
            if (spell.Action.ID == (uint)AID.TopazRay)
            {
                _topaz.Clear();
            }
        }
    }
    /*
    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        var count = _badTopaz.Count;
        for (var i = 0; i < count; i++)
        {
            var topaz = _badTopaz[i];
            Arena.ZoneCircleOutline(topaz.Position, 2f);
            WPos to = topaz.Position + topaz.Rotation.ToDirection() * 2f;
            Arena.AddLine(topaz.Position, to);
        }

        var c = _topaz.Count;
        for (var i = 0; i < c; i++)
        {
            var t = _topaz[i];
            var p = Arena.ClampToBounds(t.Position + (t.Rotation + 180f.Degrees()).ToDirection() * 3f);
            Arena.ZoneCircleOutline(p, 1f, Colors.Safe);
        }
    }
    */

    // 228,342 | 248,342 | 228,362 | 248,362
    private readonly WPos[] Quadrants = [new(228f, 342f), new(248f, 342f), new(228f, 362f), new(248f, 362f)];
    private readonly AOEShapeCustom[] Reflection1Zero = [
        new([new Square(new(223f, 337f), 5f), new Square(new(233f, 337f), 5f), new Square(new(233f, 347f), 5f), new Square(new(233f, 357f), 5f),]),
        new([new Square(new(223f, 347f), 5f), new Square(new(223f, 357f), 5f), new Square(new(223f, 367f), 5f), new Square(new(233f, 367f), 5f),]),
        new([new Square(new(243f, 337f), 5f), new Square(new(253f, 337f), 5f), new Square(new(253f, 347f), 5f), new Square(new(253f, 357f), 5f),]),
        new([new Square(new(243f, 347f), 5f), new Square(new(243f, 357f), 5f), new Square(new(243f, 367f), 5f), new Square(new(253f, 367f), 5f),]),
    ];
    private readonly AOEShapeCustom[] Reflection2Zero = [
        new([new Square(new(223f, 337f), 5f), new Square(new(223f, 347f), 5f), new Square(new(233f, 347f), 5f), new Square(new(243f, 347f), 5f)]),
        new([new Square(new(233f, 337f), 5f), new Square(new(243f, 337f), 5f), new Square(new(253f, 337f), 5f), new Square(new(253f, 347f), 5f)]),
        new([new Square(new(223f, 357f), 5f), new Square(new(223f, 367f), 5f), new Square(new(233f, 367f), 5f), new Square(new(243f, 367f), 5f)]),
        new([new Square(new(233f, 357f), 5f), new Square(new(243f, 357f), 5f), new Square(new(253f, 357f), 5f), new Square(new(253f, 367f), 5f)]),
    ];
}

sealed class ClawTail(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [with(2)];
    private static readonly AOEShapeCone cone = new(45f, 90f.Degrees());

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var max = count > 1 ? 1 : count;
        var aoes = CollectionsMarshal.AsSpan(_aoes);
        return aoes[..max];
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.ClawToTail1:
                _aoes.Add(new(cone, caster.Position, caster.Rotation, Module.CastFinishAt(spell)));
                _aoes.Add(new(cone, caster.Position, caster.Rotation + 180f.Degrees(), Module.CastFinishAt(spell, 3d)));
                break;
            case (uint)AID.TailToClaw1:
                _aoes.Add(new(cone, caster.Position, caster.Rotation + 180f.Degrees(), Module.CastFinishAt(spell)));
                _aoes.Add(new(cone, caster.Position, caster.Rotation, Module.CastFinishAt(spell, 3d)));
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count != 0)
        {
            switch (spell.Action.ID)
            {
                case (uint)AID.ClawToTail1:
                case (uint)AID.TailToClaw1:
                case (uint)AID.ClawToTail2:
                case (uint)AID.TailToClaw2:
                    _aoes.RemoveAt(0);
                    break;
            }
        }
    }
}

sealed class SpinebreakingStampede(BossModule module) : Components.GenericKnockback(module)
{
    private readonly List<Knockback> _kbs = [with(2)];
    private readonly TopazRay _topazRay = module.FindComponent<TopazRay>()!;
    private readonly AOEShapeRect _rect = new(40f, 30f);
    private bool _isAlongZAxis = false;
    private Angle _direction = default;
    public override ReadOnlySpan<Knockback> ActiveKnockbacks(int slot, Actor actor)
    {
        var kbs = CollectionsMarshal.AsSpan(_kbs);
        var count = kbs.Length;

        for (var i = 0; i < count; i++)
        {
            ref var kb = ref kbs[i];
            if (kb.Origin.AlmostEqual(Arena.Center, 0.1f))
            {
                var pos = actor.Position;
                var p = _isAlongZAxis ? pos.Z : pos.X;
                var a = _isAlongZAxis ? Arena.Center.Z : Arena.Center.X;
                var direction = p < a ? _direction : _direction + 180f.Degrees();
                kbs[i] = new(Arena.Center, 15f, kb.Activation, kb.Shape, direction, Kind.DirForward);
            }
        }

        return kbs;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        // need to update left/right kb depending on half player standing on
        if (spell.Action.ID == (uint)AID.KnockbackRectVisual)
        {
            //var act = Module.CastFinishAt(spell, 7.5d);
            //var pos = Arena.Center;
            var rot = spell.Rotation;
            var offset = 90f.Degrees();
            var rot1 = rot + offset;
            _isAlongZAxis = rot1.AlmostEqual(default, Angle.DegToRad) || rot1.AlmostEqual(180f.Degrees(), Angle.DegToRad);
            //_kbs.Add(new(pos, 15f, act, new AOEShapeRect(30f, 30f), rot1, Kind.DirForward));
        }
        else if (spell.Action.ID == (uint)AID.KnockbackCircleVisual)
        {
            //5.2d, 8.5d
            // angle to circle
            _direction = (caster.Position - Arena.Center).ToAngle();
            _kbs.Add(new(Arena.Center, 15f, Module.CastFinishAt(spell, 5.2d), null, default, Kind.None));

            var act = Module.CastFinishAt(spell, 8.5d);
            var pos = caster.Position;
            _kbs.Add(new(pos, 30f, act));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_kbs.Count != 0)
        {
            switch (spell.Action.ID)
            {
                // use jump instead of actual kb since helper casts each twice
                case (uint)AID.SpinebreakingStampedeJump1:
                case (uint)AID.SpinebreakingStampedeJump2:
                    _kbs.RemoveAt(0);
                    break;
            }
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var count = _kbs.Count;
        if (count != 0)
        {
            ref readonly var kb = ref _kbs.Ref(0);
            var act = kb.Activation;

            if (kb.Origin.AlmostEqual(Arena.Center, 0.1f))
            {
                hints.AddForbiddenZone(_rect, Arena.Center, _direction + 180f.Degrees(), act);
                hints.AddForbiddenZone(new AOEShapeDonut(3f, 40f), Arena.Center, activation: act);
            }
            else
            {
                //hints.AddForbiddenZone(new SDKnockbackInCircleAwayFromOrigin(Arena.Center, kb.Origin, 30f, 18f), act);
                var topaz = _topazRay.ActiveAOEs(slot, actor);
                var topazCount = topaz.Length;
                WPos[] topazPos = new WPos[topazCount];
                for (var i = 0; i < topazCount; i++)
                {
                    topazPos[i] = topaz[i].Origin;
                }
                // smaller AOE size, enough time to run out of AOE if inside
                hints.AddForbiddenZone(new SDKnockbackInCircleAwayFromOriginPlusAOECircles(Arena.Center, kb.Origin, 30f, 18f, topazPos, 4f, count), act);
            }
        }
    }
}

[SkipLocalsInit]
sealed class AtlasCarbuncleStates : StateMachineBuilder
{
    public AtlasCarbuncleStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<SonicHowl>()
            .ActivateOnEnter<TopazRay>()
            .ActivateOnEnter<RubyReflection>()
            .ActivateOnEnter<ClawTail>()
            .ActivateOnEnter<SpinebreakingStampede>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(AtlasCarbuncleStates),
ConfigType = null,
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = null, // replace null with typeof(TetherID) if applicable
IconIDType = null, // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.AtlasCarbuncle,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14791u,
SortOrder = 8,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class AtlasCarbuncle(WorldState ws, Actor primary) : BossModule(ws, primary, new(238f, 352f), new ArenaBoundsSquare(20f))
{
    protected override bool CheckPull() => base.CheckPull() && Raid.Player()!.Position.InCircle(Arena.Center, 20f);
}
