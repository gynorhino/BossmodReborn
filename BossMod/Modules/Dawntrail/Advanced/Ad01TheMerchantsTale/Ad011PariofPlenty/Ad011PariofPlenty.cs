namespace BossMod.Modules.Dawntrail.Advanced.Ad01TheMerchantsTale.Ad011PariofPlenty;

sealed class HeatBurst(BossModule module) : Components.RaidwideCast(module, (uint)AID.HeatBurst);

sealed class BurningGleam(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.BurningGleam, (uint)AID.BurningGleam1, (uint)AID.BurningGleam2], new AOEShapeCross(40f, 5f));

sealed class CharmedChainsIcon(BossModule module) : BossComponent(module)
{
    // pre-stack chains to reduce length needed to break
    private readonly List<Actor> _chainedActors = [];
    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (iconID == (uint)IconID.Chain)
        {
            _chainedActors.Add(actor);
        }
    }

    public override void OnTethered(Actor source, in ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.CharmedChain)
        {
            _chainedActors.Clear();
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var count = _chainedActors.Count;
        Actor? partner = null;

        for (var i = 0; i < count; i++)
        {
            var chained = _chainedActors[i];
            if (chained.InstanceID != actor.InstanceID)
            {
                partner = chained;
            }
        }

        if (partner == null)
            return;

        hints.GoalZones.Add(AIHints.GoalProximity(partner, 2f));
    }
}

sealed class CharmedChains(BossModule module) : Components.Chains(module, (uint)TetherID.CharmedChain)
{
    private readonly Actor?[] _partner = new Actor?[PartyState.MaxAllies];
    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (_partner[slot] != null)
        {
            hints.Add("Break the chains!");
        }
    }

    public override PlayerPriority CalcPriority(int pcSlot, Actor pc, int playerSlot, Actor player, ref uint customColor) => _partner[pcSlot] == player ? PlayerPriority.Danger : PlayerPriority.Irrelevant;

    public override void OnTethered(Actor source, in ActorTetherInfo tether)
    {
        if (tether.ID == TID)
        {
            TethersAssigned = true;
            var target = WorldState.Actors.Find(tether.Target);
            if (target != null)
            {
                SetPartner(source.InstanceID, target);
                SetPartner(target.InstanceID, source);
            }
        }
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (_partner[pcSlot] is var partner && partner != null)
        {
            Arena.AddLine(pc.Position, partner.Position);
        }
    }

    public override void OnUntethered(Actor source, in ActorTetherInfo tether)
    {
        if (tether.ID == TID)
        {
            SetPartner(source.InstanceID, null);
            SetPartner(tether.Target, null);
        }
    }

    private void SetPartner(ulong source, Actor? target)
    {
        var slot = Raid.FindSlot(source);
        if (slot >= 0)
        {
            _partner[slot] = target;
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (_partner[slot] is var partner && partner != null)
        {
            // MT stays, others move
            if (Module.PrimaryActor.TargetID != actor.InstanceID)
            {
                // if partner is MT, move
                if (Module.PrimaryActor.TargetID == partner.InstanceID)
                {
                    hints.AddForbiddenZone(new SDCircle(partner.Position, 15f));
                    hints.AddForbiddenZone(new SDCircle(partner.Position, (partner.Position - actor.Position).Length() + 1f));
                }
                else
                {
                    // if partner melee or closer, move
                    var playerDist = actor.DistanceToPoint(Module.PrimaryActor.Position) * (actor.Role is Role.Tank or Role.Melee ? 1f : 50f);
                    var partnerDist = partner.DistanceToPoint(Module.PrimaryActor.Position) * (partner.Role is Role.Tank or Role.Melee ? 1f : 50f);

                    if (partnerDist < playerDist)
                    {
                        hints.AddForbiddenZone(new SDCircle(partner.Position, 15f));
                        hints.AddForbiddenZone(new SDCircle(partner.Position, (partner.Position - actor.Position).Length() + 1f));
                    }
                }
            }
        }
    }
}

sealed class SimpleFableFlight(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.LeftFableflight,(uint)AID.RightFableflight], new AOEShapeCone(60f, 90f.Degrees()));

sealed class FireOfVictory(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.FireOfVictory, 4f);

sealed class FellSpark(BossModule module) : Components.InterceptTetherStatus(module, (uint)AID.FellSpark, (uint)TetherID.FellSpark, (uint)SID.DarkResistanceDown)
{
    // status lasts 7s, roughly 4.5s between casts as duo
    // status duration scales with party size
    // if duo, try swapping at soon as status sticks
    // x4 until mechanic resolves?

    private Actor? _lastHit = null;

    public override void OnStatusGain(Actor actor, ref ActorStatus status)
    {
        base.OnStatusGain(actor, ref status);

        if (status.ID == (uint)SID.DarkResistanceDown)
        {
            _lastHit = actor;
        }
    }

    public override void OnStatusLose(Actor actor, ref ActorStatus status)
    {
        base.OnStatusLose(actor, ref status);

        if (_lastHit == null)
            return;

        if (status.ID == (uint)SID.DarkResistanceDown)
        {
            if (_lastHit.InstanceID == actor.InstanceID)
            {
                _lastHit = null;
            }
        }
    }

