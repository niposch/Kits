using System;
using JetBrains.Annotations;
using Rocket.Unturned.Skills;
using System.Reflection;

namespace fr34kyn01535.Kits.Helper
{
    public class SkillEnumHelper
    {
        [CanBeNull]
        public static UnturnedSkill GetSkillEnum(string skillName)
        {
            Type unturnedSkills = typeof(UnturnedSkill);
            try
            {
                var field = unturnedSkills.GetField(skillName);
                return (UnturnedSkill)field.GetValue(null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unturned Skill {skillName} not found!");
                return null;
            }
        }
    }
}