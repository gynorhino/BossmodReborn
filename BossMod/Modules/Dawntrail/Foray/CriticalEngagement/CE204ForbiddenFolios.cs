namespace BossMod.Dawntrail.Foray.CriticalEngagement.CriticalEngagement.CE204ForbiddenFolios;

public enum OID : uint
{
    Arbatel = 0x4BD3, // R3.060, x1
    Helper = 0x233C, // R0.500, x40, Helper type
    _Gen_Page512 = 0x4BD7, // R1.950, x0 (spawn during fight)
    _Gen_Page64 = 0x4BD4, // R2.400, x0 (spawn during fight)
    _Gen_Actor1ebfcd = 0x1EBFCD, // R0.500, x0 (spawn during fight), EventObj type
    _Gen_Page16 = 0x4BD5, // R3.220, x0 (spawn during fight)
    _Gen_Page8 = 0x4BD6, // R1.500, x0 (spawn during fight)
    _Gen_ = 0x4BD8, // R2.400, x0 (spawn during fight)
}

public enum AID : uint
{
    _AutoAttack_Attack = 49056, // Arbatel->player, no cast, single-target
    _Weaponskill_KnowledgeLevelCorrection = 47296, // Arbatel->self, 5.0s cast, ???
    _Weaponskill_KnowledgeLevelCorrection1 = 47297, // Helper->self, no cast, ???
    _Weaponskill_Summon = 49055, // Arbatel->self, 3.0s cast, ???
    Summon = 47307, // Helper->location, 3.0s cast, range 4 circle
    _Spell_PrimeKnowledgeLevelDeath = 47318, // 4BD7->self, 11.0s cast, single-target
    _Spell_KnowledgeLevel5Death = 47315, // 4BD4->self, 11.0s cast, single-target
    _Spell_PrimeKnowledgeLevelDeath1 = 49879, // Helper->self, 11.0s cast, range 25 180.000-degree cone
    _Spell_PrimeKnowledgeLevelDeath2 = 50561, // Helper->self, 11.0s cast, range 25 ?-degree cone
    _Spell_KnowledgeLevel5Death1 = 50554, // Helper->self, 11.0s cast, range 25 ?-degree cone
    _Spell_KnowledgeLevel5Death2 = 47308, // Helper->self, 11.0s cast, range 25 180.000-degree cone
    _Weaponskill_Marginalia = 47328, // Arbatel->self, 5.0s cast, single-target
    _Weaponskill_Marginalia1 = 47327, // Helper->self, 5.0s cast, ???
    _Weaponskill_ = 48246, // Arbatel->location, no cast, single-target
    _Spell_KnowledgeLevel4Holy = 50559, // Helper->self, 11.0s cast, range 25 ?-degree cone
    _Spell_KnowledgeLevel4Holy1 = 47313, // Helper->self, 11.0s cast, range 25 120.000-degree cone
    _Spell_KnowledgeLevel3Flare = 47316, // 4BD5->self, 11.0s cast, single-target
    _Spell_KnowledgeLevel3Flare1 = 47312, // Helper->self, 11.0s cast, range 25 120.000-degree cone
    _Spell_KnowledgeLevel4Holy2 = 47317, // 4BD6->self, 11.0s cast, single-target
    _Spell_KnowledgeLevel3Flare2 = 50558, // Helper->self, 11.0s cast, range 25 ?-degree cone
    _Spell_PrimeKnowledgeLevelDeath3 = 50560, // Helper->self, 11.0s cast, range 25 ?-degree cone
    _Spell_PrimeKnowledgeLevelDeath4 = 47314, // Helper->self, 11.0s cast, range 25 120.000-degree cone
    CoverToCover1 = 47302, // Arbatel->self, 4.0s cast, range 30 180.000-degree cone
    CoverToCover2 = 47303, // Arbatel->self, 1.0s cast, range 30 180.000-degree cone
    UnboundInk = 49492, // Arbatel->self, 4.0s cast, range 9 circle
    _Weaponskill_BookDrop = 47319, // Arbatel->self, 3.0s cast, single-target
    BookDrop = 47322, // 4BD8->self, 8.0s cast, range 3 circle
    ThunderII = 47324, // Helper->self, 4.0s cast, range 50 width 5 rect
    _Spell_FireII = 47326, // Arbatel->self, 4.5+0.5s cast, ???
    FireII = 47325, // Helper->self, 5.0s cast, range 60 45.000-degree cone
    _Weaponskill_ArcaneRule = 47304, // Arbatel->self, 6.0s cast, single-target
    QuadRule = 47305, // Helper->self, 6.0s cast, range 25 width 10 cross
    HorizontalRule = 47306, // Helper->self, 2.0s cast, range 50 width 6 rect
    _Weaponskill_Blot = 47300, // Arbatel->self, 3.0s cast, ???
    Blot = 47301, // Helper->location, 8.0s cast, range 15 circle
    _Spell_KnowledgeLevel5Death3 = 50557, // Helper->self, 11.0s cast, range 25 ?-degree cone
    _Spell_KnowledgeLevel5Death4 = 47311, // Helper->self, 11.0s cast, range 25 120.000-degree cone
}

