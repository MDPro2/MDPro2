using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class VoiceMap : MonoBehaviour
{
    public static int Map(string character, string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (character)
        {
            case "V0001":
                return V0001(type, card, firstOnField, location, chain);
            case "V0002":
                return V0002(type, card, firstOnField, location, chain);
            case "V0003":
                return V0003(type, card, firstOnField, location, chain);
            case "V0106":
                return V0106(type, card, firstOnField, location, chain);
            case "V0209":
                return V0209(type, card, firstOnField, location, chain);
            case "V0303":
                return V0303(type, card, firstOnField, location, chain);
            case "V0307":
                return V0307(type, card, firstOnField, location, chain);
            case "V0404":
                return V0404(type, card, firstOnField, location, chain);
            case "V0501":
                return V0501(type, card, firstOnField, location, chain);
            case "V0603":
                return V0603(type, card, firstOnField, location, chain);
            case "V9995":
                return V9995(type, card, firstOnField, location, chain);
        }
        return -10;
    }
    //effect activate                     08
    //monster effect activate       09
    //summon                              13
    //attack                                  19
    //summonline                        37
    static int V0001(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 46986414:
                        return 0;
                    case 38033121:
                        return 1;
                    case 10000020:
                        return 2;
                    case 40640057:
                        return 3;
                    case 10000000:
                        return 4;
                    case 10000010:
                        return 5;
                    case 10000040:
                        return 6;
                    case 70781052:
                        return 7;
                    case 43892408:
                        return 8;
                    case 75380687:
                        return 9;
                    case 80019195:
                        return 10;
                    case 30208479:
                        return 11;
                    case 14778250:
                        //mark
                        if(true)
                            return 12;
                    case 10000030:
                        return 13;
                    case 96471335:
                        return 14;
                    case 99999999:
                        return 15;
                    case 5405694:
                        return 16;
                    case 53315891:
                        return 17;
                    case 83743222:
                        return 18;
                }
                break;
            case "attack":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 46986414:
                        return 0;
                    case 38033121:
                        return 1;
                    case 10000020:
                        return 2;
                    case 10000000:
                        return 3;
                    case 10000010:
                        return 4;
                    case 70781052:
                        return 5;
                    case 43892408:
                        return 6;
                    case 75380687:
                        return 7;
                    case 80019195:
                        return 8;
                    case 30208479:
                        return 9;
                }
                break;
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 44095762:
                        return 0;
                    case 28553439:
                        return 1;
                    case 62279055:
                        return 2;
                    case 83764718:
                        return 3;
                    case 72302403:
                        return 4;
                    case 25774450:
                        return 5;
                    case 9287078:
                        return 6;
                    case 1784686:
                        return 7;
                    case 89397517:
                        return 8;
                    case 15381252:
                        return 9;
                    case 42776960:
                        return 10;
                    case 32754886:
                        return 11;
                    case 76792184:
                        return 12;
                    case 40703222:
                        return 13;
                    case 89086566:
                        return 14;
                    case 55761792:
                        return 15;
                    case 63391643:
                        return 16;
                    case 24094653:
                        return 17;
                    case 24874630:
                        return 18;
                    case 48179391:
                        return 19;
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 10000020:
                        return 0;
                    case 10000000:
                        return 1;
                    case 10000010:
                        if(firstOnField)
                            //mark
                            return 2;
                        else
                            return 3;
                    case 40640057:
                        return 4;
                }
                break;
        }
        return -10;
    }
    static int V0002(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 89631139:
                        return 0;
                    case 23995346:
                        return 1;
                    case 10000000:
                        return 2;
                    case 91998119:
                        return 3;
                    case 2111707:
                        return 4;
                    case 99724761:
                        return 5;
                    case 25119460:
                        return 6;
                    case 82301904:
                        return 7;
                    case 99999999:
                        return 8;
                    case 34627841:
                        return 9;
                    case 14898066:
                        return 10;
                    case 22804644:
                        return 11;
                    case 84687358:
                        return 12;
                    case 58293343:
                        return 13;
                    case 53839837:
                        return 14;
                    case 53347303:
                        return 15;
                    case 50939127:
                        return 16;
                    case 52824910:
                        return 17;
                    case 62651957:
                        return 18;
                    case 65622692:
                        return 19;
                    case 64500000:
                        return 20;
                    case 30012506:
                        return 21;
                    case 77411244:
                        return 22;
                    case 3405259:
                        return 23;
                    case 1561110:
                        return 24;
                    case 65172015:
                        return 25;
                    case 45467446:
                        return 26;
                    case 57043986:
                        return 27;
                    case 8978197:
                        return 28;
                }
                break;
            case "attack":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 89631139:
                        return 0;
                    case 23995346:
                        return 1;
                    case 10000000:
                        return 2;
                    case 91998119:
                        return 3;
                    case 50939127:
                        return 4;
                    case 52824910:
                        return 5;
                    case 84687358:
                        return 6;
                    case 53839837:
                        return 7;
                    case 53347303:
                        return 8;
                    case 82301904:
                        return 9;
                    case 22804644:
                        return 10;
                    case 58293343:
                        return 11;
                    case 17985575:
                        return 12;
                    case 66602787:
                        return 13;
                    case 62651957:
                        return 14;
                    case 65622692:
                        return 15;
                    case 64500000:
                        return 16;
                    case 2111707:
                        return 17;
                    case 99724761:
                        return 19;
                    case 25119460:
                        return 20;
                    case 30012506:
                        return 21;
                    case 77411244:
                        return 22;
                    case 3405259:
                        return 23;
                    case 1561110:
                        return 24;
                    case 65172015:
                        return 25;
                    case 45467446:
                        return 26;
                    case 57043986:
                        return 27;
                    case 8978197:
                        return 28;
                    case 34627841:
                        return 29;
                    case 14898066:
                        return 30;
                }
                break;
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 68005187:
                        return 0;
                    case 57728570:
                        return 1;
                    case 83764718:
                        return 2;
                    case 35027493:
                        return 3;
                    case 54974237:
                        return 4;
                    case 83555666:
                        return 5;
                    case 58641905:
                        return 6;
                    case 21219755:
                        return 7;
                    case 24874630:
                        return 8;
                    case 98045062:
                        return 9;
                    case 22046459:
                        return 10;
                    case 24094653:
                        return 11;
                    case 11082056:
                        return 12;
                    case 39238953:
                        return 13;
                    case 59750328:
                        return 14;
                    case 42534368:
                        return 15;
                    case 29432790:
                        return 16;
                    case 43845801:
                        return 17;
                    case 93437091:
                        return 18;
                    case 17655904:
                        return 19;
                    case 2783661:
                        return 20;
                    case 50371210:
                        return 21;
                    case 86871614:
                        return 22;
                    case 36261276:
                        return 23;
                    case 52503575:
                        return 24;
                    case 54591086:
                        return 25;
                    case 56920308:
                        if((card.p.location & (uint)CardLocation.Onfield) > 0)
                            return 26;
                        else
                            return 27;
                    case 48800175:
                        return 28;
                    case 71867500:
                        return 29;
                    case 23265313:
                        return 30;
                    case 43973174:
                        return 31;
                    case 55713623:
                        return 32;
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 10000000:
                        if(Program.I().ocgcore.MD_phaseString == "End")
                            return 2;
                        else
                            return 0;
                    case 91998119:
                        return 3;
                    case 82301904:
                        return 4;
                    case 84687358:
                        return 5;
                    case 53839837:
                        if(Program.I().ocgcore.MD_phaseString == "Battle")
                            return 6;
                        else
                            return 7;
                    case 53347303:
                        return 8;
                    case 34627841:
                        return 9;
                    case 22804644:
                        return 10;
                    case 58293343:
                        return 11;
                    case 52824910:
                        return 12;
                    case 65622692:
                        return 13;
                    case 64500000:
                        return 14;
                    case 2111707:
                        return 15;
                    case 99724761:
                        return 16;
                    case 25119460:
                        return 17;
                    case 30012506:
                        return 18;
                    case 77411244:
                        return 19;
                    case 3405259:
                        return 20;
                    case 1561110:
                        return 21;
                    case 65172015:
                        return 22;
                    case 45467446:
                        return 23;
                    case 57043986:
                        //mark
                        if(chain == 1)
                            return 24;
                        else
                            return 25;
                    case 8978197:
                        return 26;
                }
                break;
        }
        return -10;
    }
    static int V0003(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 74677422:
                        return 0;
                    case 41462083:
                        return 1;
                    case 77585513:
                        return 2;
                    case 36354007:
                        return 3;
                    case 57902462:
                        return 4;
                    case 45349196:
                        return 5;
                    case 50903514:
                        return 6;
                    case 64335804:
                        return 7;
                    case 22091647:
                        return 8;
                    case 4722253:
                        return 8;
                    case 69488544:
                        return 8;
                    case 85651167:
                        return 8;
                    case 49161188:
                        return 8;
                    case 57046845:
                        return 9;
                    case 42035044:
                        return 10;
                    case 45231177:
                        return 11;
                    case 10960419:
                        return 12;
                    case 46354113:
                        return 13;
                    case 19747827:
                        return 14;
                    case 19025379:
                        return 15;
                    case 71625222:
                        return 16;
                    case 88819587:
                        return 17;
                    case 26376390:
                        return 18;
                    case 48305365:
                        return 19;
                    case 55550921:
                        return 20;
                    case 3573512:
                        return 21;
                    case 51828629:
                        return 22;
                    case 3643300:
                        return 23;
                    case 90790253:
                        return 24;
                    case 64428736:
                        return 25;
                    case 3366982:
                        return 26;
                    case 30860696:
                        return 27;
                    case 423705:
                        return 28;
                    case 11901678:
                        return 29;
                    case 90660762:
                        return 30;
                    case 30086349:
                        return 30;
                }
                break;
            case "attack":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 99999999:
                        return 0;
                    case 41462083:
                        return 1;
                    case 77585513:
                        return 2;
                    case 102380:
                        return 3;
                    case 36354007:
                        return 4;
                    case 57902462:
                        return 5;
                    case 45231177:
                        return 6;
                    case 11901678:
                        return 7;
                    case 64335804:
                        return 8;
                    case 423705:
                        return 9;
                    case 57046845:
                        return 10;
                    case 30860696:
                        return 11;
                    case 42035044:
                        return 12;
                    case 55550921:
                        return 13;
                    case 82556058:
                        return 14;
                    case 71625222:
                        return 15;
                    case 88819587:
                        return 16;
                    case 26376390:
                        return 17;
                    case 48305365:
                        return 18;
                    case 3573512:
                        return 19;
                    case 51828629:
                        return 20;
                    case 77406972:
                        return 20;
                    case 3643300:
                        return 21;
                    case 90790253:
                        return 22;
                    case 64428736:
                        return 23;
                    case 3366982:
                        return 24;
                    case 74677422:
                        return 25;
                    case 90660762:
                        return 26;
                    case 30086349:
                        return 26;
                }
                break;
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 4206964:
                        return 0;
                    case 73915051:
                        return 1;
                    case 21598948:
                        return 2;
                    case 42703248:
                        return 3;
                    case 61705417:
                        return 4;
                    case 74137509:
                        return 5;
                    case 126218:
                        return 6;
                    case 75417459:
                        return 7;
                    case 76895648:
                        return 8;
                    case 96008713:
                        return 9;
                    case 52097679:
                        return 10;
                    case 18756904:
                        return 11;
                    case 35762283:
                        return 12;
                    case 37390589:
                        return 13;
                    case 24094653:
                        return 14;
                    case 36708764:
                        return 15;
                    case 46232525:
                        return 16;
                    case 45410988:
                        return 17;
                    case 38723936:
                        return 18;
                    case 81439173:
                        return 19;
                    case 52684508:
                        return 20;
                    case 86318356:
                        return 21;
                    case 32268901:
                        return 22;
                    case 94716515:
                        return 23;
                    case 55226821:
                        return 24;
                    case 68540058:
                        return 25;
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 71625222:
                        return Random.Range(0, 2);
                    case 77585513:
                        return 2;
                    case 36354007:
                        return 3;
                    case 50903514:
                        if((card.p.location & (uint)CardLocation.Grave) > 0)
                            return 4;
                        else
                            return 5;
                    case 423705:
                        return 6;
                    case 57046845:
                        return 7;
                    case 26376390:
                        return 8;
                    case 90790253:
                        return 9;
                    case 30860696:
                        return 10;
                }
                break;
        }
        return -10;
    }
    static int V0106(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 11460577:
                        return 0;
                    case 32750510:
                        return 1;
                    case 24661486:
                        return 2;
                    case 77235086:
                        return 3;
                    case 39618799:
                        return 4;
                    case 3629090:
                        return 5;
                    case 76763417:
                        return 6;
                    case 49375719:
                        return 7;
                    case 76103404:
                        return 8;
                    case 2158562:
                        return 9;
                    case 10248389:
                        return 10;
                    case 61802346:
                        return 11;
                    case 97023549:
                        return 12;
                    case 79473793:
                        return 13;
                    case 20193924:
                        return 14;
                    case 14462257:
                        return 15;
                    case 66690411:
                        return 16;
                    case 28348537:
                        return 17;
                    case 99427357:
                        return 18;
                    case 78316184:
                        return 19;
                    case 42600274:
                        return 20;
                    case 38142739:
                        return 21;
                }
                break;
            case "attack":
                return V0106("summon", card, firstOnField, location, chain);
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 5318639:
                        return 0;
                    case 73915051:
                        return 1;
                    case 79997591:
                        return 2;
                    case 27967615:
                        return 3;
                    case 33550694:
                        return 4;
                    case 88789641:
                        return 5;
                    case 54351224:
                        return 6;
                    case 97077563:
                        return 7;
                    case 7625614:
                        return 8;
                    case 39996157:
                        return 9;
                    case 55144522:
                        return 10;
                    case 48206762:
                        return 11;
                    case 95658967:
                        return 12;
                    case 95281259:
                        return 13;
                    case 60764581:
                        return 14;
                    case 24094653:
                        return 15;
                    case 18511384:
                        return 16;
                    case 99999999:
                        return 17;
                    case 49306994:
                        return 18;
                    case 27551:
                        return 19;
                    case 11398951:
                        return 20;
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 11460577:
                        return 0;
                    case 32750510:
                        return 1;
                    case 24661486:
                        return 2;
                    case 77235086:
                        return 3;
                    case 39618799:
                        return 4;
                    case 3629090:
                        return 5;
                    case 76763417:
                        return 6;
                    case 76103404:
                        return 7;
                    case 2158562:
                        return 8;
                    case 10248389:
                        return 9;
                    case 61802346:
                        return 10;
                    case 79473793:
                        return 11;
                    case 20193924:
                        return 13;
                    case 14462257:
                        return 14;
                    case 28348537:
                        return 15;
                    case 99427357:
                        return 16;
                    case 78316184:
                        return 17;
                    case 42600274:
                        return 18;
                }
                break;
        }
        return -10;
    }
    static int V0209(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 63676256:
                        return 0;
                    case 48633301:
                        return 1;
                    case 30348744:
                        return 2;
                    case 74627016:
                        return 3;
                    case 64910482:
                        return 4;
                    case 94350039:
                        return 5;
                    case 66733743:
                        return 6;
                    case 1315120:
                        return 7;
                    case 64898834:
                        return 8;
                    case 293542:
                        return 9;
                    case 37300735:
                        return 10;
                    case 36687247:
                        return 11;
                    case 11234702:
                        return 12;
                    case 62560742:
                        return 13;
                    case 98558751:
                        return 14;
                    case 24943456:
                        return 15;
                    case 90953320:
                        return 16;
                    case 51447164:
                        return 17;
                    case 97836203:
                        return 18;
                    case 47027714:
                        return 19;
                    case 99937842:
                        return 20;
                    case 74627017:
                        return 21;
                }
                break;
            case "attack":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 74627017:
                        return -10;
                }
                return V0209("summon", card);
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 11264180:
                        return 0;
                    case 58258899:
                        return 1;
                    case 40253382:
                        return 2;
                    case 3868277:
                        return 3;
                    case 76641981:
                        return 4;
                    case 93138457:
                        return 5;
                    case 7811875:
                        return 6;
                    case 50951359:
                        return 7;
                    case 14507213:
                        return 8;
                    case 94634433:
                        return 9;
                    case 94192409:
                        return 10;
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 63676256:
                        if ((card.p.location & (uint)CardLocation.SpellZone) > 0)
                            return 1;
                        else
                            return 0;
                    case 48633301:
                        return 2;
                    case 30348744:
                        if (Program.I().ocgcore.MD_phaseString == "Battle")
                            return 4;
                        else
                            return 3;
                    case 74627016:
                        return 5;
                    case 64910482:
                        return 6;
                    case 94350039:
                        return 7;
                    case 66733743:
                        return 8;
                    case 1315120:
                        return 9;
                    case 64898834:
                        return 10;
                    case 293542:
                        if((card.p.location & (uint)CardLocation.Hand) > 0)
                            return 11;
                        else
                            return 12;
                    case 37300735:
                        return 13;
                    case 36687247:
                        return 14;
                    case 11234702:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 15;
                        else
                            return 16;
                    case 62560742:
                        return 17;
                    case 98558751:
                        //mark
                        if ((card.p.location & (uint)CardLocation.Onfield) == 0)
                            return 20;
                        else if(Program.I().ocgcore.myTurn && card.p.controller == 0)
                            return 18;
                        else if (!Program.I().ocgcore.myTurn && card.p.controller == 1)
                            return 18;
                        else
                            return 19;
                    case 24943456:
                        return 21;
                    case 90953320:
                        return 22;
                    case 51447164:
                        //mark
                        if (chain > 1)
                            return 23;
                        else if(Program.I().ocgcore.MD_phaseString == "Standby")
                            return 25;
                        else
                            return 24;
                    case 97836203:
                        if ((card.p.location & (uint)CardLocation.Grave) > 0)
                            return 27;
                        else
                            return 26;
                    case 47027714:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 28;
                        else
                            return 29;
                    case 99937842:
                        //mark
                        Debug.Log("效果数量" + card.effects.Count);
                        if (Program.I().ocgcore.myTurn == false)
                            return 32;
                        else
                            return 30;
                }
                break;
            case "summonline":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 62560742:
                        return 0;
                    case 98558751:
                        return 1;
                    case 24943456:
                        return 2;
                    case 90953320:
                        return 3;
                    case 51447164:
                        return 4;
                    case 97836203:
                        return 5;
                    case 99937842:
                        return 6;
                }
                break;
        }
        return -10;
    }
    static int V0303(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 57354389:
                        return 0;
                    case 93749093:
                        return 1;
                    case 46613515:
                        return 2;
                    case 8910971:
                        return 3;
                    case 13839120:
                        return 4;
                    case 41172955:
                        return 5;
                    case 86445415:
                        return 6;
                    case 29021114:
                        return 7;
                    case 55010259:
                        return 8;
                    case 1995985:
                        return 9;
                    case 74388798:
                        return 10;
                    case 37267041:
                        return 11;
                    case 15180041:
                        return 12;
                    case 73665146:
                        return 13;
                    case 72443568:
                        return 14;
                    case 41175645:
                        return 15;
                    case 64681432:
                        return 16;
                    case 58330108:
                        return 17;
                    case 71525232:
                        return 18;
                    case 20747792:
                        return 19;
                    case 34318086:
                        return 20;
                    case 56132807:
                        return 21;
                    case 7198399:
                        return 22;
                    case 82627406:
                        return 23;
                    case 38033121:
                        return 24;
                    case 46986414:
                        return 25;
                    case 47963370:
                        return 26;
                    case 79613121:
                        return 27;
                    case 43959432:
                        return 28;
                    case 42237854:
                        return 29;
                    case 13955608:
                        return 30;
                }
                break;
            case "attack":
                return V0303("summon", card);
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 5318639:
                        return 0;
                    case 83764718:
                        return 1;
                    case 18809562:
                        return 2;
                    case 44968459:
                        if((card.p.location & (uint)CardLocation.SpellZone) > 0)
                            return 3;
                        else
                            return 4;
                    case 18563744:
                        if ((card.p.location & (uint)CardLocation.SpellZone) > 0)
                            return 5;
                        else
                            return 6;
                    case 73616671:
                        return 7;
                    case 47222536:
                        if(firstOnField)
                            return 8;
                        else
                            return 9;
                    case 41735184:
                        return 10;
                    case 60709218:
                        return 11;
                    case 70168345:
                        return 12;
                    case 49702428:
                        return 13;
                    case 75190122:
                        return 14;
                    case 2314238:
                        return 15;
                    case 111280:
                        return 16;
                    case 21082832:
                        return 17;
                    case 16964437:
                        return 18;
                    case 54175023:
                        return 19;
                    case 17787975:
                        if(firstOnField)
                            return 20;
                        else
                            return 21;
                    case 80560728:
                        if(firstOnField)
                            return 22;
                        else
                            return 23;
                    case 16832845:
                        return 24;
                    case 89448140:
                        if((card.p.location & (uint)CardLocation.Grave) > 0)
                            return 26;
                        else
                            return 25;
                    case 81782376:
                        if ((card.p.location & (uint)CardLocation.Onfield) > 0)
                            return 27;
                        else
                            return 28;
                    case 78625592:
                        if((card.p.location & (uint)CardLocation.Onfield) == 0)
                            return 31;
                        else if(firstOnField)
                            return 29;
                        else
                            return 30;
                    case 44095762:
                        return 32;
                    case 62279055:
                        return 33;
                    case 48680970:
                        if ((card.p.location & (uint)CardLocation.Onfield) == 0)
                            return 36;
                        else if (firstOnField)
                            return 34;
                        else
                            return 35;
                    case 68334074:
                        return 37;
                    case 7922915:
                        if ((card.p.location & (uint)CardLocation.Onfield) > 0)
                            return 38;
                        else
                            return 39;
                    case 43959432:
                        return 41;
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 57354389:
                        return 0;
                    case 93749093:
                        return 1;
                    case 46613515:
                        if((card.p.location & (uint)CardLocation.Hand) > 0)
                            return 2;
                        else
                            return 3;
                    case 8910971:
                        return 4;
                    case 13839120:
                        return 5;
                    case 41172955:
                        return 6;
                    case 86445415:
                        return 7;
                    case 29021114:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 8;
                        else
                            return 9;
                    case 55010259:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 10;
                        else
                            return 11;
                    case 1995985:
                        return 12;
                    case 74388798:
                        //mark
                        if ((card.p.location & (uint)CardLocation.MonsterZone) == 0)
                            return 16;
                        else if (Program.I().ocgcore.MD_phaseString == "Standby")
                            return 14;
                        else
                            return 15;
                    case 73665146:
                        return 17;
                    case 41175645:
                        if((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 18;
                        else
                            return 19;
                    case 64681432:
                        if (Program.I().ocgcore.MD_phaseString == "End")
                            return 21;
                        else
                            return 20;
                    case 58330108:
                        return 22;
                    case 71525232:
                        //mark
                        if (Program.I().ocgcore.MD_phaseString == "End")
                            return 24;
                        else
                            return 23;
                    case 20747792:
                        //mark
                        if(Program.I().ocgcore.MD_phaseString == "Battle")
                            return 26;
                        else if(chain > 1)
                            return 26;
                        else
                            return 25;
                    case 34318086:
                        //mark
                        if (Program.I().ocgcore.MD_phaseString == "Battle")
                            return 28;
                        else
                            return 27;
                    case 56132807:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) == 0)
                            return 30;
                        else
                            return 29;
                    case 7198399:
                        if (Program.I().ocgcore.MD_phaseString == "Battle")
                            return 32;
                        else
                            return 31;
                    case 82627406:
                        return 33;
                    case 47963370:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 34;
                        else
                            return 35;
                    case 79613121:
                        //mark
                        if (Program.I().ocgcore.MD_phaseString == "Battle")
                            return 37;
                        else
                            return 36;
                }
                break;
        }
        return -10;
    }
    static int V0307(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 20137754:
                        return 0;
                    case 15610297:
                        return 1;
                    case 41114306:
                        return 2;
                    case 78509901:
                        return 3;
                    case 4998619:
                        return 4;
                    case 40392714:
                        return 5;
                    case 77387463:
                        return 6;
                    case 3775068:
                        return 7;
                    case 30270176:
                        return 8;
                    case 72664875:
                        return 9;
                }
                break;
            case "attack":
                return V0307("summon", card);
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 9659580:
                        if(firstOnField)
                            return 0;
                        else if((location & (uint)CardLocation.Onfield) > 0)
                            return 1;
                        else
                            return 2;
                    case 35058588:
                        if((location & (uint)CardLocation.Onfield) > 0)
                            return 3;
                        else
                            return 4;
                    case 1218214:
                        if (firstOnField)
                            return 5;
                        else if ((location & (uint)CardLocation.Onfield) > 0)
                            return 6;
                        else
                            return 7;
                    case 71442223:
                        return 8;
                    case 8837932:
                        return 9;
                    case 34325937:
                        if ((location & (uint)CardLocation.Onfield) > 0)
                            return 10;
                        else
                            return 11;
                    case 2434862:
                        if ((location & (uint)CardLocation.Onfield) > 0)
                            return 12;
                        else
                            return 13;
                    case 38606913:
                        if ((location & (uint)CardLocation.Onfield) > 0)
                            return 14;
                        else
                            return 15;
                    case 99999999:
                        if(firstOnField)
                            return 16;
                        else
                            return 17;
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 20137754:
                        return 0;
                    case 15610297:
                        if ((location & (uint)CardLocation.MonsterZone) > 0)
                            return Random.Range(1, 3);
                        else
                            return 3;
                    case 41114306:
                        return 4;
                    case 78509901:
                        return 5;
                    case 4998619:
                        return 6;
                    case 40392714:
                        return 7;
                    case 77387463:
                        return 8;
                    case 3775068:
                        return 9;
                    case 30270176:
                        if(Program.I().ocgcore.MD_phaseString == "Battle")
                            return 10;
                        else
                            return 11;
                    case 72664875:
                        //mark
                        if (Program.I().ocgcore.MD_phaseString == "Battle" && Program.I().ocgcore.MD_battleString == "01")
                            return 12;
                        else if (Program.I().ocgcore.MD_phaseString == "Battle")
                            return 13;
                        else
                            return 14;
                }
                break;
        }
        return -10;
    }
    static int V0404(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {

                }
                break;
            case "attack":
                return V0209("summon", card);
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {

                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {

                }
                break;
            case "summonline":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {

                }
                break;
        }
        return -10;
    }
    static int V0501(string type, gameCard card, bool firstOnField = true,uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 16178681:
                        return 0;
                    case 53025096:
                        return 1;
                    case 19221310:
                        return 2;
                    case 21770839:
                        return 3;
                    case 93149655:
                        return 4;
                    case 21250202:
                        return 5;
                    case 67754901:
                        return 6;
                    case 83347294:
                        return 7;
                    case 72378329:
                        return 8;
                    case 88305705:
                        return 9;
                    case 1516510:
                        return 10;
                    case 82044279:
                        return 11;
                    case 41209827:
                        return 12;
                    case 16195942:
                        return 13;
                    case 1621413:
                        return 14;
                    case 86238081:
                        return 15;
                    case 45627618:
                        return 16;
                    case 45014450:
                        return 17;
                    case 58074177:
                        return 18;
                    case 70335319:
                        return 19;
                    case 16306932:
                        return 20;
                    case 42562690:
                        return 21;
                    case 79967395:
                        return 22;
                    case 91449532:
                        return 23;
                    case 82224646:
                        return 24;
                    case 46136942:
                            return 25;
                    case 86157908:
                        return 26;
                    case 78835747:
                        return 27;
                    case 26270847:
                        return 28;
                    case 15978426:
                        return 29;
                    case 15452043:
                        return 30;
                    case 17857780:
                        return 31;
                    case 41440148:
                        return 32;
                    case 91584698:
                        return 33;
                    case 42002073:
                        return 34;
                    case 43241495:
                        return 35;
                    case 9106362:
                        return 36;
                    case 69211541:
                        return 37;
                    case 4239451:
                        return 38;
                    case 96606246:
                        return 39;
                    case 44364077:
                        return 40;
                    case 17330916:
                        return 41;
                    case 44481227:
                        return 42;
                    case 73130445:
                        return 43;
                    case 9000988:
                        return 44;
                    case 59123194:
                        return 45;
                    case 33656832:
                        return 46;
                    case 45667991:
                        return 47;
                    case 80335817:
                        return 48;
                    case 20409757:
                        return 49;
                    case 94415058:
                        return 50;
                    case 71692913:
                        return 51;
                    case 17086528:
                        return 52;
                    case 54941203:
                        return 53;
                    case 80896940:
                        return 54;
                    case 18027139:
                        return 55;
                    case 11050416:
                        return 55;
                }
                break;
            case "attack":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 16178681:
                        return Random.Range(0, 2);
                }
                return V0501("summon", card) +1;
            case "summonline":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 16178681:
                        return 0;
                    case 53025096:
                        return 1;
                    case 19221310:
                        return 2;
                    case 21770839:
                        return 3;
                    case 83347294:
                        return 4;
                    case 72378329:
                        return 5;
                    case 88305705:
                        return 6;
                    case 1516510:
                        return 7;
                    case 82044279:
                        return 8;
                    case 41209827:
                        return 9;
                    case 16195942:
                        return 10;
                    case 1621413:
                        return 11;
                    case 86238081:
                        return 12;
                    case 45627618:
                        return 13;
                    case 45014450:
                        return 14;
                    case 58074177:
                        return 15;
                    case 16306932:
                        return 16;
                    case 59123194:
                        return 17;
                    case 80896940:
                        return 18;
                }
                break;
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 71705144:
                        return 0;
                    case 37803970:
                        return 1;
                    case 62161698:
                        return 2;
                    case 46066477:
                        return 3;
                    case 19162134:
                        return 4;
                    case 18027138:
                        return 5;
                    case 14816688:
                        return 6;
                    case 47870325:
                        if(firstOnField)
                            return 7;
                        else
                            return 8;
                    case 35259350:
                        return 9;
                    case 2099841:
                        return 10;
                    case 37469904:
                        return 11;
                    case 41367003:
                        return 12;
                    case 83461421:
                        return 13;
                    case 18752707:
                        return 14;
                    case 5972394:
                        return 15;
                    case 78574395:
                        if(firstOnField)
                            return 16;
                        else
                            return 17;
                    case 34884015:
                        if(firstOnField)
                            return 18;
                        else
                            return 19;
                    case 11050415:
                        return 20;
                    case 82768499:
                        return 21;
                    case 11481610:
                        return 22;
                    case 55553602:
                        if (firstOnField)
                            return 23;
                        else
                            return 24;
                    case 27813661:
                        if (firstOnField)
                            return 25;
                        else
                            return 26;
                    case 22765132:
                        return 27;
                    case 36415522:
                        if (firstOnField)
                            return 28;
                        else
                            return 29;
                    case 5672432:
                        return 30;
                    case 78184733:
                        return 31;
                    case 16720314:
                        return 32;
                    case 95254840:
                        return 33;
                    case 84274024:
                        if (firstOnField)
                            return 34;
                        else
                            return 35;
                    case 9852718:
                        return 36;
                    case 58169731:
                        return 37;
                    case 92958307:
                        return 38;
                    case 16178681:
                        return 39;
                    case 21770839:
                        return 40;
                    case 93149655:
                        return 41;
                    case 21250202:
                        return 42;
                    case 67754901:
                        return 43;
                    case 86238081:
                        return 44;
                    case 45627618:
                        return 45;
                    case 45014450:
                        return 46;
                    case 58074177:
                        return 47;
                    case 70335319:
                        return 48;
                    case 16306932:
                        return 49;
                    case 82224646:
                        return 50;
                    case 46136942:
                        return 51;
                    case 86157908:
                        return 52;
                    case 91584698:
                        return 53;
                    case 42002073:
                        return 54;
                    case 43241495:
                        return 55;
                    case 9106362:
                        return 56;
                    case 69211541:
                        return 57;
                    case 4239451:
                        return 58;
                    case 17330916:
                        return 59;
                    case 44481227:
                        return 60;
                    case 73130445:
                        return 61;
                    case 9000988:
                        return 62;
                    case 33656832:
                        return 63;
                    case 45667991:
                        return 64;
                    case 80335817:
                        return 65;
                    case 71692913:
                        return 66;
                    case 17086528:
                        return 67;
                    case 80896940:
                        return 68;
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 53025096:
                        return 0;
                    case 19221310:
                        if ((location & (uint)CardLocation.Hand) > 0)
                            return 101;
                        else
                            return 1;
                    case 21770839:
                        if((location & (uint)CardLocation.Hand) > 0)
                            return 2;
                        else
                            return 3;
                    case 93149655:
                        return 4;
                    case 21250202:
                        return 5;
                    case 67754901:
                        return 6;
                    case 83347294:
                        if ((location & (uint)CardLocation.Hand) > 0)
                            return 107;
                        else 
                            return 7;
                    case 72378329:
                        return 8;
                    case 88305705:
                        return 9;
                    case 82044279:
                        return 10;
                    case 41209827:
                        return 11;
                    case 16195942:
                        return 12;
                    case 1621413:
                        if(chain == 1)
                            return 13;
                        else
                            return 14;
                    case 86238081:
                        return 15;
                    case 45627618:
                        if((location& (uint)CardLocation.MonsterZone) > 0)
                            return 16;
                        else
                            return 17;
                    case 45014450:
                        return 18;
                    case 58074177:
                        return 19;
                    case 70335319:
                        if(chain == 1)
                            return 20;
                        else
                            return 21;
                    case 16306932:
                        if ((location & (uint)CardLocation.Hand) > 0)
                            return 122;
                        else
                            return 22;
                    case 42562690:
                        return 23;
                    case 79967395:
                        return 24;
                    case 91449532:
                        return 25;
                    case 82224646:
                        return 26;
                    case 46136942:
                        if ((location & (uint)CardLocation.Hand) > 0)
                            return 127;
                        else
                            return 27;
                    case 86157908:
                        return 28;
                    case 78835747:
                        return 29;
                    case 26270847:
                        return 30;
                    case 15978426:
                        return 31;
                    case 15452043:
                        return 32;
                    case 17857780:
                        return 33;
                    case 41440148:
                        return 34;
                    case 91584698:
                        return 35;
                    case 42002073:
                        return 36;
                    case 43241495:
                        return 37;
                    case 9106362:
                        return 38;
                    case 69211541:
                        return 39;
                    case 4239451:
                        return 40;
                    case 96606246:
                        return 41;
                    case 44364077:
                        return 42;
                    case 17330916:
                        return 143;
                    case 44481227:
                        return 44;
                    case 73130445:
                        return 45;
                    case 9000988:
                        return 46;
                    case 59123194:
                        return 47;
                    case 33656832:
                        if ((location & (uint)CardLocation.Hand) > 0)
                            return 148;
                        else
                            return 48;
                    case 45667991:
                        return 49;
                    case 80335817:
                        return 50;
                    case 94415058:
                        return 51;
                    case 71692913:
                        return 52;
                    case 17086528:
                        return 53;
                    case 54941203:
                        if ((location & (uint)CardLocation.Hand) > 0)
                            return 154;
                        else
                            return 54;
                    case 80896940:
                        return 55;
                }
                break;
        }
        return -10;
    }
    static int V0603(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 78437364:
                        return Random.Range(0, 2);
                    case 30010480:
                        return 2;
                    case 22510667:
                        return 3;
                    case 95515058:
                        return 4;
                    case 47946130:
                        return 5;
                    case 11516241:
                        return 6;
                    case 30286474:
                        return 7;
                    case 88406570:
                        return 8;
                    case 59644128:
                        return 9;
                    case 10552026:
                        return 10;
                    case 71555408:
                        return 11;
                    case 7540107:
                        return 12;
                    case 24073068:
                        return 13;
                    case 67586735:
                        return 14;
                    case 97688360:
                        return 15;
                    case 54088068:
                        return 16;
                    case 12097275:
                        return 17;
                    case 93581434:
                        return 18;
                    case 85008676:
                        return 19;
                    case 20191720:
                        return 20;
                    case 60461077:
                        return 21;
                    case 77967790:
                        return 22;
                    case 69121954:
                        return 23;
                    case 22900219:
                        return 24;
                    case 58672736:
                        return 25;
                    case 48372950:
                        return 26;
                    case 93507434:
                        return 27;
                    case 29996433:
                        return 28;
                    case 75366958:
                        return 29;
                    case 56980148:
                        return 30;
                    case 80831552:
                        return 31;
                    case 82385847:
                        return 32;
                    case 11755663:
                        return 33;
                    case 35770983:
                        return 34;
                    case 54446813:
                        return 35;
                    case 61764082:
                        return 36;
                    case 61269611:
                        return 37;
                    case 61668670:
                        return 38;
                }
                break;
            case "attack":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 78437364:
                        return 0;
                }
                return V0603("summon", card) - 1;
            case "activate":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 26285557:
                        return 0;
                    case 62376646:
                        return 1;
                    case 35870016:
                        return 2;
                    case 85638822:
                        //mark
                        if(firstOnField)
                            return 3;
                        else if(Program.I().ocgcore.MD_battleString == "03")
                            return 4;
                        else
                            return 5;
                    case 15543940:
                        return 6;
                    case 90173539:
                        if ((card.p.location & (uint)CardLocation.Grave) > 0)
                            return 8;
                        else
                            return 7;
                    case 39041729:
                        return 9;
                    case 95281259:
                        return 10;
                    case 32807846:
                        return 11;
                    case 78211862:
                        return 12;
                    case 34149830:
                        return 13;
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 30010480:
                        return 0;
                    case 22510667:
                        return 1;
                    case 95515058:
                        return 2;
                    case 47946130:
                        return 3;
                    case 11516241:
                        return 4;
                    case 30286474:
                        return 5;
                    case 88406570:
                        return 6;
                    case 59644128:
                        if((card.p.location & (uint) CardLocation.Grave) > 0)
                            return 8;
                        else
                            return 7;
                    case 10552026:
                        return 9;
                    case 71555408:
                        if ((card.p.location & (uint)CardLocation.Hand) > 0)
                            return 10;
                        else
                            return 11;
                    case 7540107:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 12;
                        else
                            return 13;
                    case 24073068:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 14;
                        else
                            return 15;
                    case 67586735:
                        //mark
                        return Random.Range(16, 18);
                    case 97688360:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 18;
                        else
                            return 19;
                    case 54088068:
                        if ((card.p.location & (uint)CardLocation.Hand) > 0)
                            return 20;
                        else
                            return 21;
                    case 12097275:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 22;
                        else
                            return 23;
                    case 93581434:
                        return 24;
                    case 85008676:
                        if ((card.p.location & (uint)CardLocation.Hand) > 0)
                            return 25;
                        else
                            return 26;
                    case 20191720:
                        return 27;
                    case 60461077:
                        return 28;
                    case 77967790:
                        return 29;
                    case 69121954:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 30;
                        else
                            return 31;
                    case 22900219:
                        return 32;
                    case 58672736:
                        return 33;
                    case 48372950:
                        return 34;
                    case 93507434:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 35;
                        else
                            return 36;
                    case 29996433:
                        return 37;
                    case 56980148:
                        if ((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            return 38;
                        else
                            return 39;
                    case 82385847:
                        return 40;
                    case 11755663:
                        if ((card.p.location & (uint)CardLocation.Hand) > 0)
                            return 41;
                        else
                            return 42;
                    case 35770983:
                        if ((card.p.location & (uint)CardLocation.Grave) > 0)
                            return 44;
                        else
                            return 43;
                    case 54446813:
                        if ((card.p.location & (uint)CardLocation.Grave) > 0)
                            return 46;
                        else
                            return 45;
                    case 61764082:
                        return 47;
                    case 61269611:
                        if ((card.p.location & (uint)CardLocation.Hand) > 0)
                            return 48;
                        else
                            return 49;
                    case 61668670:
                        return 50;
                }
                break;
        }
        return -10;
    }
    static int V9995(string type, gameCard card, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1)
    {
        switch (type)
        {
            case "summon":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 10802915:
                        return 0;
                    case 52823314:
                        return 1;
                    case 26202165:
                        return 2;
                    case 64892035:
                        return 3;
                    case 63012333:
                        return 4;
                }
                break;
            case "attack":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 10802915:
                        return Random.Range(0, 2);
                    case 52823314:
                        return 2;
                    case 26202165:
                        return Random.Range(3, 5);
                    case 64892035:
                        return Random.Range(5, 7);
                    case 63012333:
                        return Random.Range(7, 9);
                }
                break;
            case "activateM":
                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
                {
                    case 10802915:
                        return 0;
                    case 52823314:
                        return 1;
                    case 26202165:
                        return 2;
                    case 64892035:
                        return 3;
                    case 63012333:
                        return 4;
                }
                break;
        }

        return -10;
    }


    //    switch (card)
    //{
    //    case :
    //        return 0;
    //    case :
    //        return 1;
    //    case :
    //        return 2;
    //    case :
    //        return 3;
    //    case :
    //        return 4;
    //    case :
    //        return 5;
    //    case :
    //        return 6;
    //    case :
    //        return 7;
    //    case :
    //        return 8;
    //    case :
    //        return 9;
    //    case :
    //        return 10;
    //    case :
    //        return 11;
    //    case :
    //        return 12;
    //    case :
    //        return 13;
    //    case :
    //        return 14;
    //    case :
    //        return 15;
    //    case :
    //        return 16;
    //    case :
    //        return 17;
    //    case :
    //        return 18;
    //    case :
    //        return 19;
    //    case :
    //        return 20;
    //    case :
    //        return 21;
    //    case :
    //        return 22;
    //    case :
    //        return 23;
    //    case :
    //        return 24;
    //    case :
    //        return 25;
    //    case :
    //        return 26;
    //    case :
    //        return 27;
    //    case :
    //        return 28;
    //    case :
    //        return 29;
    //    case :
    //        return 30;
    //    case :
    //        return 31;
    //    case :
    //        return 32;
    //    case :
    //        return 33;
    //    case :
    //        return 34;
    //    case :
    //        return 35;
    //    case :
    //        return 36;
    //    case :
    //        return 37;
    //    case :
    //        return 38;
    //    case :
    //        return 39;
    //    case :
    //        return 40;
    //    case :
    //        return 41;
    //    case :
    //        return 42;
    //    case :
    //        return 43;
    //    case :
    //        return 44;
    //    case :
    //        return 45;
    //    case :
    //        return 46;
    //    case :
    //        return 47;
    //    case :
    //        return 48;
    //    case :
    //        return 49;
    //    case :
    //        return 50;
    //    case :
    //        return 51;
    //    case :
    //        return 52;
    //    case :
    //        return 53;
    //    case :
    //        return 54;
    //    case :
    //        return 55;
    //    case :
    //        return 56;
    //    case :
    //        return 57;
    //    case :
    //        return 58;
    //    case :
    //        return 59;
    //    case :
    //        return 60;
    //    case :
    //        return 61;
    //    case :
    //        return 62;
    //    case :
    //        return 63;
    //    case :
    //        return 64;
    //    case :
    //        return 65;
    //    case :
    //        return 66;
    //    case :
    //        return 67;
    //    case :
    //        return 68;
    //    case :
    //        return 69;
    //    case :
    //        return 70;
    //    case :
    //        return 71;
    //    case :
    //        return 72;
    //    case :
    //        return 73;
    //    case :
    //        return 74;
    //    case :
    //        return 75;
    //    case :
    //        return 76;
    //    case :
    //        return 77;
    //    case :
    //        return 78;
    //    case :
    //        return 79;
    //    case :
    //        return 80;
    //    case :
    //        return 81;
    //    case :
    //        return 82;
    //    case :
    //        return 83;
    //    case :
    //        return 84;
    //    case :
    //        return 85;
    //    case :
    //        return 86;
    //    case :
    //        return 87;
    //    case :
    //        return 88;
    //    case :
    //        return 89;
    //    case :
    //        return 90;
    //    case :
    //        return 91;
    //    case :
    //        return 92;
    //    case :
    //        return 93;
    //    case :
    //        return 94;
    //    case :
    //        return 95;
    //    case :
    //        return 96;
    //    case :
    //        return 97;
    //    case :
    //        return 98;
    //    case :
    //        return 99;
    //}


//            switch (type)
//        {
//            case "summon":
//                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
//                {

//                }
//                break;
//            case "attack":
//                return V0209("summon", card);
//            case "activate":
//                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
//                {

//                }
//                break;
//            case "activateM":
//                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
//                {

//                }
//                break;
//            case "summonline":
//                switch (card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias)
//                {

//                }
//                break;
//        }
//return -10;
}