    public override void OnTethered(Actor source, in ActorTetherInfo tether)
    {
        base.OnTethered(source, tether);

        if (tether.ID == TID)
        {
            Activation = WorldState.FutureTime(4.5d);
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!Active)
            return;

        if (_lastHit == null)
            return;

        if (_lastHit.InstanceID == actor.InstanceID)
        {
            // make space from center for other players to intercept
            hints.AddForbiddenZone(new AOEShapeCircle(6f), Module.PrimaryActor.Position);
        }
        else
        {
            // move to intercept tether, how to ensure tether is taken?
            // prio people without status intercept 1st?
            hints.AddForbiddenZone(new SDInvertedRect(_lastHit.Position + (_lastHit.HitboxRadius + 0.1f) * _lastHit.DirectionTo(Module.PrimaryActor), Module.PrimaryActor.Position, 0.5f), Activation);
            // add zone around player, too close and tether won't swap
            hints.AddForbiddenZone(new SDCircle(_lastHit.Position, 3f), Activation);
        }
    }
}

sealed class CurseOfCompanionshipSolitude(BossModule module) : Components.StatusStackSpread(module, (uint)SID.CurseOfCompanionship, (uint)SID.CurseOfSolitude, 15f, 15f)
{
    // 19s until mechanic resolves
    // 9s left when baulbles appear, 8s cast time
    // depending on safe zones, default AI stands in place and dies
    private readonly BurningGleam _burningGleam = module.FindComponent<BurningGleam>()!;

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        // do spread/stack 1st, plenty of time to move if spread
        // if baubles / burning gleam active, ignore spread/stack AI for a bit to get to safe spots
        var count = _burningGleam.Casters.Count;

        if (count > 0)
        {
            var caster = _burningGleam.ActiveCasters[0];
            if (caster.Activation <= WorldState.FutureTime(4d))
            {
                base.AddAIHints(slot, actor, assignment, hints);
            }
        }
        else
        {
            base.AddAIHints(slot, actor, assignment, hints);
        }
    }
}

sealed class SpurningFlames(BossModule module) : Components.RaidwideCast(module, (uint)AID.SpurningFlames);
sealed class ImpassionedSpark(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ImpassionedSparks3, 8f);
sealed class BurningPillar(BossModule module) : Components.SimpleAOEs(module, (uint)AID.BurningPillar, 10f);
sealed class SparkPuddle(BossModule module) : Components.Voidzone(module, 10f, GetPuddles)
{
    private static Actor[] GetPuddles(BossModule module)
    {
        var enemies = module.Enemies((uint)OID.SparkPuddle);
        var count = enemies.Count;
        var index = 0;
        var puddles = new Actor[count];
        for (var i = 0; i < count; i++)
        {
            var z = enemies[i];
            if (z.EventState != 7)
            {
                puddles[index++] = z;
            }
        }
        return puddles[..index];
    }
}

sealed class FireWell(BossModule module) : Components.StackWithIcon(module, (uint)IconID.Stack, (uint)AID.FireWell, 6f, 3d)
{
    /*
    // safe spot to avoid impassions sparks, puddles, and burning pillar take priority over stack
    private readonly ImpassionedSpark _sparks = module.FindComponent<ImpassionedSpark>()!;
    private readonly BurningPillar _pillars = module.FindComponent<BurningPillar>()!;
    private readonly SparkPuddle _puddles = module.FindComponent<SparkPuddle>()!;

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Stacks.Count == 0)
            return;

        var sparkActive = _sparks.ActiveCasters;
        var sparkCount = sparkActive.Length;
        var pillarActive = _pillars.ActiveCasters;
        var pillarCount = pillarActive.Length;
        var puddleActive = _puddles.ActiveAOEs(slot, actor);
        var puddleCount = puddleActive.Length;

        if (sparkCount == 0 && pillarCount == 0 && puddleCount == 0)
        {
            base.AddAIHints(slot, actor, assignment, hints);
        }
        else
        {

        }
    }
    */
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (ActiveStacks.Count == 0)
            return;

        var target = ActiveStacks[0].Target;
        if (target.InstanceID != actor.InstanceID)
        {
            hints.GoalZones.Add(AIHints.GoalProximity(target.Position, 5f, 0.5f));
        }
    }
}

sealed class ScouringScorn(BossModule module) : Components.RaidwideCast(module, (uint)AID.ScouringScorn);

[ModuleInfo(BossModuleInfo.Maturity.Contributed,
StatesType = typeof(PariOfPlentyStates),
ConfigType = null, // replace null with typeof(PariOfPlentyConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = typeof(TetherID), // replace null with typeof(TetherID) if applicable
IconIDType = null, // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.PariOfPlenty,
Contributors = "HerStolenLight",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.VariantCriterion,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1084u,
NameID = 14274u,
SortOrder = 1,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class PariOfPlenty(WorldState ws, Actor primary) : BossModule(ws, primary, new(-760f, -805f), new ArenaBoundsSquare(20f));
