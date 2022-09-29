namespace Common
{
    public enum EUiID
    {
        Main = 0,
        PersonDetailInfo,
        Loading,
        Fighting,
        Babel, //通天塔
        CardPackage,
        Setting,
    }

    public enum EUiType
    {
        Main,
        PersonDetailInfo,
    }


    public enum EUiLayer
    {
        Low_2D = 0,
        High_2D
    }

    public enum AnimationType
    {
        none,
        Idle,
        Run,
        Attack,
        Skill,
        Skill1,
        Skill2,
        Skill3,
        Victory,
        Die,
    }

    public enum BattleUnitType
    {
        Hero01 = 1, // 光 剑圣
        Hero02 = 2, // 无头骑士
        Hero03 = 3, // 风 剑士
        Archer = 100, // 普通弓箭手
        AirArcher = 200, // 普通弓箭手
    }

    public enum NormalAtkType
    {
        Point = 1, // 每次普工伤害一名敌人
        Sector = 2, // 每次普通伤害 扇形范围内所有敌人
    }

    public enum SkillType
    {
        Passive = 0, // 被动技能
        Charge, // 充能技能
        CD, // CD 技能 
    }

    public enum SkillShape
    {
        Round = 1, // 圆形
        Sector, // 扇形
    }

    public enum BuffType
    {
        None = 0,
        FreezeBuff = 1, // 冰冻buff
        DizzyBuff = 2, // 眩晕
        GodDefendBuff = 3, // 无敌buff
        MoveSpeed = 4, // 移动buff
        AtkSpeed = 5, // 攻速buff
        Poison = 6, // 中毒buff
    }
}