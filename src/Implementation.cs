using UnityEngine;
using Harmony;

namespace Bleeding_RV
{
    public class Implementation
    {
        private const string NAME = "Bleeding_RV";


        public static void OnLoad()
        {
            Log("Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }

        public static void UpdateWounds(BaseAi ba, float mins)
        {
            
            if (ba.IsBleedingOut())
            {
                float BleedOutMintues = (float)AccessTools.Field(typeof(BaseAi), "m_DeathAfterBleeingOutMinutes").GetValue(ba);


                ba.m_CurrentHP -= ba.m_MaxHP * mins / BleedOutMintues;

                BleedOutMintues *= Mathf.Exp(mins / (60f));
                if (BleedOutMintues > 500000f)
                {
                    AccessTools.Field(typeof(BaseAi), "m_BleedingOut").SetValue(ba, false);
                    BleedOutMintues = 0;
                    ba.m_ElapsedBleedingOutMinutes = 0;
                }

                AccessTools.Field(typeof(BaseAi), "m_DeathAfterBleeingOutMinutes").SetValue(ba, BleedOutMintues);


                Implementation.Log(BleedOutMintues.ToString() + "     " + ba.m_CurrentHP.ToString());

                if (ba.m_CurrentHP <= 0)
                {
                    ba.m_CurrentHP = 0;
                    BaseAi.DamageSide damageSide = (BaseAi.DamageSide)AccessTools.Field(typeof(BaseAi), "m_LastDamageSide").GetValue(ba);
                    int Part = (int)AccessTools.Field(typeof(BaseAi), "m_LastDamageBodyPart").GetValue(ba);


                    System.Object[] temp1 = { damageSide, Part, BaseAi.SetupDamageParamsOptions.None };
                    AccessTools.Method(typeof(BaseAi), "SetDamageImpactParameter").Invoke(ba, temp1);
                    System.Object[] temp2 = { AiMode.Dead };                                      //a copy the game code
                    AccessTools.Method(typeof(BaseAi), "SetAiMode").Invoke(ba, temp2);
                    // __instance.m_ElapsedBleedingOutMinutes = float.PositiveInfinity;                                                                             //but tricking the base code did not work                 
                }
            }

            if (ba.m_Wounded)
            {
                ba.m_CurrentHP += ba.m_CurrentHP / ba.m_MaxHP * mins / 60f;           //up to one HP 1 per hour
                if (ba.m_CurrentHP > ba.m_MaxHP)
                {
                    ba.m_CurrentHP = ba.m_MaxHP;
                    ba.m_Wounded = false;
                }
            }
        }

        internal static void Log(string message)
        {
            Debug.LogFormat("[" + NAME + "] {0}", message);
        }

        internal static void Log(string message, params object[] parameters)
        {
            string preformattedMessage = string.Format("[" + NAME + "] {0}", message);
            Debug.LogFormat(preformattedMessage, parameters);
        }
    }
}