using BossMod.Autorotation.xan;
using static BossMod.Dawntrail.Extreme.Ex3QueenEternal.RadicalShift;

namespace BossMod.Dawntrail.Foray.FATE.NH110RegnantChimera;

public enum OID : uint
{
    RegnantChimera = 0x4C7D, // R5.180, x1
    GlacipotentOrb = 0x4C80, // R2.000, x0 (spawn during fight)
    FulmipotentOrb = 0x4C7F, // R2.000, x0 (spawn during fight)
    Cacophony = 0x4B71, // R1.000, x0 (spawn during fight)
}

public enum AID : uint
{
    _AutoAttack_ = 50856, // RegnantChimera->player, no cast, single-target
    TheRamsBreath = 48631, // RegnantChimera->self, 6.0s cast, range 30 120.000-degree cone
    TheRamsBreath1 = 48632, // RegnantChimera->self, no cast, range 30 120.000-degree cone
    TheRamsBreath2 = 49748, // RegnantChimera->self, no cast, range 30 120.000-degree cone

    TheRamsVoice = 48633, // RegnantChimera->self, 4.0s cast, range 9 circle
    TheRamsVoice1 = 48635, // 4C80->location, 1.0s cast, range 12 circle

    TheDragonsBreath = 48629, // RegnantChimera->self, 6.0s cast, range 30 120.000-degree cone
    TheDragonsBreath1 = 48630, // RegnantChimera->self, no cast, range 30 120.000-degree cone
    TheDragonsBreath2 = 49747, // RegnantChimera->self, no cast, range 30 120.000-degree cone

    TheDragonsVoice = 48634, // RegnantChimera->self, 4.0s cast, range ?-30 donut
    TheDragonsVoice1 = 48636, // 4C7F->location, 4.0s cast, range 8-30 donut

    Cacophony = 50113, // RegnantChimera->self, 4.0s cast, single-target
}

public enum SID : uint
{
    _Gen_1 = 5196, // RegnantChimera/4C80->4C80/RegnantChimera, extra=0x0
    _Gen_2 = 5197, // RegnantChimera/4C7F->4C7F/RegnantChimera, extra=0x0
}

public enum IconID : uint
{
    _Gen_Icon_x6r3_turning_left_c0e1 = 547, // RegnantChimera->self
    _Gen_Icon_x6r3_turning_right_c0e1 = 546, // RegnantChimera->self
}

sealed class TheRamsVoice(BossModule module) : Components.SimpleAOEs(module, (uint)AID.TheRamsVoice, 9f);
sealed class TheRamsVoice1(BossModule module) : Components.SimpleAOEs(module, (uint)AID.TheRamsVoice1, 12f)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {

    }

    public override void OnActorCreated(Actor actor)
    {
        if (actor.OID == (uint)OID.GlacipotentOrb)
        {
            Casters.Add(new(Shape, actor.Position, default, default, actorID: actor.InstanceID, shapeDistance: Shape.Distance(actor.Position, default)));
        }
    }
}
sealed class TheDragonsVoice(BossModule module) : Components.SimpleAOEs(module, (uint)AID.TheDragonsVoice, new AOEShapeDonut(8f, 30f));
sealed class TheDragonsVoice1(BossModule module) : Components.SimpleAOEs(module, (uint)AID.TheDragonsVoice1, new AOEShapeDonut(8f, 30f));

[SkipLocalsInit]
sealed class RegnantChimeraStates : StateMachineBuilder
{
    public RegnantChimeraStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<TheRamsVoice>()
            .ActivateOnEnter<TheRamsVoice1>()
            .ActivateOnEnter<TheDragonsVoice>()
            .ActivateOnEnter<TheDragonsVoice1>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
    StatesType = typeof(RegnantChimeraStates),
    ConfigType = null, // replace null with typeof(DemiMedusaConfig) if applicable
    ObjectIDType = typeof(OID),
    ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
    StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
    TetherIDType = null, // replace null with typeof(TetherID) if applicable
    IconIDType = typeof(IconID), // replace null with typeof(IconID) if applicable
    PrimaryActorOID = (uint)OID.RegnantChimera,
    Contributors = "",
    Expansion = BossModuleInfo.Expansion.Dawntrail,
    Category = BossModuleInfo.Category.Foray,
    GroupType = BossModuleInfo.GroupType.CFC,
    GroupID = 1093u,
    NameID = 14767u,
    SortOrder = 1,
    PlanLevel = 0)]
[SkipLocalsInit]
public sealed class RegnantChimera(WorldState ws, Actor primary) : OpenWorldFate(ws, primary);
