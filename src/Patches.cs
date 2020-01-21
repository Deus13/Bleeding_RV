using Harmony;
using System;


namespace Bleeding_RV
{
    [HarmonyPatch(typeof(BaseAi), "ApplyDamage", new Type[] { typeof(float), typeof(float), typeof(DamageSource), typeof(string) })]
    internal static class BaseAi_ApplyDamage
    {
        public static float BleedOutMintuesTmp;
        private static void Prefix(BaseAi __instance, float damage,ref float bleedOutMintues, DamageSource damageSource, string collider)
        {
            bleedOutMintues *= UnityEngine.Random.Range(0,1f)+ UnityEngine.Random.Range(0, 1f);
            BleedOutMintuesTmp = (float)AccessTools.Field(typeof(BaseAi), "m_DeathAfterBleeingOutMinutes").GetValue(__instance);
        }
        private static void Postfix(BaseAi __instance, float damage, float bleedOutMintues, DamageSource damageSource, string collider)
        {
            //Implementation.Log("post");

            if (BleedOutMintuesTmp > 0)
            {
                float b1 = bleedOutMintues;
                float b2 = BleedOutMintuesTmp - __instance.m_ElapsedBleedingOutMinutes;
                float bn = b1 * b2 / (b1 + b2);
                bn += __instance.m_ElapsedBleedingOutMinutes;
                AccessTools.Field(typeof(BaseAi), "m_DeathAfterBleeingOutMinutes").SetValue(__instance, bn);

                //Implementation.Log(bn.ToString() + "     " + __instance.m_CurrentHP.ToString());
            }
            //Implementation.Log(bleedOutMintues.ToString() + "     " + __instance.m_CurrentHP.ToString());
        }
    }

    [HarmonyPatch(typeof(BaseAi), "UpdateWounds", new Type[] { typeof(float) })]
    internal static class BaseAi_UpdateWounds
    {
        private static void Postfix(BaseAi __instance, float realtimeSeconds)
        {
            float Minutes = GameManager.GetTimeOfDayComponent().GetTODMinutes(realtimeSeconds);
            Implementation.UpdateWounds(__instance, Minutes);


        }
    }

    
    [HarmonyPatch(typeof(BodyDamage), "GetDamageById", new Type[] { typeof(BodyPart)})]
    internal static class BodyDamage_GetDamageById
    {
        private static void Postfix(BodyDamage __instance,ref DamageRegion __result, BodyPart bodyPart)
        {
            if(__result.m_DoNotBleedOut == true)
            {
                Implementation.Log("Bodypart:" + __result.m_bodyPart.ToString());

                __result.m_DoNotBleedOut = false;
                if (bodyPart == BodyPart.head)
                {
                    __result.m_BleedOutMinutes = 120;
                    __result.m_BleedOutMinutesForArrow = 90;
                    __result.m_BleedOutMinutesForRevolver = 160;
                    __result.m_BleedOutMinutesForFlareGunRound = 60;
                }
                if (bodyPart == BodyPart.neck)
                {
                    __result.m_BleedOutMinutes = 180;
                    __result.m_BleedOutMinutesForArrow = 120;
                    __result.m_BleedOutMinutesForRevolver = 240;
                    __result.m_BleedOutMinutesForFlareGunRound = 90;
                }
                if (bodyPart == BodyPart.torso)
                {
                    __result.m_BleedOutMinutes = 480;
                    __result.m_BleedOutMinutesForArrow = 750;
                    __result.m_BleedOutMinutesForRevolver = 640;
                    __result.m_BleedOutMinutesForFlareGunRound = 100;
                }
                if (bodyPart == BodyPart.hips)
                {
                    __result.m_BleedOutMinutes = 720;
                    __result.m_BleedOutMinutesForArrow = 480;
                    __result.m_BleedOutMinutesForRevolver = 960;
                    __result.m_BleedOutMinutesForFlareGunRound = 140;
                }
                if (bodyPart == BodyPart.limb)
                {
                    __result.m_BleedOutMinutes = 960;
                    __result.m_BleedOutMinutesForArrow = 480;
                    __result.m_BleedOutMinutesForRevolver = 1280;
                    __result.m_BleedOutMinutesForFlareGunRound = 280;
                }
            }

            //not finished
            Implementation.Log("m_BleedOutMinutes:"+ __result.m_bodyPart.ToString());

            Implementation.Log("m_BleedOutMinutes: " + __result.m_BleedOutMinutes.ToString());
            Implementation.Log("m_BleedOutMinutesForArrow: " + __result.m_BleedOutMinutesForArrow.ToString());
            Implementation.Log("m_BleedOutMinutesForRevolver: " + __result.m_BleedOutMinutesForRevolver.ToString());
            Implementation.Log("m_BleedOutMinutesForFlareGunRound: " + __result.m_BleedOutMinutesForFlareGunRound.ToString());


            //Implementation.Log(BleedOutMintues.ToString() + "     " + __instance.m_CurrentHP.ToString());
        }
    }


    
    [HarmonyPatch(typeof(BaseAi), "DeserializeUsingBaseAiDataProxy", new Type[] { typeof(BaseAiDataProxy) })]
    internal static class BaseAi_DeserializeUsingBaseAiDataProxy
    {
        private static void Postfix(BaseAi __instance, BaseAiDataProxy  proxy)
        {

            float num = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused() - proxy.m_HoursPlayed;
            num *= 60f;
            while (num>1)
            {
                Implementation.UpdateWounds(__instance, 1);                         //small septs so the bleed reductions can update
                num -= 1;   
            }
            Implementation.UpdateWounds(__instance, num);


        }
    }
}