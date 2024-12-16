using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Valorant.API.External
{
    public class GameContent
    {
        private const int CCount = 5;

        public List<Agent> Agents;
        public List<CompetitiveTier> CompetitiveTiers;
        public List<LevelBorder> LevelBorders;
        public List<Weapon> Weapons;
        public List<Weapon.WeaponSkin> WeaponSkins;
        public List<Map> Maps;
        public List<SkinBundles.SkinBundleItem> Bundles;

        public void LoadContents()
        {
                Agents = Agent.GetAgents();
                CompetitiveTiers = CompetitiveTier.GetTiers();
                LevelBorders = LevelBorder.GetLevelBorders();
                Weapons = Weapon.GetWeapons();
                WeaponSkins = Weapon.GetWeaponSkins();
            Maps = Map.GetMaps();
            Bundles = SkinBundles.GetBundles();
        }
    }
}
