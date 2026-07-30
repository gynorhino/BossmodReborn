
namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE211AppallingBehavior;

public enum OID : uint
{
    Pallmagia = 0x4D8F, // R3.504, x1
    Pallkeeper = 0x4D90, // R2.300, x4
    Helper = 0x233C, // R0.500, x46, Helper type
    _Gen_Pallmagia = 0x4D91, // R1.000, x1
    EsotericIcon = 0x1EC02A, // R0.500, x4, EventObj type, EAnim used to display add AOE type
    _Gen_Actor1ec02b = 0x1EC02B, // R0.500, x0 (spawn during fight), EventObj type, EAnim used for roulette
    _Gen_Actor1ec02c = 0x1EC02C, // R0.500, x0 (spawn during fight), EventObj type, EAnim used for roulette
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
    _Spell_Roulette = 49787, // Pallmagia->self, 4.0s cast, single-target
    _Spell_Roulette1 = 49788, // Helper->self, no cast, range 5 circle
    _Spell_Roulette2 = 49790, // Helper->self, no cast, range ?-20 donut
    _Spell_Roulette3 = 49789, // Helper->self, no cast, range ?-12 donut
    _Spell_EsotericInstruction1 = 49774, // Pallmagia->self, 13.0s cast, single-target
    _Spell_ReversePolarity = 49775, // Pallmagia->self, 5.0s cast, single-target
    _Ability_1 = 49785, // 4D90->location, no cast, single-target
    _Ability_2 = 49786, // 4D90->location, no cast, single-target
    _Spell_MagicHammer = 49793, // Pallmagia->self, 3.0s cast, single-target
    MagicHammer = 49794, // Helper->location, 5.5s cast, range 8 circle
    _Ability_3 = 49784, // 4D90->location, no cast, single-target
}

public enum SID : uint
{
    _Gen_ = 2056, // none->Pallmagia/4D90, extra=0x485/0x486/0x490, duration until adds AOE
}

public enum TetherID : uint
{
    _Gen_Tether_chn_subbly_mgc01f = 14, // 4D90->Pallmagia
    _Gen_Tether_chm_m0796_mgcchanbg_0a1 = 207, // 4D90->4D90
}

sealed class GreatWhirlwind(BossModule module) : Components.RaidwideCast(module, (uint)AID.GreatWhirlwind);
sealed class BadBreath1(BossModule module) : Components.SimpleAOEs(module, (uint)AID.BadBreath, new AOEShapeCone(50f, 50f.Degrees()));
sealed class Plaincracker(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Plaincracker, 15f);
sealed class OccultMissile(BossModule module) : Components.SimpleAOEs(module, (uint)AID.OccultMissile, 6f);
sealed class LilliputianLyric(BossModule module) : Components.SimpleAOEs(module, (uint)AID.LilliputianLyric, new AOEShapeCone(40f, 90f.Degrees()));
sealed class MagicHammer(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MagicHammer, 8f, 8);
sealed class EsotericInstruction(BossModule module) : Components.GenericAOEs(module)
{
    // track adds, handle reverse polarity if cast
    // status duration until AOE goes off assigned to 0x4D90 (PallKeeper)
    // EAnim to set mechanic assigned to 0x1EC029 (same position as PallKeeper)
    // if gained status is less than 10s, boss not using Reverse Polarity
    // EAnim happens at same position as PallKeeper
    private readonly List<AOEInstance> _aoes = [with(4)];

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

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (actor.OID == (uint)OID.EsotericIcon)
        {
            switch (state)
            {
                case 0x00100020:
                    _aoes.Add(new(new AOEShapeCircle(30f), actor.Position));
                    break;
                case 0x00010002:
                    _aoes.Add(new(new AOEShapeCone(50f, 50f.Degrees()), actor.Position, actor.Rotation));
                    break;
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
                    break;
            }
        }
    }
}
/*
sealed class Roulette(BossModule module) : Components.GenericAOEs(module)
{
    // 2/6 close spots safe, 2/8 far sides safe, center always unsafe
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => throw new NotImplementedException();
}
*/

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
            .ActivateOnEnter<MagicHammer>();
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
