using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eNameGroupType
{
    NPC,
    Skeleton,
    Zombie,
    Mummy,
    Goblin,
    Orc,
    Fimir,
    ChaosWarrior,
    Gargoyle,
    ChaosScorcer,
    Barbarian,
    Dwarf,
    Elf,
    Wizard,
   

}
public class NameManger : MonoBehaviour
{
    public static NameManger Instance { get; private set; }
    private Dictionary<eNameGroupType, List<string>> dGetListFromGroup = new Dictionary<eNameGroupType, List<string>>()
    {
        { eNameGroupType.Barbarian, new List<string>()
            {"Glend",
            "Tyod",
            "Vaedr",
            "Rhathrel",
            "Blaermerr",
            "Sgoglek",
            "Gheltonn",
            "Sgaznonn",
            "Dhistibles",
            "Frorvublelr",
            "Ford",
            "Ghirn",
            "Jalk",
            "Sketedr",
            "Eirngret",
            "Mulbilf",
            "Hitinn",
            "Tesreis",
            "Bhortmengerr",
            "Stuzgerbemm",}
        },


        { eNameGroupType.Wizard, new List<string>()
            {"Ryknil Istribush",
            "Forucne Nerardros",
            "Focsohi Mursei",
            "Zivrydh Shrarelva",
            "Encukh Amodrar",
            "Colgi Mudru",
            "Ranvon Bhiasilnec",
            "Iphreno Crugrai",
            "Ibrahi Weezonze",
            "Detirn Ghursas",
            "Ruthac Krezreerai",
            "Alnica Magozie",
            "Pidamo Yerlat",
            "Relro Tholdrarie",
            "Rhulmedh Arzinod",
            "Ucmud Shreezegec",
            "Cupunso Kheindus",
            "Furdrorn Karlos",
            "Cebonra Magish",
            "Brebsarn Bairthec",}

        },
        { eNameGroupType.Dwarf, new List<string>()
            {"Torerburum Underthane",
            "Lormouck Platespine",
            "Dwoznatir Goldarm",
            "Helgratin Wyvernbelt",
            "Movear Undersword",
            "Elkeat Platemantle",
            "Whughod Windbrand",
            "Hogroc Koboldfoot",
            "Wegrog Anvilforge",
            "Skardog Mithrilhead",
            "Orirdog Platearm",
            "Bakreath Snowaxe",
            "Otmoir Dragonfall",
            "Douleas Bottleshaper",
            "Dhomdouk Wyvernchest",
            "Buvruik Strongbuckle",
            "Heznet Chaosbraid",
            "Grataem Giantfoot",
            "Krosot Flatbranch",
            "Uminon Deepriver",}
        },

        { eNameGroupType.Elf, new List<string>()
          {"Ayduin Wranpetor",
        "Folmer Chaetumal",
        "Tamnaeth Umelee",
        "Garynnon Faquinal",
        "Illianaro Keycan",
        "Abarat Dorgolor",
        "Ryul Yelxina",
        "Haladavar Yestumal",
        "Vaeril Presnelis",
        "Aymer Dorydark",
        "Maradeim Heleralei",
        "Ryul Loraphine",
        "Taleasin Yinqirelle",
        "Castien Chaecaryn",
        "Laeroth Olara",
        "Agandaur Ulacyne",
        "Flardryn Adxalim",
        "Theoden Wynvyre",
        "Aeson Zumroris",
        "Kivessin Yelvyre",}

        },
        { eNameGroupType.Skeleton, new List<string>()
            {"Khog",
            "Buk",
            "Rug",
            "Kordrut",
            "Zoukzax",
            "Khukdok",
            "Duststriker",
            "Earthbuster",
            "Facebasher",
            "Doomhead",
            "Bak",
            "Zod",
            "Rux",
            "Vatad",
            "Zodrat",
            "Khutraz",
            "Deathwatcher",
            "Dimrobber",
            "Skinsurge",
            "Pinefury",
            "Buc",
            "Bod",
            "Vac",
            "Kiquz",
            "Khaxziq",
            "Oxzoq",
            "Giantcutter",
            "Doombone",
            "Carrionguard",
            "Grimslobber",}
        },
        { eNameGroupType.Zombie, new List<string>()
            {"Naxaeon",
            "Kisdaon",
            "Rudnetus",
            "Thonagos",
            "Xosas",
            "Deiatter",
            "Chriardetheus",
            "Zosaestus",
            "Zozaon",
            "Chryntholus",
            "Phide",
            "Chromy",
            "Ziano",
            "Xodro",
            "Karla",
            "Naraizo",
            "Kodreuzy",
            "Vittalo",
            "Hilama",
            "Masume",
            "Vimbraestus",
            "Dancanos",
            "Vulius",
            "Cheiascaon",
            "Pharlaenon",
            "Chaettoeis",
            "Viadaumas",
            "Daverus",
            "Nustrotus",
            "Chrudnotus",}
        },

        { eNameGroupType.Mummy, new List<string>()
            {"Katic The Doctor",
            "Yelekai The Tyrant",
            "Coxius Grimm",
            "Chaexius The Mad",
            "Ecrabrum The Renovator",
            "Puvras The Renewer",
            "Rizis The Abomination",
            "Yomon Carnage",
            "Couvras The Corpsemaker",
            "Dibrum The Insane",
            "Agabrum The Crippled",
            "Tekai Malicius",
            "Hovras The Raised",
            "Peivras Grimm",
            "Cruqir Kane",
            "Streilak The Blight",
            "Staezad The Dissector",
            "Straezad The Corruptor",
            "Chrimien The Demise",
            "Vekhar The Risen",
            "Aprurotia The Undertaker",
            "Prauviah The Beast",
            "Chauren The Feeble",
            "Veselm Haggard",
            "Louroti The Reaper",
            "Riocia The Blight",
            "Shepeste Fester",
            "Uxaevana The Black",
            "Zraubea The Insane",
            "Chireas Crow",}
        },

         { eNameGroupType.Goblin, new List<string>()
           {"Ox",
            "Crict",
            "Kriasz",
            "Craakx",
            "Pralb",
            "Drekorx",
            "Grutnoikt",
            "Giekniert",
            "Blegzec",
            "Krakoil",
            "Pliz",
            "Buld",
            "Gnel",
            "Teelk",
            "Jukz",
            "Kisbil",
            "Gokkiolx",
            "Zraging",
            "Fatziork",
            "Fraatex",
            "Qaass",
            "Wagee",
            "Thrulx",
            "Punxe",
            "Sloilk",
            "Threebhaahx",
            "Prostreerxe",
            "Gapfoigs",
            "Haptaqea",
            "Shanosh",}
        },

         { eNameGroupType.Orc, new List<string>()

            {"Urulg",
            "Wrukag",
            "Milug",
            "Ergoth",
            "Ortguth",
            "Ouhgan",
            "Garothmuk",
            "Gug",
            "Uzul",
            "Shakh",
            "Badbog",
            "Rogmesh",
            "Gulfim",
            "Glob",
            "Sharn",
            "Ugak",
            "Umog",
            "Oghash",
            "Ushat",
            "Dulug",
            "Ortguth",
            "Kurdan",
            "Hoknath",
            "Yagnar",
            "Xothkug",
            "Turbag",
            "Snog",
            "Dur",
            "Oglub",
            "Zahgorim",}
        },

         { eNameGroupType.Fimir, new List<string>()
            {"Troat",
            "Voxl",
            "Rhut",
            "Chirni",
            "Yaukno",
            "Jimix",
            "Achomiz",
            "Ucarlok",
            "Choltaokkoak",
            "Chorsukkax",
            "Bhushk",
            "Thusz",
            "Chox",
            "Chutzuzk",
            "Romzak",
            "Itix",
            "Aoccima",
            "Agithoashk",
            "Goanjuxosk",
            "Bhaszajuk",
            "Rhex",
            "Thras",
            "Yiss",
            "Chochiss",
            "Krurus",
            "Khecciz",
            "Dizukri",
            "Chuyarshusk",
            "Daruzus",
            "Ecarass",}
         },
         { eNameGroupType.ChaosWarrior, new List<string>()
            {"Baxa",
            "Bzim'gidh",
            "Czun'qur",
            "Czaec'nok",
            "Stir'goudh",
            "Gheethaed",
            "Bzikkageek",
            "Nuan'ghougog",
            "Iarronoz",
            "Scukdaghyx",
            "Hit'qoq",
            "Dhyk'zuq",
            "Crokredh",
            "Sharzar",
            "Cril'za",
            "Ramky",
            "Syx'rokaun",
            "Hos'teghul",
            "Prorkoroq",
            "Hurghari",}
            },

         { eNameGroupType.Gargoyle, new List<string>()
             {"Bramuatt",
            "Thauddatt",
            "Vomod",
            "Garon",
            "Stremdazz",
            "Akez",
            "Ovonaax",
            "Vrobatcog",
            "Kravromoth",
            "Khoscazan",
            "Mixizsae",
            "Penu",
            "Umoh",
            "Zaezlel",
            "Chazu",
            "Chuken",
            "Zobesmun",
            "Vaajisqa",
            "Rolkaku",
            "Huvivo",
            "Harock",
            "Rhekkux",
            "Stonad",
            "Noscick",
            "Dimmam",
            "Qalgoth",
            "Credograg",
            "Unobom",
            "Khosgadoz",
            "Vrojaguck",}
            },

         { eNameGroupType.ChaosScorcer, new List<string>()
            {"Dozahr",
            "Ipius",
            "Usior",
            "Urlidus",
            "Grakius",
            "Uroveus",
            "Ivoxium",
            "Gudore",
            "Exar",
            "Ogibahn",
            "Qavile",
            "Mephiane",
            "Utaxis",
            "Qaweahl",
            "Chuzyni",
            "Ulyn",
            "Uzaphi",
            "Odemaev",
            "Estrea",
            "Efora",
            "Sinora",
            "Akora",
            "Axis",
            "Otogoris",
            "Onelle",
            "Shizahne",
            "Idyrin",
            "Inora",
            "Iweahl",
            "Nohith",}

    },


    };
    private Dictionary<eNameGroupType, List<string>> dUsedNames = new Dictionary<eNameGroupType, List<string>>();
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        int i = 0;
        foreach (KeyValuePair<eNameGroupType, List<string>> alist in dGetListFromGroup)
        {
            dUsedNames[alist.Key] = new List<string>();
        }
     
    }


    public static string GetNameByGroupType(eNameGroupType inNameGroup)
    {
        return Instance.GetNameByGroupType_Local(inNameGroup);
    }
    private string GetNameByGroupType_Local(eNameGroupType inNameGroup)
    {
        if (dUsedNames[inNameGroup].Count == 0)
        {
            //your out of new names. RIP
            dUsedNames[inNameGroup] = new List<string>();
        }
        if (dUsedNames[inNameGroup].Count == dGetListFromGroup[inNameGroup].Count)
        {
            //your out of new names. RIP
            dUsedNames[inNameGroup].Clear();
            dUsedNames[inNameGroup] = new List<string>();
        }

       
        List<string> aNameList = dGetListFromGroup[inNameGroup];
        string strReturn = "";
        do
        {     
            strReturn = aNameList[Random.Range(0, aNameList.Count - 1)];
        } while (dUsedNames[inNameGroup].Contains(strReturn));
        dUsedNames[inNameGroup].Add(strReturn);
        return strReturn;
    }
}
