using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EscapeFromTheWoods.Objects {
    public class TreesGrid {
        public TreesGrid(double delta, Map map) {
            Delta = delta;
            Map = map;
            //aantal blokken berekenen volgens begrenzingen van map en grootte van de zijde van één gridblok
            NX = (int)(map.DX / delta) +1;
            NY = (int)(map.DY / delta) +1;
            //Grid initialiseren
            TreesData = new List<Tree>[NX][];
            for (int i = 0; i < NX; i++) {
                TreesData[i] = new List<Tree>[NY];
                for (int j = 0; j < NY; j++) TreesData[i][j] = new List<Tree>();
            }
        }

        public TreesGrid(double delta, Map map, List<Tree> trees) : this(delta, map) {
            foreach (Tree tree in trees) {
                AddTree(tree);
            }
        }

        public double Delta { get; private set; }
        public List<Tree>[][] TreesData { get; private set; }
        public int NX { get; private set; }
        public int NY { get; private set; }
        public Map Map { get; private set; }

        public void AddTree(Tree tree) {
            //Tree toevoegen aan bijhorend blok op basis van coördinaten
            int i = (int)((tree.x - Map.xmin) / Delta);
            int j = (int)((tree.y - Map.ymin) / Delta);
            if (i == NX) i--;
            if (j == NY) j--;
            TreesData[i][j].Add(tree);
        }
    }
}