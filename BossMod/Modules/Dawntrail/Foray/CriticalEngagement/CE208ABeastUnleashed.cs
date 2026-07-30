namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE208ABeastUnleashed;

public enum OID : uint
{
    AtlasCarbuncle = 0x4C4F, // R9.067, x1
    Helper = 0x233C, // R0.500, x20, Helper type
    _Gen_CrescentGusion = 0x4E41, // R3.500, x1 (spawn during fight)
    _Gen_TopazStone = 0x4C50, // R1.000, x12
    _Gen_CrescentMedusa = 0x4E1D, // R1.350, x1 (spawn during fight)
    _Gen_Actor1ec031 = 0x1EC031, // R0.500, x1, EventObj type
    _Gen_Actor1ec046 = 0x1EC046, // R0.500, x2, EventObj type
    _Gen_Actor1ec045 = 0x1EC045, // R0.500, x1, EventObj type
    _Gen_AtlasCarbuncle = 0x4D88, // R1.000, x1
}

public enum AID : uint
{
    _Ability_ = 49104, // 4D88->self, no cast, ???
    _AutoAttack_ = 50852, // AtlasCarbuncle->player, no cast, single-target
    SonicHowl = 48298, // AtlasCarbuncle->self, 5.0s cast, ???
    _Weaponskill_SonicHowl1 = 49505, // Helper->self, no cast, ???
    _Weaponskill_TopazStones = 48280, // AtlasCarbuncle->self, 3.0s cast, single-target
    TopazRay1 = 48281, // 4C50->self, 3.0s cast, range 4 circle
    TopazRay2 = 48282, // 4C50->self, 3.0s cast, range 4 circle
    _Weaponskill_RubyGlow = 48284, // AtlasCarbuncle->self, 3.0s cast, ???
    _Ability_RubyGlow = 50637, // Helper->self, no cast, ???
    _Weaponskill_ReflectiveCoat = 50418, // AtlasCarbuncle->self, 3.0s cast, single-target
    _Weaponskill_RubyReflection = 48285, // Helper->self, no cast, range 20 width 20 rect
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
    _Weaponskill_RubyReflection1 = 48287, // Helper->self, no cast, range 40 width 40 rect
    _Weaponskill_RubyReflection2 = 48286, // Helper->self, no cast, range 40 width 40 rect
}

public enum SID : uint
{
    _Gen_DirectionalDisregard = 3808, // none->AtlasCarbuncle, extra=0x0
}

sealed class SonicHowl(BossModule module) : Components.RaidwideCast(module, (uint)AID.SonicHowl);
sealed class TopazRay(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.TopazRay1, (uint)AID.TopazRay2], 4f);
/*
sealed class RubyReflection(BossModule module) : Components.GenericAOEs(module)
{
    // fixed pattern for safe spots?
    // mark unsafe topaz if it intersects cell?
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => throw new NotImplementedException();
}
*/
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
    public override ReadOnlySpan<Knockback> ActiveKnockbacks(int slot, Actor actor)
    {
        return CollectionsMarshal.AsSpan(_kbs);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        // need to update left/right kb depending on half player standing on
        switch (spell.Action.ID)
        {
            case (uint)AID.KnockbackRectVisual:
                break;
            case (uint)AID.KnockbackCircleVisual:
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {

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
            .ActivateOnEnter<ClawTail>();
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
