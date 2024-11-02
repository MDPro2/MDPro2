namespace YGOSharp.OCGWrapper.Enums
{
    public enum CardReason
    {
        DESTROY = 0x1,
        RELEASE = 0x2,
        TEMPORARY = 0x4,
        MATERIAL = 0x8,
        SUMMON = 0x10,
        BATTLE = 0x20,
        EFFECT = 0x40,
        COST = 0x80,
        ADJUST = 0x100,
        LOST_TARGET = 0x200,
        RULE = 0x400,
        SPSUMMON = 0x800,
        DISSUMMON = 0x1000,
        FLIP = 0x2000,
        DISCARD = 0x4000,
        RDAMAGE = 0x8000,
        RRECOVER = 0x10000,
        RETURN = 0x20000,
        FUSION = 0x40000,
        SYNCHRO = 0x80000,
        RITUAL = 0x100000,
        XYZ = 0x200000,
        REPLACE = 0x1000000,
        DRAW = 0x2000000,
        REDIRECT = 0x4000000,
        LINK = 0x10000000
    }
}