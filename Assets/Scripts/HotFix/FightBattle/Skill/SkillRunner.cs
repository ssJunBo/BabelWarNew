using System.Collections.Generic;
using System.Linq;

namespace HotFix.FightBattle.Skill
{
    // 技能执行器
    public class SkillRunner
    {
        private readonly List<SkillBase> _skillBases = new List<SkillBase>();

        public void AddSkill(SkillBase skillBase)
        {
            if (_skillBases.Contains(skillBase)) return;
            
            _skillBases.Add(skillBase);
        }
        
        public void Update()
        {
            foreach (var skill in _skillBases)
            {
                skill.Update();
            }
        }

        public SkillBase GetCurrentCanCastSkill()
        {
            return _skillBases.FirstOrDefault(skill => skill.isReady);
        } 
    }
}