public enum SID : uint
{
    Correction1 = 5014, // none->player, extra=0x0
    Correction2 = 5015, // none->player, extra=0x0
    Correction3 = 5016, // none->player, extra=0x0
    Correction4 = 5017, // none->player, extra=0x0
    Correction5 = 5018, // none->player, extra=0x0
    Invincibility = 4875, // none->4BD7/4BD4/4BD5/4BD6/4BD8, extra=0x0
}

public enum IconID : uint
{
    _Gen_Icon_m0489trg_a0c = 136, // player->self
}

public enum TetherID : uint
{
    _Gen_Tether_chn_normal02k1 = 245, // 4BD7/4BD4/4BD5/4BD6->Arbatel
}

sealed class GetEffectiveKnowledgeLevel(BossModule module) : BossComponent(module)
{
    public uint Level()
    {
        return 0;
    }
}

sealed class Summon(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Summon, 4f);
sealed class CoverToCover(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.CoverToCover1, (uint)AID.CoverToCover2], new AOEShapeCone(30f, 90f.Degrees()), 1);
sealed class UnboundInk(BossModule module) : Components.SimpleAOEs(module, (uint)AID.UnboundInk, 6f);
sealed class BookDrop(BossModule module) : Components.CastTowers(module, (uint)AID.BookDrop, 3f, 3);
sealed class ThunderII(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ThunderII, new AOEShapeRect(50f, 2.5f));
sealed class FireII(BossModule module) : Components.SimpleAOEs(module, (uint)AID.FireII, new AOEShapeCone(60f, 22.5f.Degrees()));
sealed class QuadRule(BossModule module) : Components.SimpleAOEs(module, (uint)AID.QuadRule, new AOEShapeCross(25f, 5f));
sealed class HorizontalRule(BossModule module) : Components.SimpleAOEs(module, (uint)AID.HorizontalRule, new AOEShapeRect(50f, 3f));
sealed class Blot(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Blot, 15f);

[SkipLocalsInit]
sealed class ArbatelStates : StateMachineBuilder
{
    public ArbatelStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<GetEffectiveKnowledgeLevel>()
            .ActivateOnEnter<Summon>()
            .ActivateOnEnter<CoverToCover>()
            .ActivateOnEnter<UnboundInk>()
            .ActivateOnEnter<BookDrop>()
            .ActivateOnEnter<ThunderII>()
            .ActivateOnEnter<FireII>()
            .ActivateOnEnter<QuadRule>()
            .ActivateOnEnter<HorizontalRule>()
            .ActivateOnEnter<Blot>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP,
StatesType = typeof(ArbatelStates),
ConfigType = null, // replace null with typeof(ArbatelConfig) if applicable
ObjectIDType = typeof(OID),
ActionIDType = typeof(AID), // replace null with typeof(AID) if applicable
StatusIDType = typeof(SID), // replace null with typeof(SID) if applicable
TetherIDType = typeof(TetherID), // replace null with typeof(TetherID) if applicable
IconIDType = typeof(IconID), // replace null with typeof(IconID) if applicable
PrimaryActorOID = (uint)OID.Arbatel,
Contributors = "",
Expansion = BossModuleInfo.Expansion.Dawntrail,
Category = BossModuleInfo.Category.Foray,
GroupType = BossModuleInfo.GroupType.CFC,
GroupID = 1093u,
NameID = 14520u,
SortOrder = 4,
PlanLevel = 0)]
[SkipLocalsInit]
public sealed class Arbatel(WorldState ws, Actor primary) : BossModule(ws, primary, new(660f, 660f), new ArenaBoundsCircle(20f))
{
    protected override bool CheckPull() => base.CheckPull() && Raid.Player()!.Position.InCircle(Arena.Center, 20f);
}
