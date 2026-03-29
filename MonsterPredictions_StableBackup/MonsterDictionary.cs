using Godot;
namespace MonsterPredictions;
public static class MonsterDictionary {
    public static int GetStrengthGainFromMove(string monsterId, string stateId, int ascensionLevel) {
        switch(monsterId) {
            case "Axebot":
                if (stateId == "BOOT_UP_MOVE") return 1;
                if (stateId == "SHARPEN_MOVE") return 4;
                break;
            case "BowlbugNectar":
                if (stateId == "BUFF_MOVE") return ascensionLevel >= 2 ? 16 : 15;
                break;
            case "BruteRubyRaider":
                if (stateId == "ROAR_MOVE") return 3;
                break;
            case "BygoneEffigy":
                if (stateId == "WAKE_MOVE") return 10;
                break;
            case "CeremonialBeast":
                if (stateId == "PLOW_MOVE") return 2;
                if (stateId == "CRUSH_MOVE") return ascensionLevel >= 2 ? 4 : 3;
                break;
            case "Crusher":
                if (stateId == "ADAPT_MOVE") return 2;
                break;
            case "CubexConstruct":
                if (stateId == "CHARGE_UP_MOVE") return 2;
                if (stateId == "REPEATER_MOVE_2") return 2;
                break;
            case "DecimillipedeSegment":
                if (stateId == "BULK_MOVE") return 2;
                break;
            case "Door":
                if (stateId == "ENFORCE_MOVE") return ascensionLevel >= 2 ? 4 : 3;
                break;
            case "Doormaker":
                if (stateId == "GET_BACK_IN_MOVE") return 5;
                break;
            case "Entomancer":
                if (stateId == "PHEROMONE_SPIT_MOVE") return 1;
                break;
            case "Exoskeleton":
                if (stateId == "ENRAGE_MOVE") return 2;
                break;
            case "FlailKnight":
                if (stateId == "WAR_CHANT") return 3;
                break;
            case "Fogmog":
                if (stateId == "SWIPE_RANDOM_MOVE") return 1;
                break;
            case "FrogKnight":
                if (stateId == "FOR_THE_QUEEN") return 5;
                break;
            case "FuzzyWurmCrawler":
                if (stateId == "INHALE") return 7;
                break;
            case "GlobeHead":
                if (stateId == "GALVANIC_BURST") return 2;
                break;
            case "GremlinMerc":
                if (stateId == "HEHE_MOVE") return 2;
                break;
            case "InfestedPrism":
                if (stateId == "PULSATE_MOVE") return ascensionLevel >= 2 ? 5 : 4;
                break;
            case "KinFollower":
                if (stateId == "POWER_DANCE_MOVE") return ascensionLevel >= 2 ? 3 : 2;
                break;
            case "KinPriest":
                if (stateId == "RITUAL_MOVE") return ascensionLevel >= 2 ? 3 : 2;
                break;
            case "KnowledgeDemon":
                if (stateId == "PONDER_MOVE") return ascensionLevel >= 2 ? 3 : 2;
                break;
            case "LagavulinMatriarch":
                if (stateId == "SOUL_SIPHON_MOVE") return 2;
                break;
            case "LivingShield":
                if (stateId == "SMASH_MOVE") return 3;
                break;
            case "LouseProgenitor":
                if (stateId == "CURL_AND_GROW_MOVE") return 5;
                break;
            case "MechaKnight":
                if (stateId == "WINDUP_MOVE") return 5;
                break;
            case "Ovicopter":
                if (stateId == "NUTRITIONAL_PASTE_MOVE") return ascensionLevel >= 2 ? 4 : 3;
                break;
            case "PhantasmalGardener":
                if (stateId == "ENLARGE_MOVE") return ascensionLevel >= 2 ? 3 : 2;
                break;
            case "Queen":
                // MISSING PROP RESOLUTION: if (stateId == "BURN_BRIGHT_FOR_ME_MOVE") return strengthAmount;
                if (stateId == "ENRAGE_MOVE") return 2;
                break;
            case "Rocket":
                if (stateId == "CHARGE_UP_MOVE") return 2;
                break;
            case "ScrollOfBiting":
                if (stateId == "MORE_TEETH") return 2;
                break;
            case "Seapunk":
                if (stateId == "BUBBLE_BURP_MOVE") return ascensionLevel >= 2 ? 2 : 1;
                break;
            case "SewerClam":
                if (stateId == "PRESSURIZE_MOVE") return 4;
                break;
            case "SkulkingColony":
                if (stateId == "INERTIA_MOVE") return 3;
                break;
            case "SlimedBerserker":
                if (stateId == "LEECHING_HUG_MOVE") return 3;
                break;
            case "SludgeSpinner":
                if (stateId == "RAGE_MOVE") return 3;
                break;
            case "SlumberingBeetle":
                if (stateId == "ROLL_OUT_MOVE") return 2;
                break;
            case "SnappingJaxfruit":
                if (stateId == "ENERGY_ORB_MOVE") return 2;
                break;
            case "TestSubject":
                if (stateId == "BURNING_GROWL_MOVE") return ascensionLevel >= 2 ? 3 : 2;
                break;
            case "TheAdversaryMkOne":
                if (stateId == "BARRAGE_MOVE") return 2;
                break;
            case "TheAdversaryMkThree":
                if (stateId == "BARRAGE_MOVE") return 4;
                break;
            case "TheAdversaryMkTwo":
                if (stateId == "BARRAGE_MOVE") return 3;
                break;
            case "TheInsatiable":
                if (stateId == "SALIVATE_MOVE") return ascensionLevel >= 2 ? 3 : 2;
                break;
            case "TheLost":
                if (stateId == "DEBILITATING_SMOG") return 2;
                break;
            case "TheObscura":
                if (stateId == "SAIL_MOVE") return 3;
                break;
            case "Wriggler":
                if (stateId == "WRIGGLE_MOVE") return 2;
                break;
            case "Nibbit":
                if (stateId == "HISS_MOVE") return ascensionLevel >= 2 ? 3 : 2;
                break;
            case "TurretOperator":
                if (stateId == "RELOAD_MOVE") return ascensionLevel >= 2 ? 3 : 2;
                break;
        }
        return 0;
    }
}
